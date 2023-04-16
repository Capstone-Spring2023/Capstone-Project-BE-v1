using AutoScheduling.DataLayer;
using Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduling.Reader
{
    
    public class ScheduleReader
    {
        //private readonly string filePath = @"D:\Schedule\schedule.csv";
        public async Task fromScheduleFile_writeToDatabase(IFormFile file,int semesterid)
        {
            var userId_subjectCode_Slot_day_slot = new List<(int, string, string, int, int)>();
            List<(string, string)> list = new List<(string, string)>()
            {
                ("A1","A2"),("P1","P2"),
                ("A3","A4"),("P3","P4"),
                ("A5","A6"),("P5","P6")
            };
            using (var _context = new CFManagementContext())
            {
                
               
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var parts = line.Split(',');
                        int lecturerId = int.Parse(parts[0]);
                        string classCode = parts[3].ToUpper().Trim();
                        string subjectName = parts[2];
                        int asubjectId = _context.AvailableSubjects
                            .First(x => x.SemesterId == 1 && x.SubjectName == subjectName).AvailableSubjectId;
                        var registerSubject = _context.RegisterSubjects.FirstOrDefault(x => x.AvailableSubjectId == asubjectId && x.UserId == lecturerId);
                        int registerSubjectId;//= _context.RegisterSubjects.First(x => x.AvailableSubjectId == asubjectId && x.UserId == lecturerId).RegisterSubjectId;
                        if (registerSubject == null)
                        {
                            registerSubject = new RegisterSubject()
                            {
                                AvailableSubjectId = asubjectId,
                                ClassId = 123,
                                IsRegistered = false,
                                Status = true,
                                RegisterDate = DateTime.Now,
                                UserId = lecturerId,
                                
                            };
                            _context.RegisterSubjects.Add(registerSubject);   
                        }
                        else
                        {
                            registerSubject.Status = true;
                        }
                        await _context.SaveChangesAsync();
                        registerSubjectId = registerSubject.RegisterSubjectId;
                        var class1 = _context.Classes.First(x => x.ClassCode == classCode);
                        class1.RegisterSubjectId = registerSubjectId;
                        await _context.SaveChangesAsync();
                        int day, slot;
                        ClassDaySlotReader.APx_to_day_slot(parts[4], out day, out slot);
                        userId_subjectCode_Slot_day_slot.Add((lecturerId, subjectName, parts[4], day, slot));
                    }

                }
                

            }
           
            Getter UserGetter = new Getter();

            var userDic_andD = UserGetter.getAllUser();
            var alphaIndexs = userDic_andD.Item3.ToArray();
            var d = userDic_andD.Item2;
            var getter = new RegisterSubjectGetter();
            var registerSubjectAndSlots = getter.readRegisterSubject();

            var u = new Dictionary<int, double>();


            using (var _context = new CFManagementContext())
            {
                foreach (var a in registerSubjectAndSlots)
                {
                    u.Add(a.userId, Constant.POINT);
                    //Lấy register Subject and slot tương ứng với user\
                    var registerSubjects = a.RegisterSubjects;
                    var registerSlots = a.RegisterSlots;
                    var userList = userId_subjectCode_Slot_day_slot
                        .Where(x => x.Item1 == a.userId)
                        .OrderBy(x => (x.Item4, x.Item5));
                    //.ToList();
                    int previous_day = -1, previous_slot = -1;
                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine($"UserId: {userDic_andD.Item1.First(x => x.Item2 == a.userId).Item3}");
                    List<string> subjectCodeAlreadyMinus = new List<string>();
                    foreach (var userId_subjectCode_Slot_item in userList)
                    {
                        
                        var ui = u[a.userId];
                        //Check Register Ssubject
                        if (!registerSubjects.Exists(x => x.AvailableSubject.SubjectName.ToUpper() == userId_subjectCode_Slot_item.Item2.Trim()))
                        {
                            if (!subjectCodeAlreadyMinus.Contains(userId_subjectCode_Slot_item.Item2.Trim()))
                            {
                                subjectCodeAlreadyMinus.Add(userId_subjectCode_Slot_item.Item2.Trim());
                                ui -= 2;
                                Console.WriteLine($"Minus register Subject by subject: {userId_subjectCode_Slot_item.Item2} - new value: {ui}");
                            }
                            
                        }
                        //Check Register Slot
                        var slotAPx = list.First(x => x.Item1 == userId_subjectCode_Slot_item.Item3
                                            || x.Item2 == userId_subjectCode_Slot_item.Item3);
                        if (!registerSlots.Exists(x => x.Slot == slotAPx.Item1))
                        {
                            
                            ui--;
                            Console.WriteLine($"Minus register slot by slot: {userId_subjectCode_Slot_item.Item3} - new value: {ui}");
                        }
                        
                        //check độ khít của slot
                        int day = userId_subjectCode_Slot_item.Item4;
                        int slot = userId_subjectCode_Slot_item.Item5;
                        Console.WriteLine($"*************** - day: {day} - slot: {slot}");
                        if (previous_day != -1 && previous_day == day)
                        {
                            
                            ui = ui - (slot - previous_slot) + 1;
                            if (slot - previous_slot > 1)
                                Console.WriteLine($"Minus on tighten slot - day: {day} - previous slot: {previous_slot} - slot: {slot} - new value: {ui}");
                        }
                        previous_day = day;
                        previous_slot = slot;
                        u[a.userId] = ui;

                    }
                    int userIndex = userDic_andD.Item1.First(x => x.Item2 == a.userId).Item1;
                    var alphaIndex = userDic_andD.Item3[userIndex];
                    var ui_tmp = u[a.userId];
                    int count_num_teaching_class = userList.Count();
                    Console.WriteLine($"d[i]: {d[userIndex]} - numClass: {count_num_teaching_class} - value: {ui_tmp}");
                    if (count_num_teaching_class < d[userIndex] || count_num_teaching_class > 10)
                    {
                        ui_tmp -= Math.Abs(count_num_teaching_class - d[userIndex]);
                    }
                    else
                    {
                        ui_tmp += Math.Abs(count_num_teaching_class - d[userIndex]);
                    }
                    Console.WriteLine($"After Calculated - value: {ui_tmp}");
                    ui_tmp = ui_tmp * alphaIndex;

                    var pointIndex = _context.PointIndices.First(x => x.UserId == a.userId && x.SemesterId == semesterid);
                    pointIndex.NumClass = userList.Count();
                    pointIndex.UPoint = ui_tmp;
                    pointIndex.AlphaIndex = alphaIndex;
                    await _context.SaveChangesAsync();
                }
                var Indices = _context.PointIndices.Where(x => x.UserId != -1 && x.SemesterId == semesterid).ToList();

                var sum_uPoint = Indices.Sum(x => x.UPoint);
                var sum_numClass = Indices.Sum(_x => _x.NumClass);
                var sum_alphaIndex = Indices.Sum(_x => _x.AlphaIndex);
                
                foreach (var a in Indices )
                {
                    a.PercentPoint = (a.UPoint / sum_uPoint) * 100;
                    await _context.SaveChangesAsync();
                }
                var sumPointIndex = _context.PointIndices.First(x=> x.UserId == -1 && x.SemesterId == semesterid);
                sumPointIndex.NumClass = sum_numClass;
                sumPointIndex.UPoint = sum_uPoint;
                sumPointIndex.PercentPoint = 100;
                sumPointIndex.AlphaIndex=sum_alphaIndex;

                await _context.SaveChangesAsync();

                //chỉnh lại các register subject nhưng k dạy = false
                foreach(var a in registerSubjectAndSlots)
                {
                    var registerSubjects = a.RegisterSubjects;
                    foreach(var rs in registerSubjects)
                    {
                        var find = _context.Classes.FirstOrDefault(x=> x.RegisterSubjectId == rs.RegisterSubjectId);
                        if (find == null)
                        {
                            rs.Status = false;
                        }
                        var track = _context.Attach(rs);
                        track.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                       
                    }
                    
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}

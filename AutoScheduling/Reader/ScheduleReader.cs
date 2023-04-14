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
            var userId_subjectCode_Slot = new List<(int, string, string)>();
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

                        userId_subjectCode_Slot.Add((lecturerId,subjectName,parts[4]));
                    }

                }
                

            }
           
            Getter UserGetter = new Getter();

            var userDic_andD = UserGetter.getAllUser();
            var alphaIndexs = userDic_andD.Item3.ToArray();

            var getter = new RegisterSubjectGetter();
            var registerSubjectAndSlots = getter.readRegisterSubject();

            var u = new Dictionary<int, float>();


            using (var _context = new CFManagementContext())
            {
                foreach (var a in registerSubjectAndSlots)
                {
                    u.Add(a.userId, 20);
                    //Lấy register Subject and slot tương ứng với user\
                    var registerSubjects = a.RegisterSubjects;
                    var registerSlots = a.RegisterSlots;
                    var userList = userId_subjectCode_Slot.Where(x => x.Item1 == a.userId);
                    foreach (var userId_subjectCode_Slot_item in userList)
                    {
                        //Check Register Ssubject
                        if (!registerSubjects.Exists(x => x.AvailableSubject.SubjectName.ToUpper() == userId_subjectCode_Slot_item.Item2.Trim()))
                        {
                            var ui = u[a.userId];
                            ui--;
                        }
                        //Check Register Slot
                        var slot = list.First(x => x.Item1 == userId_subjectCode_Slot_item.Item3
                                            || x.Item2 == userId_subjectCode_Slot_item.Item3);
                        if (!registerSlots.Exists(x => x.Slot.ToUpper() == userId_subjectCode_Slot_item.Item3.ToUpper().Trim()))
                        {
                            var ui = u[a.userId];
                            ui--;
                        }

                    }
                    int userIndex = userDic_andD.Item1.First(x => x.Item2 == a.userId).Item1;
                    var alphaIndex = userDic_andD.Item3[userIndex];
                    var ui_tmp = u[a.userId];
                    ui_tmp = ui_tmp * alphaIndex;

                    var pointIndex = _context.PointIndices.First(x => x.UserId == a.userId && x.SemesterId == semesterid);
                    pointIndex.NumClass = userList.Count();
                    pointIndex.UPoint = ui_tmp;
                    await _context.SaveChangesAsync();

                }
                var sum = _context.PointIndices.Sum(x => x.UPoint);
                var Indices = _context.PointIndices.ToList();
                foreach (var a in Indices )
                {
                    a.PercentPoint = (a.UPoint / sum) * 100;
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using AutoScheduling.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoScheduling.Reader
{
    public class ClassDaySlotReader
    {
        private readonly CFManagementContext _context;
        public ClassDaySlotReader(CFManagementContext context)
        {
            _context = context;
        }
        public ClassDaySlotReader() { }
        public int[,,] readClassDaySlotCsv(IFormFile file,List<(int,string)> subjectDic,out List<(int,int,string)> subject_class_className)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                subject_class_className = new List<(int, int, string)>();
                for (int i = 0; i < 1; i++) reader.ReadLine();
                int classIndex = 0;
                var class_day_slot_list = new List<(int,int,int)>();
                //int[,,] class_day_slot = new int[]
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    string subjectName = parts[1];
                    string classGroup = parts[0];
                    string className = subjectName + "_" + classGroup;
                    string APx = parts[2];


                    int subjectIndex = subjectDic.First(x => x.Item2 == subjectName).Item1;
                    subject_class_className.Add((subjectIndex,classIndex,className));



                    int day, slot;
                    APx_to_day_slot(APx, out day, out slot);
                    class_day_slot_list.Add((classIndex,day,slot));

                    //class_day_slot_list.Add((classIndex, day + 3, slot));
                    classIndex++;
                }
                int[,,] class_day_slot = new int[classIndex, 6, 4];
                foreach (var a in class_day_slot_list)
                {
                    class_day_slot[a.Item1, a.Item2, a.Item3] = 1;
                    class_day_slot[a.Item1, a.Item2 + 3, a.Item3] = 1;
                }
                return class_day_slot;
            }
        }
        public async Task readClassDaySlotCsvToDb(IFormFile file,int semesterId)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                List<string> subjectsRaw = new List<string>();
                var subject_class_day_slot_slotAx = new List<(string, string, int, int,string)>();
                for (int i = 0; i < 1; i++) reader.ReadLine();
                int classIndex = 0;
                //int[,,] class_day_slot = new int[]
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(',');
                    string subjectName = parts[1];
                    string classGroup = parts[0];
                    string className = subjectName + "_" + classGroup;
                    string APx = parts[2];

                    if (!subjectsRaw.Exists(x => x.ToUpper().Trim() == subjectName.ToUpper().Trim())) subjectsRaw.Add(subjectName);
                    int day, slot;
                     APx_to_day_slot(APx,out day ,out slot);
                    

                    

                    subject_class_day_slot_slotAx.Add((subjectName, className, day, slot,APx));
                    //class_day_slot_list.Add((classIndex, day + 3, slot));
                }
                WriterToDB writer = new WriterToDB();
                DateTime startDate = DateTime.Parse("05-08-2023");
                await writer.writeAvaialbleSubject_Class_Schedule(semesterId, subjectsRaw, subject_class_day_slot_slotAx, startDate, 6);
                var listUserIdOfLecturerAndLeader = _context.Users.Where(x => x.RoleId == 2 || x.RoleId == 3).ToList();
                foreach (var user in listUserIdOfLecturerAndLeader)
                {
                    var notification = new Notification();
                    notification.Type = "schedule";
                    notification.UserId = user.UserId;
                    notification.Message = "has open class registration please register before deadline";
                    notification.Sender = null;
                    notification.SubjectCode = null;
                    notification.Status = "Unread";
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }
            }          
        }
        public static void APx_to_day_slot(string APx, out int day, out int slot)
        {
            //int day, slot;

            string a = APx.Substring(1);
            day = (int.Parse(a) - 1) / 2;

            if (APx.StartsWith("A"))
            {
                slot = (int.Parse(a) + 1 ) % 2;
            }
            else
            {
                slot = (int.Parse(a) + 1) % 2 + 2;
            }
            //return (day, slot);
        }
    }
}

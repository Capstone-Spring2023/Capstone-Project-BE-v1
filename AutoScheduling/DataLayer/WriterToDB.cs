using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoScheduling.DataLayer
{
    public class WriterToDB
    {
        public async Task writeAvaialbleSubject_Class_Schedule(int semesterId, List<string> subjects
            ,List<(string, string, int, int,string)> subject_class_day_slot_slotAPx, DateTime startDate, int summerTime)
        {
            using (var context = new CFManagementContext())
            {
                //Create Available Subject
                List<(string, int)> subjectName_AsubjectId = new List<(string, int)>();
                foreach (var a in subjects)
                {
                    Subject subject = await context.Subjects.FirstOrDefaultAsync(x => x.SubjectName.ToUpper() == a.ToUpper().Trim());
                    if (subject != null)
                    {
                        var ASubject = new AvailableSubject()
                        {
                            SubjectId = subject.SubjectId,
                            SemesterId = semesterId,
                            SubjectName = subject.SubjectName,
                            LeaderName = "Not yet",
                            LeaderId = -1,
                            Status = true,
                        };
                        context.Add(ASubject);
                        await context.SaveChangesAsync();
                        subjectName_AsubjectId.Add((subject.SubjectName, ASubject.AvailableSubjectId));
                    }
                }

                //Create Class
                var subject_classId = new List<(string, int)>(); 
                foreach (var a in subject_class_day_slot_slotAPx)
                {
                    var Class1 = new Class()
                    {
                        ClassCode = a.Item2.ToUpper(),
                        Slot = a.Item5,
                        Status = true
                    };
                    context.Add(Class1);
                    await context.SaveChangesAsync();
                    subject_classId.Add((a.Item1, Class1.ClassId));

                    // Create Schedule
                    int i = 1;
                    bool summer = false;
                    DateTime monday = startDate;
                    while (i <= 1)
                    {
                        if (i == summerTime && !summer)
                        {
                            summer = true;
                            monday = monday.AddDays(7);
                            continue;
                        }
                        Schedule schedule = new Schedule()
                        {
                            ScheduleDate = monday.AddDays(a.Item3),
                            Slot = a.Item4 + 1,
                            ClassId = Class1.ClassId
                        };
                        context.Add(schedule);
                        Schedule schedule1 = new Schedule()
                        {
                            ScheduleDate = monday.AddDays(a.Item3 + 3),
                            Slot = a.Item4 + 1,
                            ClassId = Class1.ClassId
                        };
                        context.Add(schedule1);
                        monday = monday.AddDays(7);
                        i++;
                    }
                }

                //Create Class_ASubject
                foreach(var a in subjectName_AsubjectId)
                {
                    var classId = subject_classId.First(x => x.Item1.ToLower().Trim() == a.Item1.ToLower().Trim()).Item2;
                    ClassAsubject classAsubject = new ClassAsubject()
                    {
                        ClassId = classId,
                        AsubjectId = a.Item2,
                        SubjectName = a.Item1,
                    };
                    context.Add(classAsubject);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}

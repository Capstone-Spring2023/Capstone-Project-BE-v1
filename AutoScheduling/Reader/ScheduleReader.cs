using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrTools.Reader
{
    
    public class ScheduleReader
    {
        private readonly string filePath = @"D:\Schedule\schedule.csv";
        public void fromScheduleFile_writeToDatabase()
        {
            using(var _context = new CFManagementContext())
            {
                using (var reader = new StreamReader(filePath))
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
                        int registerSubjectId = _context.RegisterSubjects.First(x => x.AvailableSubjectId == asubjectId && x.UserId == lecturerId).RegisterSubjectId;
                        
                        var class1 = _context.Classes.First(x => x.ClassCode == classCode);
                        class1.RegisterSubjectId = registerSubjectId;
                        _context.SaveChanges();
                    }
                }
            }
        }
    }
}

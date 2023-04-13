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
            using(var _context = new CFManagementContext())
            {
                //List<(int)> userId_subjectCode_Slot 
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



                    }
                }
            }
        }
    }
}

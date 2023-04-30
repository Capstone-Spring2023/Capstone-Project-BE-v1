using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AutoScheduling.Reader
{
    public class OutOfFlow
    {
        public async Task createAbleSubjectDatabase(IFormFile file)
        {
            RegisterSubjectReader reader = new RegisterSubjectReader();
            var list = reader.readRegisterSubjectFile(file);
            using(CFManagementContext _context = new CFManagementContext())
            {
                var subjects = _context.Subjects.ToList();
                foreach(var a in list)
                {
                    var userSubjects = a.Item3;
                    var user = _context.Users
                        .Include(x => x.Subjects)
                        .First(x => x.UserId == a.Item1);
                    var userSubjectsDb = user.Subjects;
                    foreach(var userSubjectName in userSubjects)
                    {
                        if (!userSubjectsDb.Select(x=> x.SubjectName).Contains(userSubjectName))
                        {
                            var subject = _context.Subjects.FirstOrDefault(x=>x.SubjectName == userSubjectName);
                            user.Subjects.Add(subject);
                        }
                    }
                }
                await _context.SaveChangesAsync();  
            }
        }
        public async Task createRegisterSubjectDatabaseFromFile(IFormFile file, int semesterId)
        {
            RegisterSubjectReader reader = new RegisterSubjectReader(); 
            var list = reader.readRegisterSubjectFile(file);
            using(CFManagementContext _context = new CFManagementContext())
            {
                foreach (var a in list)
                {
                    int lecturerId = a.Item1;
                    var subjects = a.Item3;
                    var slots = new List<String>();
                    if (a.Item4) slots.Add("A1");
                    if (a.Item5) slots.Add("P1");
                    if (a.Item6) slots.Add("A3");
                    if (a.Item7) slots.Add("P3");
                    if (a.Item8) slots.Add("A5");
                    if (a.Item9) slots.Add("P5");
                    
                    //Create subjects
                    foreach(var subject in subjects)
                    {
                        var AsubjectId = _context.AvailableSubjects.FirstOrDefault(x => x.SubjectName.ToUpper() == subject.ToUpper()
                        && x.SemesterId == semesterId
                        ).AvailableSubjectId;
                        RegisterSubject registerSubject = new RegisterSubject()
                        {
                            UserId = lecturerId,
                            AvailableSubjectId = AsubjectId,
                            ClassId = 123,
                            RegisterDate = DateTime.Now,
                            Status = true,
                            IsRegistered = true,
                        };
                        _context.Add(registerSubject);
                        
                    }
                    await _context.SaveChangesAsync();

                    //Create Slots
                    foreach (var slot in slots)
                    {
                        RegisterSlot registerSlot = new RegisterSlot()
                        {
                            SemesterId = semesterId,
                            UserId = lecturerId,
                            Slot = slot,
                            Status = true,
                        };
                        _context.Add(registerSlot);

                    }
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}

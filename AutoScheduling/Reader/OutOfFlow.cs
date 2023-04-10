using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OrTools.Reader
{
    public class OutOfFlow
    {
        public async Task createRegisterSubjectDatabaseFromFile()
        {
            RegisterSubjectReader reader = new RegisterSubjectReader(); 
            var list = reader.readRegisterSubjectFile();
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
                        var AsubjectId = _context.AvailableSubjects.FirstOrDefault(x => x.SubjectName.ToUpper() == subject.ToUpper()).AvailableSubjectId;
                        RegisterSubject registerSubject = new RegisterSubject()
                        {
                            UserId = lecturerId,
                            AvailableSubjectId = AsubjectId,
                            ClassId = 123,
                            RegisterDate = DateTime.Now,
                            Status = true,
                        };
                        _context.Add(registerSubject);
                        
                    }
                    await _context.SaveChangesAsync();

                    //Create Slots
                    foreach (var slot in slots)
                    {
                        RegisterSlot registerSlot = new RegisterSlot()
                        {
                            SemesterId = 1,
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

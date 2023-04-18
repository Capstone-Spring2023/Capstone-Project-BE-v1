using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
namespace AutoScheduling.DataLayer
{
    public class Updater
    {
        public void DeleteRedundantRegisterSubject()
        {
            using (CFManagementContext _context = new CFManagementContext())
            {
                var a = _context.RegisterSubjects.Where(x=> x.Classes.Count() == 0).ToList();
                _context.RemoveRange(a);
                _context.SaveChanges();
            }
        }
        public void UpdateDNumber(int semesterId, List<(int, string, List<string>, bool, bool, bool, bool, bool, bool, bool, int)> list)
        {
            using(CFManagementContext _context = new CFManagementContext())
            {
                foreach (var a in list)
                {
                    var user = _context.PointIndices.First(x => a.Item1 == x.UserId && x.SemesterId == semesterId);
                    
                }
            }
        }
    }
}

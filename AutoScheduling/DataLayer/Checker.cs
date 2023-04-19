using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
namespace AutoScheduling.DataLayer
{
    public class Checker
    {
        public bool checkIfThereAlreadyAvailableSubject(int semesterId)
        {
            using (CFManagementContext _context = new CFManagementContext())
            {
                var check = _context.AvailableSubjects.FirstOrDefault(x=> x.SemesterId == semesterId);
                if (check != null)
                {
                    return true;
                }
                else return false;
            }
        }
    }
}

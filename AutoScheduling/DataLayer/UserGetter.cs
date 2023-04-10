using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrTools.DataLayer
{
    public class Getter
    {
        public List<(int,int,string)> getAllUser()
        {
            using ( CFManagementContext context  = new CFManagementContext())
            {
                var users = context.Users.ToList();
                int userIndex = 0;
                List<(int,int,string)> result = new List<(int,int,string)> ();
                foreach (var user in users)
                {
                    result.Add((userIndex,user.UserId,user.FullName));
                    userIndex++;
                }
                return result;
            }
        }
        public List<(int,string)> getAllSubject(int semesterId)
        {
            using (CFManagementContext context = new CFManagementContext())
            {
                var subjects = context.AvailableSubjects
                    .Where(x => x.SemesterId == semesterId)
                    .ToList();
                int subjectIndex = 0;
                List<(int, string)> result = new List<(int, string)>();
                foreach (var asubject in subjects)
                {
                    //result.Add((userIndex, user.UserId));
                    result.Add((subjectIndex,asubject.SubjectName));
                    subjectIndex++;
                }
                return result;
            }
        }
    }
}

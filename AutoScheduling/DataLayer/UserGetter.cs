using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoScheduling.DataLayer
{
    public class Getter
    {
        public (List<(int,int,string)>,List<int>,List<float>) getAllUser()
        {
            
            using ( CFManagementContext context  = new CFManagementContext())
            {
                var users = context.Users
                    .Include(x => x.Subjects)
                    .Where(x => x.UserId != -1).ToList();
                int userIndex = 0;
                List<(int,int,string)> result = new List<(int,int,string)> ();
                List<int> d = new List<int>();
                List<float> alphas = new List<float>(); 
                foreach (var user in users)
                {
                    result.Add((userIndex,user.UserId,user.FullName));
                    d.Add((int) user.NumMinClass);
                    alphas.Add((float)user.AlphaIndex);
                    userIndex++;
                }
                return (result, d, alphas);
            }
        }
        public Dictionary<int,List<string>> getUserAndAvailableSubject(int semesterId)
        {
            using ( CFManagementContext context = new CFManagementContext())
            {
                var dic = new Dictionary<int,List<string>>();
                var users = context.Users
                 .Include(x => x.Subjects)
                 .Where(x => x.UserId != -1).ToList();
                var ASubjects = context.AvailableSubjects.Where(x=> x.SemesterId == semesterId);
                foreach(var user in users)
                {
                    var Asubjects = ASubjects.Where(x => user.Subjects.Select(x => x.SubjectId).Contains(x.SubjectId));
                    dic.Add(user.UserId, Asubjects.Select(x =>x.SubjectName).ToList());
                }
                return dic;
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

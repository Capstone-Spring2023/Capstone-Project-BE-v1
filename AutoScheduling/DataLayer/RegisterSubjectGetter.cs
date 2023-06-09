﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoScheduling.DataLayer
{
    public class UserRegisterSubjectAndSlot
    {
        public int userId { get; set; }
        public User User { get; set; }
        public List<RegisterSubject> RegisterSubjects { get; set; }
        public List<RegisterSlot> RegisterSlots { get; set; }
    }
    public class RegisterSubjectGetter
    {
        public List<UserRegisterSubjectAndSlot> readRegisterSubject(int semesterId)
        {
            using (var context = new CFManagementContext())
            {
                var res = new List<UserRegisterSubjectAndSlot>();
                var users = context.Users
                    .Skip(1)
                    .ToList();
                foreach (var user in users)
                {
                    var registerSubjects = context.RegisterSubjects
                        .Include(x => x.AvailableSubject)
                        .Where(x => x.UserId == user.UserId  && x.AvailableSubject.SemesterId ==semesterId).ToList();
                    var registerSlots = context.RegisterSlots.Where(x => x.UserId == user.UserId && x.SemesterId == semesterId).ToList();
                    var a = new UserRegisterSubjectAndSlot()
                    {
                        userId = user.UserId,
                        User = user,
                        RegisterSlots = registerSlots,
                        RegisterSubjects = registerSubjects
                    };
                    res.Add(a);
                }
                return res;
            }
            
        }
    }
}

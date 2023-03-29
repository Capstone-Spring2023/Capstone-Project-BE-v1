﻿using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Class
    {
        public Class()
        {
            ClassAsubjects = new HashSet<ClassAsubject>();
            Schedules = new HashSet<Schedule>();
        }

        public int ClassId { get; set; }
        public string ClassCode { get; set; } = null!;
        public bool Status { get; set; }

        public virtual ICollection<ClassAsubject> ClassAsubjects { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Class
    {
        public Class()
        {
            Schedules = new HashSet<Schedule>();
        }

        public int ClassId { get; set; }
        public int ClassNumber { get; set; }
        public int SemesterId { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
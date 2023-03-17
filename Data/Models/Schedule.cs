﻿using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int ClassId { get; set; }
        public DateTime Time { get; set; }
        public int Slot { get; set; }
        public bool ClassStatus { get; set; }

        public virtual Class Class { get; set; } = null!;
    }
}

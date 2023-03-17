using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Semester
    {
        public Semester()
        {
            AvailableSubjects = new HashSet<AvailableSubject>();
            CurrentHeaders = new HashSet<CurrentHeader>();
        }

        public int SemesterId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<AvailableSubject> AvailableSubjects { get; set; }
        public virtual ICollection<CurrentHeader> CurrentHeaders { get; set; }
    }
}

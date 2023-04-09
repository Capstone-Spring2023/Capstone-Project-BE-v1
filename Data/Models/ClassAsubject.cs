using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class ClassAsubject
    {
        public int ClassId { get; set; }
        public int AsubjectId { get; set; }
        public string SubjectName { get; set; } = null!;

        public virtual AvailableSubject Asubject { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
    }
}

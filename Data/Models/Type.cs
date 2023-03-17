using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Type
    {
        public Type()
        {
            ExamSchedules = new HashSet<ExamSchedule>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;

        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Type
    {
        public Type()
        {
            Subjects = new HashSet<Subject>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;

        public virtual ICollection<Subject> Subjects { get; set; }
    }
}

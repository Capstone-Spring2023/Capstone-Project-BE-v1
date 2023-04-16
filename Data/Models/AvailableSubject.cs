using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public partial class AvailableSubject
    {
        public AvailableSubject()
        {
            ClassAsubjects = new HashSet<ClassAsubject>();
            RegisterSubjects = new HashSet<RegisterSubject>();
        }

        public int AvailableSubjectId { get; set; }
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public int LeaderId { get; set; }
        public string? SubjectName { get; set; }
        public string? LeaderName { get; set; }
        public bool Status { get; set; }

        [JsonIgnore]
        public virtual User Leader { get; set; } = null!;
        [JsonIgnore]
        public virtual Semester Semester { get; set; } = null!;
        [JsonIgnore]
        public virtual Subject Subject { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<ClassAsubject> ClassAsubjects { get; set; }
        [JsonIgnore]
        public virtual ICollection<RegisterSubject> RegisterSubjects { get; set; }
    }
}

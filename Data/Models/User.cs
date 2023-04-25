using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class User
    {
        public User()
        {
            AvailableSubjects = new HashSet<AvailableSubject>();
            Notifications = new HashSet<Notification>();
            PointIndices = new HashSet<PointIndex>();
            RegisterSlots = new HashSet<RegisterSlot>();
            RegisterSubjects = new HashSet<RegisterSubject>();
            Subjects = new HashSet<Subject>();
        }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool Status { get; set; }
        public string? UserCode { get; set; }
        public int? NumMinClass { get; set; }
        public double? AlphaIndex { get; set; }
        public bool? IsColab { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<AvailableSubject> AvailableSubjects { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<PointIndex> PointIndices { get; set; }
        public virtual ICollection<RegisterSlot> RegisterSlots { get; set; }
        public virtual ICollection<RegisterSubject> RegisterSubjects { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
    }
}

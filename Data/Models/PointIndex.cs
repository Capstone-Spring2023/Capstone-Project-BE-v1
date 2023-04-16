using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Data.Models
{
    public partial class PointIndex
    {
        public int PointId { get; set; }
        public double? PercentPoint { get; set; }
        public double? UPoint { get; set; }
        public int? NumClass { get; set; }
        public int UserId { get; set; }
        public int SemesterId { get; set; }
        public double? AlphaIndex { get; set; }

        [JsonIgnore]
        public virtual Semester Semester { get; set; } = null!;
        [JsonIgnore]
        public virtual User User { get; set; } = null!;
    }
}

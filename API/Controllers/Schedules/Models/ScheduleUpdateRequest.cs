namespace API.Controllers.Schedules.Models
{
    public class ScheduleUpdateRequest
    {
        public string Type { get; set; }
        public int ScheduleId { get; set; }
        public int ClassId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int Slot { get; set; }
    }
}

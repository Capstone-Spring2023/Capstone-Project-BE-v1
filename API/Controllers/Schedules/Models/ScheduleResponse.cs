namespace API.Controllers.Schedules.Models
{
    public class ScheduleResponse
    {
        public int ScheduleId { get; set; }
        public int ClassId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int Slot { get; set; }
        public string ClassCode { get; set; }
    }
}

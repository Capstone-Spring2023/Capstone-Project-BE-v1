namespace API.Controllers.Schedules.Models
{
    public class RegisterSubjectSlot
    {
        public int userId { get; set; }
        public int semesterId { get; set; } = 1;
        public List<int> availableSubjectIds { get; set; }
        public List<string> registerSlots { get; set; }
    }
}

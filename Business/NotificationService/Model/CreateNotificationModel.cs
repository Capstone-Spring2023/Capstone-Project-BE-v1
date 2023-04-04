using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.NotificationService.Model
{
    public class CreateNotificationModel
    {
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int UserId { get; set; }
    }
}

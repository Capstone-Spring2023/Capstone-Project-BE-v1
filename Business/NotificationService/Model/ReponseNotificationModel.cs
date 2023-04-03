using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.NotificationService.Model
{
    public class ReponseNotificationModel
    {
        public int NotiId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int UserId { get; set; }
    }
}

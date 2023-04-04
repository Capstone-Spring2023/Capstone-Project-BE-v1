using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface INotificationRepository
    {
        public Task CreateNotification(Notification model);
        public Task DeleteNotification(int id);
        public Task<List<Notification>> GetAllNotificationsByUserId(int userId);
        public Task<Notification> GetNotification(int id);
        public Task MarkAsRead(int id);
    }
}

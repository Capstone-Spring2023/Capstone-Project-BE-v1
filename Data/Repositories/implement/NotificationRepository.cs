using Data.Models;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.implement
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly CFManagementContext _context;
        public NotificationRepository(CFManagementContext context) 
        {
            _context = context;
        }

        public async Task CreateNotification(Notification model)
        {
            _context.Notifications.Add(model);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteNotification(int id)
        {
            var notification = _context.Notifications.Find(id);
            if(notification != null)
            {
                notification.Status = "Deleted";
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetAllNotificationsByUserId(int userId)
        {
            var notifications = await _context.Notifications.Where(x => x.UserId == userId && x.Status != "Deleted").ToListAsync();
            return notifications;
        }

        public async Task<Notification> GetNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            return notification;
        }

        public async Task MarkAsRead(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification != null)
            {
                notification.Status = "Read";
            }
            await _context.SaveChangesAsync();
        }
    }
}

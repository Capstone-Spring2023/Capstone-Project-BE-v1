using Business.NotificationService.Model;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.NotificationService.Interfaces
{
    public interface INotificationService
    {
        public Task<ResponseModel> GetAllNotificaionsByUserId(int userId);
        public Task<ResponseModel> DeleteNotificaion(int id);
        public Task<ResponseModel> SaveNotification(CreateNotificationModel model);
        public Task<ResponseModel> MarkAsRead(int id);
    }
}

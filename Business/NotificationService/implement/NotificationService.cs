﻿using AutoMapper;
using Business.NotificationService.Interfaces;
using Business.NotificationService.Model;
using Data.Models;
using Data.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.NotificationService.implement
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<ResponseModel> DeleteNotificaion(int id)
        {
            var notification = await _notificationRepository.GetNotification(id);
            if (notification == null)
            {
                return null;
            }
            await _notificationRepository.DeleteNotification(id);
            return new()
            {
                StatusCode = 201,
                Data = "Deleted"
            };
        }

        public async Task<ResponseModel> GetAllNotificaionsByUserId(int userId)
        {
            var listNotification = await _notificationRepository.GetAllNotificationsByUserId(userId);
            var listResponse = new List<ReponseNotificationModel>();
            var listResponseNotification = new List<ReponseNotificationModel>();
            int count = 0;
            foreach (var notification in listNotification)
            {
                var responseNotification = _mapper.Map<ReponseNotificationModel>(notification);
                listResponse.Add(responseNotification);
                if(responseNotification.Status == "Unread")
                {
                    count += 1;
                }
            }
            
            for(var i = listResponse.Count - 1; i >= 0; i--)
            {
                listResponseNotification.Add(listResponse[i]);
            }
            var response = new ResponseNofication();
            response.reponseNotificationModels = listResponseNotification;
            response.countUnread = count;
            return new()
            {
                StatusCode = 200,
                Data = response
            };
        }

        public async Task<ResponseModel> MarkAsRead(int id)
        {
            var notification = await _notificationRepository.GetNotification(id);
            if (notification == null)
            {
                return null;
            }
            await _notificationRepository.MarkAsRead(id);
            return new()
            {
                StatusCode = 201,
                Data = "Read"
            };
        }

        public async Task<ResponseModel> SaveNotification(CreateNotificationModel model)
        {
            // status = 1 là chưa đọc
            var notification = _mapper.Map<Notification>(model);
            notification.Status = "Unread";
            await _notificationRepository.CreateNotification(notification);
            return new()
            {
                StatusCode = 201,
                Data = "Saved"
            };
        }
    }
}

using Business.NotificationService.Interfaces;
using Business.NotificationService.Model;
using Data.Models;
using Data.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Notifications
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService) 
        {
            _notificationService = notificationService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllNotificationsByUserId(int userId) 
        {
            var response = await _notificationService.GetAllNotificaionsByUserId(userId);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var response = await _notificationService.DeleteNotificaion(id);
            if(response == null)
            {
                return BadRequest();
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveNotification(CreateNotificationModel notification)
        {
            var response = await _notificationService.SaveNotification(notification);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var response = await _notificationService.MarkAsRead(id);
            if (response == null)
            {
                return BadRequest();
            }
            return Ok(response);
        }
    }
}

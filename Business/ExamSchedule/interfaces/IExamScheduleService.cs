using Business.ExamSchedule.Models;
using Business.NotificationService.Model;
using Business.UserService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamSchedule.interfaces
{
    public interface IExamScheduleService
    {
        public Task<ResponseModel> CreateExamSchedule(CreateExamScheduleModel examScheduleModel, int availableSubjectId);
        public Task<ResponseModel> GetExamSchedule(int id);
        public Task<ResponseModel> GetAllExamScheduleByLeaderId(int leaderId);
        Task<ObjectResult> UpdateExamSchedule(UpdateExamScheduleModel updateExamScheduleModel, int availableSubjectId);
        Task<ObjectResult> DeleteExamSchedule(int availableSubjectId);
        public Task<ResponseModel> GetDetailRequestExamSchedule(int availableSubjectId);
    }
}

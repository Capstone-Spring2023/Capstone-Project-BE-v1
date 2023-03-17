using Business.ExamSchedule.Models;
using Business.UserService.Models;
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
    }
}

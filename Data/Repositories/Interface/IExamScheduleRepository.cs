using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface IExamScheduleRepository
    {
        public Task CreateScheduleExam(ExamSchedule examSchedule);
        public Task<ExamSchedule> GetExamScheduleAsync(int id);
        public Task<List<ExamSchedule>> GetAllAsync();
        public Task<ExamSchedule> GetExamScheduleByRegisterSubjectId(int id);
        public Task<List<ExamSchedule>> GetAllExamScheduleByLeaderId(int leaderId);
    }
}

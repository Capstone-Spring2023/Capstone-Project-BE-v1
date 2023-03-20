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
        Task UpdateExamSchedule(List<ExamSchedule> examSchedule);
        Task DeleteExamSchedule (int examScheduleId);
        public Task CreateScheduleExam(ExamSchedule examSchedule);
        public Task<ExamSchedule> GetExamScheduleAsync(int id);
        public Task<List<ExamSchedule>> GetAllAsync();
        public Task<ExamSchedule> GetExamScheduleByRegisterSubjectId(int id);
        public Task<List<ExamSchedule>> GetAllExamScheduleByLeaderId(int leaderId);
        public Task<List<ExamSchedule>> getExamScheduleByAvailableSubjectId(int availableSubjectId);
    }
}

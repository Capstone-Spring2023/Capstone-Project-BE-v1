using Data.Models;
using Data.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface IAvailableSubjectRepository
    {
        public Task<List<AvailableSubject>> GetAllAvailableSubjectsByLeaderId(int id);
        Task<List<AvailableSubject>> GetAvailableSubjects(Expression<Func<AvailableSubject, bool>> ex, PagingRequest pageRequest);
        Task CreateAvailableSubject(AvailableSubject availableSubject);
        public Task<List<AvailableSubject>> GetAllAvailableSubjectsHaveExamScheduleByLeaderId(int leaderId);
        public Task<AvailableSubject> GetAvailableSubjectById(int id);
        public Task<List<AvailableSubject>> GetAvailableSubjectsByDepartmentId(int departmentId); 
    }
}

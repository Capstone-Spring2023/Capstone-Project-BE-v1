using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Paging;

namespace Data.Repositories.Interface
{
    public interface IExamPaperRepository
    {
        Task<ExamPaper> GetById(int id);
        Task<IEnumerable<ExamPaper>> GetAll(Expression<Func<ExamPaper, bool>> ex, PagingRequest pageRequest);
        Task DeleteExam(int id);
        Task CreateExam(ExamPaper exam);
        Task Update(ExamPaper exam);
        Task<List<ExamPaper>> GetAllByExamScheduleId(int examScheduleId);
    }
}

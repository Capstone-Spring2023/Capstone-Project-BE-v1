using Data.Models;

using Data.Paging;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Repositories.implement
{
    public class ExamPaperRepository : IExamPaperRepository
    {
        private readonly CFManagementContext _context;
        public ExamPaperRepository(CFManagementContext context)
        {
            _context = context;
        }
        public async Task CreateExam(ExamPaper exam)
        {
            
            _context.Add(exam);
            await _context.SaveChangesAsync();
            
        }

        public async Task DeleteExam(int id)
        {
            var exam = _context.ExamPapers.FirstOrDefault(x => x.ExamPaperId == id);
            _context.Remove(exam);
            await _context.SaveChangesAsync();
            
        }

        public async Task<IEnumerable<ExamPaper>> GetAll(Expression<Func<ExamPaper, bool>> ex, PagingRequest pageRequest)
        {
            var exams = await _context.ExamPapers
                .Include(x => x.Comments)
                .Where(ex)
                .Skip(pageRequest.PageSize * (pageRequest.PageIndex - 1))
                .Take(pageRequest.PageSize)
                .ToListAsync();
            return exams;
        }

        public async Task<List<ExamPaper>> GetAllByExamScheduleId(int examScheduleId)
        {
            var exam = await _context.ExamPapers.Where(x => x.ExamScheduleId == examScheduleId).ToListAsync();
            return exam;
        }

        public async Task<ExamPaper> GetById(int id)
        {
            return await _context.ExamPapers
                .Include(x => x.ExamSchedule)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x=> x.ExamPaperId == id); 
        }
        public async Task Update(ExamPaper exam)
        {
            var track = _context.Attach(exam);
            track.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}

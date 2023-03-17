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
            int id = _context.ExamPapers.Max(x => x.ExamPaperId) + 1;
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
                .Where(ex)
                .Skip(pageRequest.PageSize * (pageRequest.PageIndex - 1))
                .Take(pageRequest.PageSize)
                .ToListAsync();
            return exams;
        }

        public async Task<ExamPaper> GetById(int id)
        {
            return await _context.ExamPapers

                .FirstOrDefaultAsync(x=> x.ExamPaperId == id); 
        }

        public async Task UpdateExam(ExamPaper exam)
        {
            var exam1 = _context.ExamPapers.FirstOrDefault(x=> x.ExamPaperId == exam.ExamPaperId);
            var track = _context.Attach(exam1);
            track.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Update(ExamPaper exam)
        {
            _context.ExamPapers.Update(exam);
            await _context.SaveChangesAsync();
        }
    }
}

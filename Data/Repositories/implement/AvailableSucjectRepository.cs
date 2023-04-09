using Data.Models;
using Data.Paging;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.implement
{
    public class AvailableSucjectRepository : IAvailableSubjectRepository
    {
        private readonly CFManagementContext _context;
        public AvailableSucjectRepository(CFManagementContext context)
        {
            _context = context;
        }

        public async Task<List<AvailableSubject>> GetAllAvailableSubjectsHaveExamScheduleByLeaderId(int leaderId)
        {
            var listAvailableSubject = await _context
                .AvailableSubjects
                .Where(x => x.LeaderId == leaderId)
                .ToListAsync();
            listAvailableSubject = listAvailableSubject.Where(x => checkIfSubjectAlreadyHaveExamSchedule(x.AvailableSubjectId)).ToList();
            return listAvailableSubject;
        }
        public async Task<List<AvailableSubject>> GetAllAvailableSubjectsByLeaderId(int leaderId)
        {
            var listAvailableSubject = await _context
                .AvailableSubjects
                .Where(x => x.LeaderId == leaderId)
                .ToListAsync();
            listAvailableSubject = listAvailableSubject.Where(x => !checkIfSubjectAlreadyHaveExamSchedule(x.AvailableSubjectId)).ToList();
            return listAvailableSubject;
        }
        private bool checkIfSubjectAlreadyHaveExamSchedule(int availableSubjectId)
        {
            var examSchedules = _context.ExamSchedules.Where(x => x.AvailableSubjectId == availableSubjectId && x.Status == true);
            if (examSchedules == null || examSchedules.Count() == 0) return false;
            return true;
        }
        public async Task<List<AvailableSubject>> GetAvailableSubjects(Expression<Func<AvailableSubject, bool>> ex, PagingRequest pageRequest)
        {
            return await _context.AvailableSubjects
                .Where(ex)
                .Skip(pageRequest.PageSize * (pageRequest.PageIndex - 1))
                .Take(pageRequest.PageSize)
                .ToListAsync();
        }
        
        public async Task CreateAvailableSubject(AvailableSubject availableSubject)
        {
            _context.Add(availableSubject);
            await _context.SaveChangesAsync();
        }

        public async Task<AvailableSubject> GetAvailableSubjectById(int id)
        {
            return await _context.AvailableSubjects.FindAsync(id);
        }

        public async Task<List<AvailableSubject>> GetAvailableSubjectsByDepartmentId(int departmentId)
        {
            var listAvailableSubjects = await _context.AvailableSubjects.Where(x => x.Subject.DepartmentId == departmentId && x.Status).ToListAsync();
            return listAvailableSubjects;
        }
    }
}

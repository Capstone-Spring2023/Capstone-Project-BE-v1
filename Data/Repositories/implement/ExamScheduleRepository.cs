using Data.Models;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.implement
{
    public class ExamScheduleRepository:IExamScheduleRepository
    {
        private readonly CFManagementContext _context;
        public ExamScheduleRepository(CFManagementContext context)
        {
            _context = context;
        }

        public async Task CreateScheduleExam(ExamSchedule examSchedule)
        {
            _context.ExamSchedules.Add(examSchedule);
            await _context.SaveChangesAsync();

        }


        public async Task<ExamSchedule> GetExamScheduleAsync(int id)
        {
            var examSchedule = await _context.ExamSchedules.FindAsync(id);

            return examSchedule;
        }



        public async Task<List<ExamSchedule>> GetAllAsync()
        {
            var listExamSchedule = await _context.ExamSchedules.ToListAsync();
            if (listExamSchedule == null)
            {
                throw new ArgumentNullException(nameof(listExamSchedule));
            }
            return listExamSchedule;
        }

        public async Task<ExamSchedule> GetExamScheduleByRegisterSubjectId(int id)
        {
            var examSchedule = await _context.ExamSchedules.Where(x => x.RegisterSubjectId == id && x.Status == true).FirstOrDefaultAsync();
            return examSchedule;
        }

        public async Task<List<ExamSchedule>> GetAllExamScheduleByLeaderId(int leaderId)
        {
            var listExamSchedule = await _context.ExamSchedules.Where(x => x.LeaderId== leaderId && x.Status == true).ToListAsync();
            return listExamSchedule;
        }

        public async Task UpdateExamSchedule(List<ExamSchedule> examSchedules)
        {
            foreach (var a in examSchedules)
            {
                var track = _context.Attach(a);
                track.State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            
        }

        public async Task DeleteExamSchedule(int examScheduleId)
        {
            var a = _context.ExamSchedules.FirstOrDefault(x => x.ExamScheduleId == examScheduleId);
            a.Status = false;
            await _context.SaveChangesAsync();
        }

        public async Task<List<ExamSchedule>> getExamScheduleByAvailableSubjectId(int availableSubjectId)
        {
            var listExamShedule = await _context.ExamSchedules.Where(x => x.AvailableSubjectId == availableSubjectId && x.Status == true).ToListAsync();
            return listExamShedule;
        }
    }
}

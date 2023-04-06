using Data.Models;
using Data.Paging;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.implement
{
    public class RegisterSubjectRepository:IRegisterSubjectRepository
    {
        private readonly CFManagementContext _context;

        public RegisterSubjectRepository(CFManagementContext context)
        {
            _context = context;
        }

        public async Task CreateRegisterSubject(RegisterSubject registerSubject)
        {
            _context.RegisterSubjects.Add(registerSubject);
            await _context.SaveChangesAsync();
        }

        public Task<List<RegisterSubject>> GetAllRegisterSubject()
        {
            var listRegisterSubject = _context.RegisterSubjects.ToListAsync();
            return listRegisterSubject;
        }

        public async Task<RegisterSubject> GetRegisterSubjectById(int id)
        {
            var registerSubject = await _context.RegisterSubjects.FindAsync(id);
            return registerSubject;
        }

        public async Task<List<RegisterSubject>> SearchBySubjectId(int id)
        {
            var listRegisterSubject = await _context.RegisterSubjects.Where(x => x.AvailableSubjectId == id && x.Status == true).ToListAsync();
            return listRegisterSubject;
        }
        public async Task<List<User>> SearchTeachersBySubjectId(int subjectId, PagingRequest pageRequest)
        {
            var registerSubjects = await SearchBySubjectId(subjectId);
            var teacherIds = registerSubjects.Select(x => x.UserId);
            var teachers = await _context.Users
                .Where(x => teacherIds.Contains(x.UserId))
                .Skip(pageRequest.PageSize * (pageRequest.PageIndex - 1))
                .Take(pageRequest.PageSize)
                .ToListAsync();
            return teachers;
        }
    }
}

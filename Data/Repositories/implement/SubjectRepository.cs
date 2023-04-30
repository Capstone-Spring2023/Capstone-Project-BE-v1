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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly CFManagementContext _context;
        public SubjectRepository(CFManagementContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Subject>> GetAll()
        {
            return await _context.Subjects.ToListAsync();
        }
        public async Task<Subject> getSubject(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }
    }
}

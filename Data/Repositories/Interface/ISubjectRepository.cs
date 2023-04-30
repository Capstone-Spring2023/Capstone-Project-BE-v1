using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAll();
        public Task<Subject> getSubject(int id);
    }
}

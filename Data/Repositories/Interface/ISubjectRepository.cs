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
        public Task<Subject> getSubject(int id);
    }
}

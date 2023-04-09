using Data.Models;
using Data.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface IRegisterSubjectRepository
    {
        public Task<List<RegisterSubject>> SearchBySubjectId(int id);

        public Task<RegisterSubject> GetRegisterSubjectById(int id);

        public Task<List<RegisterSubject>> GetAllRegisterSubject();

        Task<List<User>> SearchTeachersBySubjectId(int subjectId, PagingRequest pageRequest);

        public Task CreateRegisterSubject(RegisterSubject registerSubject);
    }
}

using Business.RegisterSubjectService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.RegisterSubjectService.Interfaces
{
    public interface IRegisterSubjectService
    {
        public Task<ResponseModel> CreateRegisterSubject(CreateRegisterSubjectModel model);
        Task<ObjectResult> getRegisterSubjects(int userId);
    }
}

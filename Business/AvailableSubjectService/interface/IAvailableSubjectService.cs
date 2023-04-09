using Data.Models;
using Data.Paging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.AvailableSubjectService.Interface
{
    public interface IAvailableSubjectService
    {
        public  Task<ObjectResult> GetAvailableSubjects(Expression<Func<Data.Models.AvailableSubject, bool>> ex, PagingRequest paging);
        Task<ObjectResult> GetTeachersBySubjectId(int subjectId, PagingRequest paging);
        Task<ResponseModel> GetAllAvailableSubjectByLeaderId(int leaderId);
        Task<ResponseModel> GetAvailableSubjectById(int id);
        Task<ResponseModel> GetAvailableSubjectByDepartmentId(int departmentId);
        Task<ResponseModel> GetAllAvailableSubjectNotHaveRegister();
    }
}

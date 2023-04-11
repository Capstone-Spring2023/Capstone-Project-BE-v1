using Business.UserService.Models;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace Business.UserService.Interfaces
{
    public interface IUserService
    {
        public Task<ResponseModel> GetUser(int id);

        public Task<ResponseModel> UpdateUser(int id, UserModel user);
        public Task<ResponseModel> GetAllLeaders();
        public Task<ResponseModel> getAllDepartmentByHeader(int userId);
        public Task<ResponseModel> GetLecturersHaveRegisterSubjectByAvailableSubjectId(int availableSubjectId);
        public Task<ResponseModel> GetAllLecturerHaveRegisterSubject();
    }
}

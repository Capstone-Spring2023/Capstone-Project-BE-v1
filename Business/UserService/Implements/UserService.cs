using AutoMapper;
using Business.Constants;
using Business.UserService.Interfaces;
using Business.UserService.Models;
using Data.Models;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Xml.Xsl;

namespace Business.UserService.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly CFManagementContext _context;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, CFManagementContext context, IMapper mapper)
        {
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel> GetUser(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
            {
                return new()
                {
                    StatusCode = (int) StatusCode.NOTFOUND
                };
            }
            
            UserModel userModel = new UserModel(
                user.FullName, user.Phone.Trim(), user.Address, user.RoleId
            );
           
            return new()
            {
                StatusCode = (int) StatusCode.OK,
                Data = userModel
            };
        }

        public async Task<ResponseModel> UpdateUser(int id, UserModel userModel)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
            {
                return new()
                {
                    StatusCode = 400,
                };
            }
            user.FullName = userModel.fullName;
            user.Phone = userModel.phone.Trim();
            user.Address = userModel.address;
            user.RoleId = userModel.roldId;
            await _userRepository.UpdateUserAsync(id, user);
            return new()
            {
                StatusCode = (int) StatusCode.SUCCESS,
                Data = userModel
            };

        }

        public async Task<ResponseModel> GetAllLeaders()
        {
            var listUser = await _userRepository.GetAllAsync();
            var listLeader = new List<UserModel>();
            foreach (var user in listUser)
            {
                if(user.RoleId == ((int)Constants.Role.Leader))
                {
                    listLeader.Add(new UserModel(
                        user.FullName, user.Phone.Trim(), user.Address, user.RoleId
                    ));
                }
            }
            return new()
            {
                StatusCode= (int) StatusCode.OK,
                Data = listLeader
            };
        }

        public async Task<ResponseModel> getAllDepartmentByHeader(int userId)
        {
            var headers = _context.CurrentHeaders.Where(x => x.UserId == userId && x.Status).ToList();
            var ListDepartment = new List<ResponseDepartment>();
            foreach (var header in headers)
            {
                var department = _context.Departments.Find(header.DepartmentId);

                if(department != null && department.Status)
                {
                    ListDepartment.Add(_mapper.Map<ResponseDepartment>(department));
                }
                
            }
            return new()
            {
                StatusCode = 200,
                Data = ListDepartment
            };
        }

        public async Task<ResponseModel> GetLecturersHaveRegisterSubjectByAvailableSubjectId(int availableSubjectId)
        {
            var listRegisterSubjects = await _context.RegisterSubjects.Where(x => x.AvailableSubjectId == availableSubjectId && x.Status).ToListAsync();
            var listResponse = new List<ResponseLecturerModel>();
            foreach (var registerSubject in listRegisterSubjects)
            {
                var response = new ResponseLecturerModel();
                var availableSubject = _context.AvailableSubjects.Find(availableSubjectId);
                if(availableSubject != null && availableSubject.Status)
                {
                    var semester = _context.Semesters.Find(availableSubject.SemesterId);
                    response.semester = semester.Name;
                    var lecturer = _context.Users.Where(x => x.UserId == registerSubject.UserId).FirstOrDefault();
                    response.fullName = lecturer.FullName;
                    response.subjectName = _context.AvailableSubjects.Find(availableSubjectId).SubjectName;
                    listResponse.Add(response);
                }
                
            }
            return new()
            {
                StatusCode = 200,
                Data = listResponse
            };

        }
    }
}

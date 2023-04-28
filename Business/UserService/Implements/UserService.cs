using AutoMapper;
using Business.Constants;
using Business.ExamSchedule.Models;
using Business.UserService.Interfaces;
using Business.UserService.Models;
using Data.Models;
using Data.Repositories.implement;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
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
            var roleName = _context.Roles.Find(user.RoleId).RoleName;
            UserModel userModel = new UserModel(
                user.FullName, user.Phone.Trim(), user.Address, user.RoleId, roleName
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
                    var roleName = _context.Roles.Find(user.RoleId).RoleName;
                    listLeader.Add(new UserModel(
                        user.FullName, user.Phone.Trim(), user.Address, user.RoleId, roleName
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
                if (availableSubject != null && availableSubject.Status)
                {
                    var semester = _context.Semesters.Find(availableSubject.SemesterId);
                    response.semester = semester.Name;
                    response.semesterId = semester.SemesterId;
                    var lecturer = _context.Users.Where(x => x.UserId == registerSubject.UserId).FirstOrDefault();
                    response.fullName = lecturer.FullName;
                    response.subjectName = availableSubject.SubjectName;
                    var isLeader = false;
                    if (availableSubject.LeaderId == lecturer.UserId)
                    {
                        isLeader = true;
                    }
                    response.isCol = lecturer.IsColab;
                    response.isLeader = isLeader;
                    response.availableSubjectId = availableSubjectId;
                    var examSchedule = _context.ExamSchedules.Where(x => x.RegisterSubjectId == registerSubject.RegisterSubjectId && x.Status).FirstOrDefault();
                    if(examSchedule != null)
                    {
                        var examPaper = _context.ExamPapers.Where(x => x.ExamScheduleId == examSchedule.ExamScheduleId && x.Status != ExamPaperStatus.REJECTED).FirstOrDefault();
                        if (examPaper != null)
                        {
                            if(examPaper.Status == ExamPaperStatus.PENDING)
                            {
                                response.status = "Pending Review";
                            } 
                            if(examPaper.Status == ExamPaperStatus.APPROVED || examPaper.Status == ExamPaperStatus.APPROVED_MANUAL)
                            {
                                response.status = "Approved";
                            }
                            response.examLink = examPaper.ExamLink;
                        }
                        else
                        {
                            response.status = "Not Submit";
                        }
                        if (examSchedule.AppovalUserId != null)
                        {
                            var approvalUserName = _context.Users.Find(examSchedule.AppovalUserId).FullName;
                            response.approvalUserName = approvalUserName;
                        }                      
                    }
                    response.userId = lecturer.UserId;
                    listResponse.Add(response);
                }

            }
            return new()
            {
                StatusCode = 200,
                Data = listResponse
            };

        }

        public async Task<ResponseModel> GetAllLecturerHaveRegisterSubject()
        {
            var listRegister = await _context.RegisterSubjects.Where(x => x.Status).ToListAsync();
            var listUserId = listRegister.Select(x => x.UserId).Distinct().ToList();
            List<ResponseLecturerHaveRegisterSubject> listResponse = new List<ResponseLecturerHaveRegisterSubject>();
            listUserId.ForEach(delegate (int s)
            {
                var lecturer = new ResponseLecturerHaveRegisterSubject();
                lecturer.UserId = s;
                lecturer.FullName = _context.Users.Find(s).FullName;
                listResponse.Add(lecturer);
            });
            return new()
            {
                StatusCode = 200,
                Data = listResponse
            };
        }

        public async Task<ResponseModel> SetLeader(SetLeaderModel model)
        {
            var availableSubject = _context.AvailableSubjects.Find(model.AvailableSubjectId);
            var user = _context.Users.Find(model.UserId);
            if(user.RoleId != 1)
            {
                var oldLeader = _context.Users.Find(availableSubject.LeaderId);
                user.RoleId = 2;
                availableSubject.LeaderId = user.UserId;
                _context.Users.Update(user);
                _context.AvailableSubjects.Update(availableSubject);
                await _context.SaveChangesAsync();
                var check = _context.AvailableSubjects.Where(x => x.LeaderId == oldLeader.UserId && x.Status).ToList();
                if(check == null || !check.Any())
                {
                    oldLeader.RoleId = 3;
                    _context.Users.Update(oldLeader);
                }
                var notification = new Notification();
                notification.Type = "subjectLead";
                notification.UserId = model.UserId;
                var subjectName = _context.Subjects.Find(availableSubject.SubjectId).SubjectName;
                notification.Message = "you have been leader for " + subjectName;
                notification.Sender = _context.Users.Find(model.UserIdOfHeader).FullName;
                notification.SubjectCode = null;
                notification.Status = "Unread";
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                return new()
                {
                    StatusCode = 200,
                    Data = "Set Leader Success"
                };
            }
            return new()
            {
                StatusCode = 500,
                Data = "Something wrong"
            };
        }
        public async Task<ResponseModel> GetUserCanTeachByAvailableSubjectId(int availableSubjectId)
        {
            try
            {
                var listUser = await _context.Users.Include(x => x.Subjects).ToListAsync();
                var listResponse = new List<ResponseTeacher>();
                if (listUser != null)
                {
                    var availableSubject = _context.AvailableSubjects.Find(availableSubjectId);
                    if (availableSubject != null)
                    {
                        foreach (var user in listUser)
                        {
                            if(user.UserId == -1)
                            {
                                continue;
                            }
                            var response = new ResponseTeacher();
                            response.UserId = user.UserId;
                            response.FullName = user.FullName;
                            response.Status = false;
                            if (user.Subjects.Where(x => x.SubjectId == availableSubject.SubjectId).FirstOrDefault() != null)
                            {
                                response.Status = true;
                            }
                            listResponse.Add(response);
                        }
                    }
                }
                return new()
                {
                    StatusCode = 200,
                    Data = listResponse
                };
            }catch(Exception ex)
            {
                return new()
                {
                    StatusCode = 500,
                    Data = ex.Message
                };
            }
        }
    }
}

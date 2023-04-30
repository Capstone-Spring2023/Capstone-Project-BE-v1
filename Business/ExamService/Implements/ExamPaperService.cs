using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Data.Paging;
using Data.Repositories.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Business.ExamPaperService.Interfaces;
using Business.ExamService.Models;
using Data.Repositories.implement;
using Business.Constants;
using Business.ExamSchedule.Models;
using Data;

namespace Business.ExamPaperService.Implements
{
    public class ExamPaperService : IExamPaperService
    {
        private readonly IExamPaperRepository ExamPaperRepository;
        private readonly ICommentRepository CommentRepository;
        private readonly IExamScheduleRepository ExamScheduleRepository;
        private readonly IAvailableSubjectRepository _availableSubjectRepository;
        private readonly IRegisterSubjectRepository _registerSubjectRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITypeRepository _typeRepository;
        private readonly IUserRepository _userRepository;
        private readonly CFManagementContext _context;
        private IMapper mapper;
        private readonly INotificationRepository NotificationRepository;
        public ExamPaperService(CFManagementContext context, IExamPaperRepository ExamPaperRepository, 
            ICommentRepository commentRepository, IMapper mapper, IExamScheduleRepository examScheduleRepository,
            INotificationRepository NotificationRepository, IUserRepository userRepository, ITypeRepository typeRepository,
            ISubjectRepository subjectRepository, IRegisterSubjectRepository registerSubjectRepository, IAvailableSubjectRepository availableSubjectRepository)
        {
            this.ExamPaperRepository = ExamPaperRepository;
            this.mapper = mapper;
            this.CommentRepository = commentRepository;
            _context = context;
            this.ExamScheduleRepository = examScheduleRepository;
            this.NotificationRepository = NotificationRepository;
            _availableSubjectRepository = availableSubjectRepository;
            _subjectRepository = subjectRepository;
            _typeRepository = typeRepository;
            _registerSubjectRepository = registerSubjectRepository;
            _userRepository = userRepository;
        }

        public async Task<ObjectResult> CreateExam(int examScheduleId,ExamCreateRequestModel ExamPaperCreateRequest)
        {
            var ExamPaper = mapper.Map<ExamPaper>(ExamPaperCreateRequest);
            ExamPaper.ExamScheduleId = examScheduleId;
            ExamPaper.Status = ExamPaperStatus.PENDING;
            try
            {
                await ExamPaperRepository.CreateExam(ExamPaper);
                var notification = mapper.Map<Notification>(ExamPaperCreateRequest);
                var registerSubjectId = _context.ExamSchedules.Find(examScheduleId).RegisterSubjectId;
                var registerSubject = _context.RegisterSubjects.Find(registerSubjectId);
                notification.Sender = _context.Users.Find(registerSubject.UserId).FullName;
                var leaderId = _context.ExamSchedules.Find(examScheduleId).LeaderId;
                notification.UserId = leaderId;
                var availableSubject = _context.AvailableSubjects.Find(_context.ExamSchedules.Find(examScheduleId).AvailableSubjectId);
                var subject = _context.Subjects.Where(x => x.SubjectId == availableSubject.SubjectId && x.Status).FirstOrDefault();
                notification.SubjectCode = subject.SubjectCode;
                notification.Status = "Unread";
                await NotificationRepository.CreateNotification(notification);
                return new ObjectResult("Create Success")
                {
                    StatusCode = 201,
                };
            } catch (Exception ex)
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<ObjectResult> DeleteExam(int id)
        {
            try
            {
                await ExamPaperRepository.DeleteExam(id);
                return new ObjectResult("Delete Success")
                {

                    StatusCode = 200,
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message)
                {
                    StatusCode = 500,
                };
            }
        }

        public async Task<ObjectResult> GetAllExams(Expression<Func<ExamPaper, bool>> ex, PagingRequest paging)
        {
            try
            {
                var ExamPapers = await ExamPaperRepository.GetAll(ex, paging);
                List<ExamResponseModel> datas = ExamPapers.Select(x => mapper.Map<ExamResponseModel>(x)).ToList();
                foreach(var data in datas)
                {
                    var examSchedule = _context.ExamSchedules.FirstOrDefault(x => x.ExamScheduleId == data.ExamScheduleId);
                    data.SubjectName = _context.AvailableSubjects.FirstOrDefault(x => x.AvailableSubjectId == examSchedule.AvailableSubjectId).SubjectName;
                    
                    var register = _context.RegisterSubjects.Find(examSchedule.RegisterSubjectId);
                    data.LecturerName = _context.Users.Find(register.UserId).FullName;
                    var typeId = _context.ExamSchedules.Find(data.ExamScheduleId).TypeId;
                    data.Type = _context.Types.Find(typeId).TypeName;
                    var comment = ExamPapers.FirstOrDefault(x=> x.ExamPaperId == data.ExamPaperId).Comments.FirstOrDefault();
                    if (comment == null)
                    {
                        data.Comment = "";
                    }
                    else
                    {
                        data.Comment = comment.CommentContent.Trim();
                    }
                    
                }

                return new ObjectResult(datas)
                {
                    StatusCode = 200,
                };
            }
            catch (Exception exc)
            {
                return new ObjectResult(exc.Message)
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<ObjectResult> GetExam(int id)
        {
            try
            {
                var ExamPaper = await ExamPaperRepository.GetById(id);
                var data = mapper.Map<ExamResponseModel>(ExamPaper);
                int statusCode;
                if (data == null) statusCode = 404;
                else statusCode = 200;

                var examSchedule = _context.ExamSchedules.FirstOrDefault(x => x.ExamScheduleId == data.ExamScheduleId);
                data.SubjectName = _context.AvailableSubjects.FirstOrDefault(x => x.AvailableSubjectId == examSchedule.AvailableSubjectId).SubjectName;
                var typeId = _context.ExamSchedules.Find(data.ExamScheduleId).TypeId;
                data.Type = _context.Types.Find(typeId).TypeName;
                var register = _context.RegisterSubjects.Find(examSchedule.RegisterSubjectId);
                data.LecturerName = _context.Users.Find(register.UserId).FullName;
                return new ObjectResult(data)
                {
                    StatusCode = statusCode,
                };
            }
            catch (Exception exc)
            {
                return new ObjectResult(exc)
                {
                    StatusCode = 500,
                };
            }
        }

        public async Task<ObjectResult> UpdateExam(int id, ExamUpdateRequestModel examUpdateModel)
        {
            try
            {
                var ExamPaper =await ExamPaperRepository.GetById(id);
                ExamPaper = mapper.Map(examUpdateModel,ExamPaper);
                ExamPaper.ExamPaperId = id;
                await ExamPaperRepository.Update(ExamPaper);
                return new ObjectResult(ExamPaper)
                {
                    StatusCode = 200
                };
            }
            catch (Exception exc)
            {
                return new ObjectResult(exc)
                {
                    StatusCode = 500
                };
            }
        }
        
        public async Task<ObjectResult> ApproveExam(CommentModel commentModel, ExamUpdateApproveModel examUpdateModel)
        {
            try
            {
                var examPaper = await ExamPaperRepository.GetById(commentModel.ExamPaperId);
                var senderName = _context.Users.Find(commentModel.ApprovalUserId).FullName;
                var examSchedule = _context.ExamSchedules.Find(examPaper.ExamScheduleId);
                var registerSubject = _context.RegisterSubjects.Find(examSchedule.RegisterSubjectId);
                var availableSubject = _context.AvailableSubjects.Find(examSchedule.AvailableSubjectId);
                if (examUpdateModel.Status == "Reject")
                { 
                    examPaper.Status = ExamPaperStatus.REJECTED;
                    var comment = mapper.Map<Comment>(commentModel);
                    comment.ApprovalUserName = senderName;
                    await CommentRepository.Create(comment);
                    // notificatiion when Reject Exam
                    var notification = new Notification();
                    notification.Type = "Reject";                  
                    notification.UserId = registerSubject.UserId;
                    notification.Message = "Your exam has been rejected";                  
                    notification.Sender = senderName;
                    notification.SubjectCode = _context.Subjects.Find(availableSubject.SubjectId).SubjectCode;
                    notification.Status = "Unread";
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }
                
                if (examUpdateModel.Status == "Approve")
                {
                    if (examPaper.ExamSchedule.TypeId == 1) 
                    { 
                        examPaper.Status = ExamPaperStatus.APPROVED;
                        var notification = new Notification();
                        notification.Type = "Appprove";
                        notification.UserId = registerSubject.UserId;
                        notification.Message = "Your exam has been approved";
                        notification.Sender = senderName;
                        notification.SubjectCode = _context.Subjects.Find(availableSubject.SubjectId).SubjectCode;
                        notification.Status = "Unread";
                        _context.Notifications.Add(notification);
                        await _context.SaveChangesAsync();
                    }
                    if(examPaper.ExamSchedule.TypeId == 2)
                    {
                        if(examPaper.Status == ExamPaperStatus.PENDING)
                        {
                            examPaper.Status = ExamPaperStatus.APPROVED_MANUAL;
                            var notification = new Notification();
                            notification.Type = "Instruction";
                            notification.UserId = registerSubject.UserId;
                            notification.Message = "Your exam has been submitted instruction for " + _context.AvailableSubjects.Find(registerSubject.AvailableSubjectId).SubjectName;
                            notification.Sender = senderName;
                            notification.SubjectCode = _context.Subjects.Find(availableSubject.SubjectId).SubjectCode;
                            notification.Status = "Unread";
                            _context.Notifications.Add(notification);
                            await _context.SaveChangesAsync();
                        }
                        else if(examPaper.Status == ExamPaperStatus.APPROVED_MANUAL)
                        {
                            examPaper.Status = ExamPaperStatus.APPROVED;
                            var notification = new Notification();
                            notification.Type = "Appprove";
                            notification.UserId = registerSubject.UserId;
                            notification.Message = "Your exam has been approved";
                            notification.Sender = senderName;
                            notification.SubjectCode = _context.Subjects.Find(availableSubject.SubjectId).SubjectCode;
                            notification.Status = "Unread";
                            _context.Notifications.Add(notification);
                            await _context.SaveChangesAsync();
                        }
                    }
                    
                }


                await ExamPaperRepository.Update(examPaper);

                return new ObjectResult(examPaper)
                {
                    StatusCode = 200
                };
            }
            catch (Exception exc)
            {
                return new ObjectResult(exc)
                {
                    StatusCode = 500
                };
            }
        }
        public async Task<ObjectResult> SendInstructionLink(int id,ExamUpdateInstructionLinkModel exam)
        {
            try
            {
                
                var examPaper = await ExamPaperRepository.GetById(id);
                if (examPaper.Status != ExamPaperStatus.APPROVED_MANUAL)
                {
                    return new ObjectResult("Not allowed")
                    {
                        StatusCode = 500
                    };
                }
                examPaper.ExamInstruction = exam.ExamInstruction;
                await  ExamPaperRepository.Update(examPaper);
                return new ObjectResult(examPaper)
                {
                    StatusCode = 200
                };
            }
            catch (Exception exc)
            {
                return new ObjectResult(exc)
                {
                    StatusCode = 500
                };
            }
        }
        
        private async Task<List<ExamResponseModel>> mapToExamResponse(List<ExamPaper> ExamPapers)
        {
            List<ExamResponseModel> datas = ExamPapers.Select(x => mapper.Map<ExamResponseModel>(x)).ToList();
            foreach (var data in datas)
            {
                var examSchedule = await ExamScheduleRepository.GetExamScheduleAsync(data.ExamScheduleId);
                var ASubject = await _availableSubjectRepository.GetAvailableSubjectById(examSchedule.AvailableSubjectId);
                data.SubjectName = ASubject.SubjectName;
                data.Tittle = examSchedule.Tittle;

                var subject = await _subjectRepository.getSubject(ASubject.SubjectId); ;
                int typeId = subject.TypeId;

                var type = await _typeRepository.getTypeById(typeId);
                data.Type = type.TypeName;

                var register = await _registerSubjectRepository.GetRegisterSubjectById(examSchedule.RegisterSubjectId);
                var user = await _userRepository.GetUserAsync(register.UserId);
                data.LecturerName = user.FullName;

                if(data.Status == ExamPaperStatus.APPROVED_MANUAL)
                {
                    if(data.ExamInstruction == null)
                    {
                        data.Status = "Not-Submitted-Introduction";
                    }
                    else
                    {
                        data.Status = "Submitted-Introduct";
                    }
                    
                }

                var comment = ExamPapers.FirstOrDefault(x => x.ExamPaperId == data.ExamPaperId).Comments.FirstOrDefault();
                if (comment == null)
                {
                    data.Comment = "";
                }
                else
                {
                    data.Comment = comment.CommentContent.Trim();
                }
            }
            return datas;
        }

        public async Task<ObjectResult> getExamPaperPendingOrWaitingByAppovalUserId(int appovalUserId)
        {
            var ExamPapers = await ExamPaperRepository.GetExamPaperPendingOrWaitingByApprovalUserId(appovalUserId);

            if (ExamPapers != null)
            {
                List<ExamResponseModel> datas = await mapToExamResponse(ExamPapers);
                return new ObjectResult(datas)
                {
                    StatusCode = 200,
                };
            }
            return new ObjectResult(new List<object>())
            {
                StatusCode = 500
            };
        }
        public async Task<ObjectResult> getExamPaperApprovedByApprovalUserId(int approvalUserId)
        {
            var ExamPapers = await ExamPaperRepository.GetExamPaperWithExamScheduleByApprovalUserId(ExamPaperStatus.APPROVED, approvalUserId);
            if (ExamPapers != null)
            {
                List<ExamResponseModel> datas = await mapToExamResponse(ExamPapers);
                return new ObjectResult(datas)
                {
                    StatusCode = 200,
                };
            }
            return new ObjectResult(new List<object>())
            {
                StatusCode = 500
            };
        }

        
    }
}
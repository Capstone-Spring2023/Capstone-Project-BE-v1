using AutoMapper;
using Business.Constants;
using Business.ExamSchedule.interfaces;
using Business.ExamSchedule.Models;
using Business.NotificationService.Model;
using Data.Models;
using Data.Repositories.implement;
using Data.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ExamSchedule.Implements
{
    public class ExamScheduleService : IExamScheduleService
    {
        private readonly IExamScheduleRepository _examScheduleRepository;
        private readonly IRegisterSubjectRepository _registerSubjectRepository;
        private readonly IAvailableSubjectRepository _availableSubjectRepository;
        private readonly CFManagementContext _context;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public ExamScheduleService(IExamScheduleRepository examRepository, IRegisterSubjectRepository registerSubjectRepository, IAvailableSubjectRepository availableSubjectRepository, IMapper mapper, CFManagementContext context, INotificationRepository notificationRepository)
        {
            _examScheduleRepository = examRepository;
            _registerSubjectRepository = registerSubjectRepository;
            _availableSubjectRepository = availableSubjectRepository;   
            _context = context;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        public async Task<ResponseModel> GetAllExamScheduleByLeaderId(int leaderId)
        {
            var listAvailableSubject = await _availableSubjectRepository.GetAllAvailableSubjectsHaveExamScheduleByLeaderId(leaderId);
            if(listAvailableSubject == null)
            {
                return new()
                {
                    StatusCode = 404,
                };
            }
            var listGroupExamSchedule = new List<ResponseGroupExamSchedule>();

            foreach(var availableSubject in listAvailableSubject)
            {
                    var groupExamSchedule = new ResponseGroupExamSchedule();


                    var examSchedules = await _examScheduleRepository.getExamScheduleByAvailableSubjectId(availableSubject.AvailableSubjectId);
                    
                    if (examSchedules != null)
                    {
                    var examSchedule = examSchedules.ElementAt(0);
                    groupExamSchedule.SubjectName = availableSubject.SubjectName;
                    groupExamSchedule.AvailableSubjectId = examSchedule.AvailableSubjectId;
                    groupExamSchedule.LeaderId = examSchedule.LeaderId;
                    groupExamSchedule.Deadline = examSchedule.Deadline;
                    groupExamSchedule.ExamLink = examSchedule.ExamLink;
                    groupExamSchedule.Tittle = examSchedule.Tittle;
                    listGroupExamSchedule.Add(groupExamSchedule);
                    }
                
            }
            if(!listGroupExamSchedule.Any())
            {
                return new()
                {
                    StatusCode = (int)Business.Constants.StatusCode.NOTFOUND
                };
            }
            return new()
            {
                StatusCode = 200,
                Data = listGroupExamSchedule
            };
        }
        public async Task<ResponseModel> CreateExamSchedule(CreateExamScheduleModel createExamScheduleModel, int availableSubjectId)
        {
            var listRegisterSubject = await _registerSubjectRepository.SearchBySubjectId(availableSubjectId);
            if (!listRegisterSubject.Any())
            {
                return new()
                {
                    StatusCode = (int)StatusCode.BADREQUEST
                };
            }
            var availableSubject = await _availableSubjectRepository.GetAvailableSubjectById(availableSubjectId);
            foreach (var registerSubject in listRegisterSubject)
            {

                if (registerSubject.Status)
                {
                    //check Exist Request
                    var checkExistReqest = await _examScheduleRepository.GetExamScheduleByRegisterSubjectId(registerSubject.RegisterSubjectId);
                    if (checkExistReqest != null && checkExistReqest.Status)
                    {
                        return new()
                        {
                            StatusCode = (int)Business.Constants.StatusCode.BADREQUEST                           
                        };
                    }
                    if (registerSubject.UserId != createExamScheduleModel.LeaderId && registerSubject.UserId != createExamScheduleModel.AppovalUserId)
                    {

                        //Create Request
                        var examScheduleModel = _mapper.Map<Data.Models.ExamSchedule>(createExamScheduleModel);
                    int typeId = _context.Subjects.Find(availableSubject.SubjectId).TypeId;

                    examScheduleModel.TypeId = typeId;
                    examScheduleModel.RegisterSubjectId = registerSubject.RegisterSubjectId;
                    var Subject = _context.Subjects.Where(x => x.SubjectId == availableSubject.SubjectId && x.Status).FirstOrDefault();
                    examScheduleModel.TypeId= Subject.TypeId;
                    examScheduleModel.AvailableSubjectId = availableSubject.AvailableSubjectId;
                    examScheduleModel.Status = true;

                    await _examScheduleRepository.CreateScheduleExam(examScheduleModel);
                    var notification = _mapper.Map<Notification>(createExamScheduleModel);
                    
                        notification.UserId = registerSubject.UserId;
                        notification.Sender = _context.Users.Find(createExamScheduleModel.LeaderId).FullName;
                        notification.SubjectCode = Subject.SubjectCode;
                        notification.Status = "Unread";
                        await _notificationRepository.CreateNotification(notification);
                    }
                    
                }              
            }
            
            return new()
            {
                StatusCode = (int)StatusCode.OK,
                Data = "Created"
            };
        }
        
        public async Task<ResponseModel> GetExamSchedule(int id)
        {
            var examSchedule = await _examScheduleRepository.GetExamScheduleAsync(id);
            if (examSchedule == null)
            {
                return new()
                {
                    StatusCode = (int) StatusCode.NOTFOUND
                };
            }
            return new()
            {
                StatusCode = (int)StatusCode.OK,
                Data = examSchedule
            };
        }

        public async Task<ObjectResult> UpdateExamSchedule(UpdateExamScheduleModel updateExamScheduleModel,int availableSubjectId)
        {
            var registers = _context.RegisterSubjects
                .Where(x => x.AvailableSubjectId == availableSubjectId && x.Status).ToList();
            List<Data.Models.ExamSchedule> examSchedules = new List<Data.Models.ExamSchedule>();
            foreach (var a in registers)
            {
                var examSchedule = await _examScheduleRepository.GetExamScheduleByRegisterSubjectId(a.RegisterSubjectId);
                if (examSchedule != null)
                {
                    examSchedule = _mapper.Map( updateExamScheduleModel, examSchedule);
                    examSchedules.Add(examSchedule);
                }
            }
            try
            {
                await _examScheduleRepository.UpdateExamSchedule(examSchedules);
                var res = new ObjectResult(examSchedules.Select( x =>  _mapper.Map<ResponseExamSchedule>(x)))
                {
                    StatusCode = 200,
                };
                return res;
            }catch (Exception ex)
            {
                return new ObjectResult(ex)
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<ObjectResult> DeleteExamSchedule(int availableSubjectId)
        {
            var listExamSchedule = await _examScheduleRepository.getExamScheduleByAvailableSubjectId(availableSubjectId);
            
            if (listExamSchedule != null)
            {
                foreach(var examSchedule in listExamSchedule)
                {
                    examSchedule.Status = false;
                    await _examScheduleRepository.DeleteExamSchedule(examSchedule.ExamScheduleId);
                }
            }
            return new ObjectResult("DELETED")
            {
                StatusCode = 200,
            };
        }

        public async Task<ResponseModel> GetDetailRequestExamSchedule(int availableSubjectId)
        {            
                var groupExamSchedule = new ResponseGroupExamSchedule();
                var examSchedules = await _examScheduleRepository.getExamScheduleByAvailableSubjectId(availableSubjectId);
                if (examSchedules != null)
                {
                    var examSchedule = examSchedules.ElementAt(0);
                    groupExamSchedule.AvailableSubjectId = examSchedule.AvailableSubjectId;
                    var availableSubject = await _availableSubjectRepository.GetAvailableSubjectById(availableSubjectId);
                    groupExamSchedule.SubjectName = availableSubject.SubjectName;
                    groupExamSchedule.LeaderId = examSchedule.LeaderId;
                    groupExamSchedule.Deadline = examSchedule.Deadline;
                    groupExamSchedule.ExamLink = examSchedule.ExamLink;
                    groupExamSchedule.Tittle = examSchedule.Tittle;
                    return new()
                    {
                        StatusCode = 200,
                        Data = groupExamSchedule
                    };
                }
            return new()
            {
                StatusCode = 404,
            };
        }
    }
}

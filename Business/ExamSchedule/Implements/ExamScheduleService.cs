﻿using AutoMapper;
using Business.Constants;
using Business.ExamSchedule.interfaces;
using Business.ExamSchedule.Models;
using Data.Models;
using Data.Repositories.implement;
using Data.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;

        public ExamScheduleService(IExamScheduleRepository examRepository, IRegisterSubjectRepository registerSubjectRepository, IAvailableSubjectRepository availableSubjectRepository, IMapper mapper, CFManagementContext context)
        {
            _examScheduleRepository = examRepository;
            _registerSubjectRepository = registerSubjectRepository;
            _availableSubjectRepository = availableSubjectRepository;   
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel> GetAllExamScheduleByLeaderId(int leaderId)
        {
            var listExamSchedule = await _examScheduleRepository.GetAllExamScheduleByLeaderId(leaderId);
            
            return new()
            {
                StatusCode = 200,
                Data = listExamSchedule
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
            var listResponseExamSchedule = new List<ResponseExamSchedule>();
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
                    

                    //Create Request
                    var examScheduleModel = _mapper.Map<Data.Models.ExamSchedule>(createExamScheduleModel);

                    examScheduleModel.RegisterSubjectId = registerSubject.RegisterSubjectId;

                    examScheduleModel.LeaderId = availableSubject.LeaderId;
                    examScheduleModel.SubjectId = availableSubject.SubjectId;

                    examScheduleModel.Status = true;

                    await _examScheduleRepository.CreateScheduleExam(examScheduleModel);
                    //respone
                    var response = new ResponseExamSchedule();
                    response.examScheduleId = examScheduleModel.ExamScheduleId;
                    response.SubjectId = availableSubject.SubjectId;
                    response.Deadline = examScheduleModel.Deadline;
                    response.ExamLink = examScheduleModel.ExamLink;
                    response.Tittle = examScheduleModel.Tittle;
                    response.TypeId = examScheduleModel.TypeId;
                    response.Status= examScheduleModel.Status;
                    listResponseExamSchedule.Add(response);
                }
                
            }
            return new()
            {
                StatusCode = (int)StatusCode.OK,
                Data = listResponseExamSchedule
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

        public async Task<ObjectResult> DeleteExamSchedule(int id)
        {
            await _examScheduleRepository.DeleteExamSchedule(id);
            return new ObjectResult("DELETED")
            {
                StatusCode = 200,
            };
        }
    }
}

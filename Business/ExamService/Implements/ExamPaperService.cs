﻿using System;
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

namespace Business.ExamPaperService.Implements
{
    public class ExamPaperService : IExamPaperService
    {
        private readonly IExamPaperRepository ExamPaperRepository;
        private readonly ICommentRepository CommentRepository;
        private readonly IExamScheduleRepository ExamScheduleRepository;
        private readonly CFManagementContext _context;
        private IMapper mapper;
        public ExamPaperService(CFManagementContext context, IExamPaperRepository ExamPaperRepository, ICommentRepository commentRepository, IMapper mapper, IExamScheduleRepository examScheduleRepository)
        {
            this.ExamPaperRepository = ExamPaperRepository;
            this.mapper = mapper;
            this.CommentRepository = commentRepository;
            _context = context;
            ExamScheduleRepository = examScheduleRepository;
        }

        public async Task<ObjectResult> CreateExam(ExamCreateRequestModel ExamPaperCreateRequest)
        {
            var ExamPaper = mapper.Map<ExamPaper>(ExamPaperCreateRequest);
            ExamPaper.Status = ExamPaperStatus.PENDING;
            try
            {
                await ExamPaperRepository.CreateExam(ExamPaper);
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
                    data.SubjectName = _context.Subjects.FirstOrDefault(x => x.SubjectId == examSchedule.SubjectId).SubjectName;
                    
                    var register = _context.RegisterSubjects.Find(examSchedule.RegisterSubjectId);
                    data.LecturerName = _context.Users.Find(register.UserId).FullName;
                    
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
                data.SubjectName = _context.Subjects.FirstOrDefault(x => x.SubjectId == examSchedule.SubjectId).SubjectName;

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
                var ExamPaper = mapper.Map<ExamPaper>(examUpdateModel);
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
                if (!examUpdateModel.IsApproved)
                {
                    var comment = mapper.Map<Comment>(commentModel);
                    await CommentRepository.Create(comment);
                }
                var examPaper = await ExamPaperRepository.GetById(commentModel.ExamPaperId);
                if (examUpdateModel.IsApproved)
                {
                    if (examPaper.ExamSchedule.TypeId == 1) examPaper.Status = ExamPaperStatus.APPROVED;
                    else examPaper.Status = ExamPaperStatus.APPROVED_MANUAL;
                }
                else
                {
                    examPaper.Status = ExamPaperStatus.REJECTED;
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

        public async Task<ObjectResult> ViewExamSubmissionByLeaderId(int leaderId)
        {
            var listExamSchedule = await ExamScheduleRepository.GetAllExamScheduleByLeaderId(leaderId);
            
            var listExamSubmission = new List<ExamPaper>();
            foreach(var examSchedule in listExamSchedule)
            {
                var examSubmissions = await ExamPaperRepository.GetAllByExamScheduleId(examSchedule.ExamScheduleId);
                if(examSubmissions == null)
                {
                    return new(examSubmissions)
                    {
                        StatusCode = (int)Business.Constants.StatusCode.NOTFOUND
                    };
                }
                foreach(var exam in examSubmissions)
                {
                    if(exam.Status == "Pending")
                    {
                        listExamSubmission.Add(exam);
                        break;
                    }
                }
            }
            return new ObjectResult(listExamSubmission) 
            { 
                StatusCode = 200 
            };
        }
    }
}
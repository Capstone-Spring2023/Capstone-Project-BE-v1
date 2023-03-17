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

namespace Business.ExamPaperService.Implements
{
    public class ExamPaperService : IExamPaperService
    {
        private readonly IExamPaperRepository ExamPaperRepository;
        private readonly ICommentRepository CommentRepository;
        private IMapper mapper;
        public ExamPaperService(IExamPaperRepository ExamPaperRepository, ICommentRepository commentRepository, IMapper mapper)
        {
            this.ExamPaperRepository = ExamPaperRepository;
            this.mapper = mapper;
            this.CommentRepository = commentRepository;
        }

        public async Task<ObjectResult> CreateExam(ExamCreateRequestModel ExamPaperCreateRequest)
        {
            var ExamPaper = mapper.Map<ExamPaper>(ExamPaperCreateRequest);
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
                await ExamPaperRepository.UpdateExam(ExamPaper);
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
                examPaper.IsApproved = examUpdateModel.IsApproved; 
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

        
    }
}
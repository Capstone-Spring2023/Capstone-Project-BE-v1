using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Business.ExamService.Models;
using Data.Models;
using Data.Paging;
using Microsoft.AspNetCore.Mvc;

namespace Business.ExamPaperService.Interfaces
{
    public interface IExamPaperService
    {
        public Task<ObjectResult> GetExam(int id);
        public Task<ObjectResult> CreateExam( int examScheduleId,ExamCreateRequestModel exam);

        public Task<ObjectResult> UpdateExam(int id, ExamUpdateRequestModel examUpdateModel);
        public Task<ObjectResult> GetAllExams(Expression<Func<ExamPaper, bool>> ex,PagingRequest paging);
        public Task<ObjectResult> DeleteExam(int id);
        public Task<ObjectResult> ApproveExam(CommentModel commentModel, ExamUpdateApproveModel examUpdateRequestModel);
        public Task<ObjectResult> SendInstructionLink(int id, ExamUpdateInstructionLinkModel exam);
        public Task<ObjectResult> ViewExamSubmissionByLeaderId(int leaderId);
    }
}

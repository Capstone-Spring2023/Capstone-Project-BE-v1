using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Business.ExamService.Models;
using Business.AvailableSubjectService.Models;
using Business.ExamSchedule.Models;

namespace Business
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExamCreateRequestModel, ExamPaper>();
            CreateMap<ExamUpdateRequestModel, ExamPaper>();
            CreateMap<ExamUpdateApproveModel, ExamPaper>().ReverseMap();
            CreateMap<ExamPaper, ExamResponseModel>();
            CreateMap<AvailableSubject, AvailableSubjectResponse>();
            CreateMap<User, TeacherResponse>();
            CreateMap<CreateExamScheduleModel, Data.Models.ExamSchedule>().ReverseMap();
            CreateMap<Comment, CommentModel>().ReverseMap();
        }
    }
}

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
using Business.NotificationService.Model;
using Business.UserService.Models;
using Business.RegisterSubjectService.Models;
namespace Business
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterSlot, RegisterSlotResponse>();
            CreateMap<RegisterSubject, RegisterSubjectResponse>()
                .ForMember(src => src.SubjectName, act => act.MapFrom(des => des.AvailableSubject.SubjectName));
            CreateMap<ExamCreateRequestModel, ExamPaper>();
            CreateMap<ExamUpdateRequestModel, ExamPaper>();
            CreateMap<ExamUpdateApproveModel, ExamPaper>().ReverseMap();
            CreateMap<ExamPaper, ExamResponseModel>();
            CreateMap<AvailableSubject, AvailableSubjectResponse>();
            CreateMap<User, TeacherResponse>();
            CreateMap<CreateExamScheduleModel, Data.Models.ExamSchedule>().ReverseMap();
            CreateMap<Comment, CommentModel>().ReverseMap();
            CreateMap<UpdateExamScheduleModel, Data.Models.ExamSchedule>().ReverseMap();
            CreateMap<Data.Models.ExamSchedule, ResponseExamSchedule>();
            CreateMap<Data.Models.ExamSchedule, ResponseGroupExamSchedule>().ReverseMap();
            CreateMap<CreateNotificationModel, Notification>().ReverseMap();
            CreateMap<ReponseNotificationModel, Notification>().ReverseMap();
            CreateMap<ResponseDepartment, Department>().ReverseMap();
            CreateMap<CreateRegisterSubjectModel, RegisterSubject>().ReverseMap();
            CreateMap<CreateExamScheduleModel, Notification>().ReverseMap();
        }
    }
}

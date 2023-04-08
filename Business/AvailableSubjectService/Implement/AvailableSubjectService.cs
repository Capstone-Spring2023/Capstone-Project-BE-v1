
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Interface;
using Data.Models;
using Data.Paging;
using Business.AvailableSubjectService.Models;
using AutoMapper;
using Business.AvailableSubjectService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Business.AvailableSubjectService.Implement
{
    public class AvailableSubjectService: IAvailableSubjectService
    {
        private readonly IAvailableSubjectRepository availableSubjectRepository;
        private readonly IRegisterSubjectRepository registerSubjectRepository;
        private readonly IMapper mapper;
        public AvailableSubjectService(IAvailableSubjectRepository availableSubjectRepository, IRegisterSubjectRepository registerSubjectRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this.registerSubjectRepository = registerSubjectRepository;
            this.availableSubjectRepository = availableSubjectRepository;
        }
        public async Task<ObjectResult> GetAvailableSubjects(Expression<Func<AvailableSubject, bool>> ex, PagingRequest paging)
        {
            try
            {
                var datas = await availableSubjectRepository.GetAvailableSubjects(ex, paging);
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
        public async Task<ObjectResult> GetTeachersBySubjectId(int subjectId, PagingRequest paging)
        {
            try
            {
                var teachers = await registerSubjectRepository.SearchTeachersBySubjectId(subjectId , paging);
                var datas = teachers.Select(x => mapper.Map<TeacherResponse>(x)).ToList();
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
        public async Task<ResponseModel> GetAllAvailableSubjectByLeaderId(int leaderId)
        {
            var listAvailableSubject = await availableSubjectRepository.GetAllAvailableSubjectsByLeaderId(leaderId);
            if (!listAvailableSubject.Any())
            {
                return new()
                {
                    StatusCode = (int)Business.Constants.StatusCode.NOTFOUND,
                };
            }
            return new()
            {
                StatusCode = (int)Business.Constants.StatusCode.OK,
                Data = listAvailableSubject
            };
        }

        public async Task<ResponseModel> GetAvailableSubjectById(int id)
        {
            var availableSubject = await availableSubjectRepository.GetAvailableSubjectById(id);
            if(availableSubject == null)
            {
                return new()
                {
                    StatusCode = 404
                };
            }
            return new()
            {
                StatusCode = 200,
                Data = availableSubject
            };
        }

        public async Task<ResponseModel> GetAvailableSubjectByDepartmentId(int departmentId)
        {
            var listSubjects = await availableSubjectRepository.GetAvailableSubjectsByDepartmentId(departmentId);
            return new()
            {
                StatusCode = 200,
                Data = listSubjects
            };
        }

        public Task<ResponseModel> GetAllAvailableSubjectNotHaveRegister()
        {


            throw new NotImplementedException();
        }
    }
}

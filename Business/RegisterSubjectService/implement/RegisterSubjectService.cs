using AutoMapper;
using Business.RegisterSubjectService.Interfaces;
using Business.RegisterSubjectService.Models;
using Data.Models;
using Data.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.RegisterSubjectService.implement
{
    public class RegisterSubjectService : IRegisterSubjectService
    {
        private readonly IRegisterSubjectRepository _registerSubjectRepository;
        private readonly CFManagementContext _context;
        private readonly IMapper _mapper;
        public RegisterSubjectService(IRegisterSubjectRepository registerSubjectRepository, IMapper mapper, CFManagementContext context)
        {
            _registerSubjectRepository = registerSubjectRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ResponseModel> CreateRegisterSubject(CreateRegisterSubjectModel model)
        {
            try
            {
                var registerSubject = _mapper.Map<RegisterSubject>(model);
                registerSubject.ClassId = 1;
                registerSubject.RegisterDate = DateTime.Now;
                registerSubject.Status = false;
                await _registerSubjectRepository.CreateRegisterSubject(registerSubject);
                var registerSlot = new RegisterSlot();
                registerSlot.Slot = model.Slot;
                registerSlot.Status = false;
                registerSlot.UserId = model.UserId;
                registerSlot.SemesterId = _context.AvailableSubjects.Where(x => x.AvailableSubjectId == model.AvailableSubjectId && x.Status).FirstOrDefault().SemesterId;
                _context.RegisterSlots.Add(registerSlot);
                await _context.SaveChangesAsync();
            } catch (Exception ex)
            {
                return new()
                {
                    StatusCode = 500,
                };
            }
            return new()
            {
                StatusCode = 201,
                Data = "Created"
            };
        }
    }
}

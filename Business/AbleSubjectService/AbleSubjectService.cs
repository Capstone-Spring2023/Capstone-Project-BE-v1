using Business.AbleSubjectService.Interface;
using Business.AbleSubjectService.Models;
using Data.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AbleSubjectService
{
    public class AbleSubjectService : IAbleSubjectService
    {
        private readonly IUserRepository userRepository;
        private readonly ISubjectRepository subjectRepository;
        public AbleSubjectService(IUserRepository userRepository, ISubjectRepository subjectRepository)
        {
            this.userRepository = userRepository;
            this.subjectRepository = subjectRepository;
        }
        public async Task<ObjectResult> getAbleSubjectsByUserId(int userId)
        {
            var user =await userRepository.GetUserWithSubjectsById(userId);
            var res = user.Subjects.Select(x => new AbleSubjectResponse()
            {
                subjectId = x.SubjectId,
                subjectName = x.SubjectName,
            });
            return new ObjectResult(res);
        }
        public async Task<ObjectResult> getAbleSubjects()
        {
            var subjects =await subjectRepository.GetAll();
            var res = subjects.Select(_x => new AbleSubjectResponse()
            {
                subjectId = _x.SubjectId,
                subjectName = _x.SubjectName,
            });
            return new ObjectResult(res);
        }
        public async Task<ObjectResult> createAbleSubject(AbleSubjectRequest request)
        {
            var user = await userRepository.GetUserWithSubjectsById(request.userId);
            if (!user.Subjects.Select(x => x.SubjectId).Contains(request.subjectId))
            {
                var subject = await subjectRepository.getSubject(request.subjectId);
                user.Subjects.Add(subject);
                userRepository.UpdateUserAsync(0, user);
                return new ObjectResult("Created")
                {
                    StatusCode = 201
                };
            }
            else
            {
                return new ObjectResult("Lecturer already registered this subject")
                {
                    StatusCode = 400
                };
            }
        }
        public async Task<ObjectResult> deleteAbleSubject(AbleSubjectRequest request)
        {
            var user = await userRepository.GetUserWithSubjectsById(request.userId);
            if (user.Subjects.Select(x => x.SubjectId).Contains(request.subjectId))
            {
                var subject = user.Subjects.First(x=> x.SubjectId == request.subjectId);
                user.Subjects.Remove(subject);
                userRepository.UpdateUserAsync(0, user);
                return new ObjectResult("Deleted")
                {
                    StatusCode = 200
                };
            }
            else
            {
                return new ObjectResult("Lecturer does not have this subject")
                {
                    StatusCode = 400
                };
            }
        }
    }
}

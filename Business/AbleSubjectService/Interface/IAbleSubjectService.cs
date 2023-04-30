using Business.AbleSubjectService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AbleSubjectService.Interface
{
    public interface IAbleSubjectService
    {
        Task<ObjectResult> getAbleSubjectsByUserId(int userId);
        Task<ObjectResult> getAbleSubjects();
        Task<ObjectResult> createAbleSubject(AbleSubjectRequest request);
        Task<ObjectResult> deleteAbleSubject(AbleSubjectRequest request);
    }
}

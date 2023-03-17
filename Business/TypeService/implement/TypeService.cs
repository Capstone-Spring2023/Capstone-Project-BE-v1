using Business.TypeService.@interface;
using Data.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.TypeService.implement
{
    public class TypeService : ITypeService
    {
        private readonly ITypeRepository _typeRepository;
        public TypeService(ITypeRepository typeRepository)
        {
            _typeRepository = typeRepository;
        }

        public async Task<ResponseModel> GetAllType()
        {
            var listType = await _typeRepository.getAllType();
            if(listType == null)
            {
                return new()
                {
                    StatusCode = (int)Business.Constants.StatusCode.NOTFOUND
                };
            }
            return new()
            {
                StatusCode = (int)Business.Constants.StatusCode.OK,
                Data = listType
            };
        }
    }
}

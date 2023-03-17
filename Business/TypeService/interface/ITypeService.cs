using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.TypeService.@interface
{
    public interface ITypeService
    {
        public Task<ResponseModel> GetAllType();
    }
}

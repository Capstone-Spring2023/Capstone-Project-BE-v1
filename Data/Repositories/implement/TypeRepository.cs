using Data.Models;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.implement
{
    public class TypeRepository : ITypeRepository
    {
        private readonly CFManagementContext _context;
        public TypeRepository(CFManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Data.Models.Type>> getAllType()
        {
            var listType = await _context.Types.ToListAsync();
            return listType;
        }
    }
}

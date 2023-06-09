﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface ITypeRepository
    {
        public Task<List<Data.Models.Type>> getAllType();
        public Task<Data.Models.Type> getTypeById(int id);
    }
}

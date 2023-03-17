using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interface
{
    public interface ICommentRepository
    {
        public Task<Comment> GetById(int id);
        public Task Create(Comment comment);

    }
}

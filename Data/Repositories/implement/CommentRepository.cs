﻿using Data.Models;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.implement
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CFManagementContext _context;
        public CommentRepository(CFManagementContext context)
        {
            _context = context;
        }

        public async Task Create(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Comment> GetByExamPaperId(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(x => x.ExamPaperId == id);
        }
    }
}

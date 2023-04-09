using AutoMapper;
using Data.Models;
using Data.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Data.Repositories.implement
{
    public class UserRepository : IUserRepository
    {
        private readonly CFManagementContext _context;
        public UserRepository(CFManagementContext context) 
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            var listUser = await _context.Users.ToListAsync();
            return listUser;
        }

        public async Task<User> GetUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task UpdateUserAsync(int id, User user)
        {        
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            return user;
        }
        
    }
}

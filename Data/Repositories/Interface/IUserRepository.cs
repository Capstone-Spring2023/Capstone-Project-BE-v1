using Data.Models;
using System;
using System.Collections.Generic;

namespace Data.Repositories.Interface
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllAsync();
        public  Task<User> GetUserAsync(int id);
        public Task UpdateUserAsync(int id, User user);
        public Task<User> GetUserByEmail(string email);

    }
}
 
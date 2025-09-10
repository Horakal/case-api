using Microsoft.EntityFrameworkCore;
using RestApiCase.Domain.User.Interface;
using RestApiCase.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskDbContext _context;

        public UserRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<Guid?> Authenticate(string name, string password)
        {
            var user = await _context.Users.Where(user => user.UserName == name && user.Password == password).FirstOrDefaultAsync();

            return user?.Id;
        }
    }
}
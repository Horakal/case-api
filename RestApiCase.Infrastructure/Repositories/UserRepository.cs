using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Entities;
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
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid?> Authenticate(string name, string password)
        {
            var user = await _context.Users.Where(user => user.Username == name).FirstOrDefaultAsync();
            if (user is not null && BCrypt.Net.BCrypt.Verify(password, user.Password)) { return user.Id; }

            return null;
        }

        public async Task DeleteToken(RefreshToken token)
        {
            _context.Remove(token);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Role>> GetByIdWithRolesAsync(Guid userId)
        {
            return await _context.Roles
                .Where(u => u.UserId == userId).ToListAsync();
        }

        public async Task<RefreshToken?> GetToken(Guid userId)
        {
            var refreshTokenEntry = await _context.RefreshTokens
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            return refreshTokenEntry;
        }

        public async Task<User?> GetUser(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task SaveToken(RefreshToken refreshToken)
        {
            await _context.RefreshTokens
                .Where(r => r.UserId == refreshToken.UserId)
                .ExecuteDeleteAsync();
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
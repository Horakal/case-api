using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.User.Interface
{
    public interface IUserService
    {
        Task<Guid?> Authenticate(string name, string password);

        Task<User.Entities.User?> GetById(Guid id);

        ClaimsPrincipal? GetCurrentUser(string token);

        Task<List<Role>> GetRoles(Guid userId);

        Task CreateToken(RefreshToken token);

        Task<bool> ValidateToken(string token, Guid userId);

        Task DeleteToken(Guid userId);

        Task<RefreshToken?> GetRefreshToken(Guid userId);
    }
}
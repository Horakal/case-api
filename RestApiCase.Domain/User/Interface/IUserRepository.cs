using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.User.Interface
{
    public interface IUserRepository
    {
        Task<Guid?> Authenticate(string name, string password);

        Task<User.Entities.User?> GetUser(Guid id);

        Task<List<Role>> GetByIdWithRolesAsync(Guid userId);

        Task SaveToken(RefreshToken token);

        Task<RefreshToken?> GetToken(Guid userId);

        Task DeleteToken(RefreshToken token);
    }
}
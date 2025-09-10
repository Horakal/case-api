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

        ClaimsPrincipal? GetCurrentUser(string token);
    }
}
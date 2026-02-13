using RestApiCase.Domain.User.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.User.Interface
{
    public interface IUserRoles
    {
        Task<IEnumerable<UserRoles>> Authenticate(string name, string password);
    }
}
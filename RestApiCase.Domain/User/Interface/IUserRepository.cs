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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RestApiCase.Domain.User.Interface;

namespace RestApiCase.Application.User.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Guid?> Authenticate(string name, string password)
        {
            return await _userRepository.Authenticate(name, password);
        }

        public ClaimsPrincipal? GetCurrentUser(string token)
        {
            return Thread.CurrentPrincipal as ClaimsPrincipal;
        }
    }
}
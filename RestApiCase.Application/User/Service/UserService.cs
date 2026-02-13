using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RestApiCase.Domain.Commons;
using RestApiCase.Domain.User.Entities;
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

        public async Task CreateToken(RefreshToken token)
        {
            await _userRepository.SaveToken(token);
        }

        public ClaimsPrincipal? GetCurrentUser(string token)
        {
            return Thread.CurrentPrincipal as ClaimsPrincipal;
        }

        public async Task<RestApiCase.Domain.User.Entities.User?> GetById(Guid userId)
        {
            return await _userRepository.GetUser(userId);
        }

        public async Task<List<Role>> GetRoles(Guid id)
        {
            return await _userRepository.GetByIdWithRolesAsync(id);
        }

        public async Task<bool> ValidateToken(string token, Guid userId)
        {
            var refToken = await _userRepository.GetToken(userId);
            if (refToken is not null && refToken.IsExpired())
            {
                await _userRepository.DeleteToken(refToken);
            }
            else if (refToken is not null && !refToken.IsExpired())
            {
                if (string.Compare(refToken.Token, token) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task DeleteToken(Guid userId)
        {
            var refToken = await _userRepository.GetToken(userId);
            if (refToken is not null)
            {
                await _userRepository.DeleteToken(refToken);
            }
        }

        public async Task<RefreshToken?> GetRefreshToken(Guid userId)
        {
            return await _userRepository.GetToken(userId);
        }
    }
}
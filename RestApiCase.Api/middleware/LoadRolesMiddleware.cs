using RestApiCase.Application.User.Service;
using RestApiCase.Domain.User.Entities;
using RestApiCase.Domain.User.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RestApiCase.Api.middleware
{
    public class LoadRolesMiddleware
    {
        private readonly RequestDelegate _next;

        public LoadRolesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var userId = context.User?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userId != null)
            {
                Guid.TryParse(userId, out Guid guidId);
                var roles = await userService.GetRoles(guidId);
                var identity = context?.User?.Identity as ClaimsIdentity;
                foreach (var role in roles)
                {
                    identity?.AddClaim(new Claim(ClaimTypes.Role, role.RoleType.ToString()));
                }
            }
            await _next(context);
        }
    }
}
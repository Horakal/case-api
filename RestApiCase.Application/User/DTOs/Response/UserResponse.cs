using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Application.User.DTOs.Response
{
    public class UserResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
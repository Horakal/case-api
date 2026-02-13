using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Application.User.DTOs.Requests
{
    public class RefreshAccess
    {
        public string RefreshToken { get; set; }
        public string UserId { get; set; }
    }
}
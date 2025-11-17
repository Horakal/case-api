namespace RestApiCase.Application.User.DTOs.Requests
{
    public class UserLogin
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
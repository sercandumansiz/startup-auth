namespace Startup.Auth.Models.Responses
{
    public class LoginResponseModel
    {
        public UserResponseModel User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
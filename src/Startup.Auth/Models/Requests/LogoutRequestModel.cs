using System.ComponentModel.DataAnnotations;

namespace Startup.Auth.Models.Requests
{
    public class LogoutRequestModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
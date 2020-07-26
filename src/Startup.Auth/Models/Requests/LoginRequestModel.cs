using System.ComponentModel.DataAnnotations;

namespace Startup.Auth.Models.Requests
{
    public class LoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
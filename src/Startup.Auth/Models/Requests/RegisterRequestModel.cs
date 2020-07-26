using System.ComponentModel.DataAnnotations;

namespace Startup.Auth.Models.Requests
{
    public class RegisterRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
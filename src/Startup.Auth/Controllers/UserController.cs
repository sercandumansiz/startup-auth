using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Startup.Auth.Models.Requests;
using Startup.Auth.Services;

namespace Startup.Auth.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestModel request)
        {
            await _userService.Register(request.Email, request.Password);

            return Ok();
        }
    }
}
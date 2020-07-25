using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Startup.Auth.Models;
using Startup.Auth.Models.Requests;
using Startup.Auth.Models.Responses;
using Startup.Auth.Provider;
using Startup.Auth.Services;

namespace Startup.Auth.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtProvider _jwtProvider;
        public UserController(IUserService userService, IJwtProvider jwtProvider)
        {
            _userService = userService;
            _jwtProvider = jwtProvider;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
        {
            await _userService.Register(request.Email, request.Password);

            return Ok();
        }

        [HttpPost("token")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            Guid userId = await _userService.Login(request.Email, request.Password);

            string token = _jwtProvider.GenerateToken(userId);

            LoginResponseModel loginResponseModel = new LoginResponseModel();

            loginResponseModel.Token = token;

            return Ok(loginResponseModel);
        }
    }
}
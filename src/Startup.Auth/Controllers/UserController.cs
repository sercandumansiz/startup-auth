using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Startup.Auth.Models;
using Startup.Auth.Models.Requests;
using Startup.Auth.Models.Responses;
using Startup.Auth.Provider;
using Startup.Auth.Services;
using Startup.Auth.Utilities;

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
            // TODO : better model validation  
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || request.Password.Length < 6 || !RegexUtilities.IsValidEmail(request.Email))
            {
                return BadRequest(new { error = "Email and Password fields are required and Password must be minimum length of '6'" });
            }

            BaseModel<bool> result = await _userService.Register(request.Email, request.Password);

            if (result.HasError)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            if (result.Data)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("token")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            // TODO : better model validation  
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || request.Password.Length < 6 || !RegexUtilities.IsValidEmail(request.Email))
            {
                return BadRequest(new { error = "Email and Password fields are required and Password must be minimum length of '6'" });
            }

            BaseModel<UserModel> result = await _userService.Login(request.Email, request.Password);

            if (result.HasError)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            if (result.Data == null)
            {
                return Unauthorized();
            }

            LoginResponseModel loginResponseModel = new LoginResponseModel();

            loginResponseModel.Token = result.Data.Token;
            loginResponseModel.RefreshToken = result.Data.RefreshToken;

            loginResponseModel.User = new UserResponseModel()
            {
                Id = result.Data.Id,
                Email = result.Data.Email,
                CreatedAt = result.Data.CreatedAt,
                Type = result.Data.Type
            };

            return Ok(loginResponseModel);
        }

        [HttpGet("token/{refreshToken}/refresh")]
        public async Task<IActionResult> RefreshToken([FromRoute] string refreshToken)
        {
            // TODO : better model validation  
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { error = "RefreshToken is empty or null." });
            }

            BaseModel<UserModel> result = await _userService.RefreshAccessToken(refreshToken);

            if (result.HasError)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            if (result.Data == null)
            {
                return BadRequest();
            }

            LoginResponseModel loginResponseModel = new LoginResponseModel();

            loginResponseModel.Token = result.Data.Token;
            loginResponseModel.RefreshToken = result.Data.RefreshToken;

            loginResponseModel.User = new UserResponseModel()
            {
                Id = result.Data.Id,
                Email = result.Data.Email,
                CreatedAt = result.Data.CreatedAt,
                Type = result.Data.Type
            };

            return Ok(loginResponseModel);
        }

        [HttpPost("{id}/logout")]
        public async Task<IActionResult> Logout([FromRoute] Guid id, [FromBody] LogoutRequestModel request)
        {
            // TODO : better model validation  
            if (id == Guid.Empty || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest();
            }

            BaseModel<bool> result = await _userService.Logout(id, request.Token, request.RefreshToken);

            if (result.HasError || !result.Data)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok();
        }

        [HttpGet("introspect/{token}")]
        public async Task<IActionResult> Introspect([FromRoute] string token)
        {
            // TODO : better model validation  
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }

            BaseModel<bool> result = await _userService.Introspect(token);

            if (result.Data)
            {
                return Unauthorized();
            }

            return Ok();
        }
    }
}
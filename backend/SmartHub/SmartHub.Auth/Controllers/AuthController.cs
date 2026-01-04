using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHub.Auth.Interfaces;
using SmartHub.Auth.Models;
using SmartHub.Core.Interfaces;

namespace SmartHub.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    return StatusCode(404, "Email and password must be provided.");

                var user = await _userService.GetUserByEmail(request.Email);
                if (user == null)
                    return StatusCode(401, "Invalid email or password.");

                var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                if (!isPasswordValid)
                    return StatusCode(401, "Invalid email or password.");

                var token = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role);
                var loginReponse = new LoginResponse
                {
                    AccessToken = token,
                    ExpiresIn = 60 * 15 // 15 minutes
                };

                return StatusCode(200, loginReponse);
            }
            catch (Exception ex)
            {
                return StatusCode(404, "Invalid email or password.");
            }
        }
    }
}

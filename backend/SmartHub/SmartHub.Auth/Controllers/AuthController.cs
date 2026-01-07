using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHub.Auth.Interfaces;
using SmartHub.Auth.Models;
using SmartHub.Auth.Repositories;
using SmartHub.Core.CommonUtility;
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
                    return BadRequest("Email and password must be provided.");

                var user = await _userService.GetUserByEmail(request.Email);
                if (user == null)
                    return Unauthorized("Invalid email or password.");

                var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                if (!isPasswordValid)
                    return Unauthorized("Invalid email or password.");

                var token = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role);
                var loginReponse = new LoginResponse
                {
                    AccessToken = token,
                    ExpiresIn = 60 * 15 // 15 minutes
                };

                return Ok(loginReponse);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid email or password.");
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");
            
            if (!Utility.IsPasswordValid(request.Password))
                return BadRequest("Password must be at least 8 characters and contain letters and numbers.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            try
            {
                await _userService.RegisterUser(request.Email, passwordHash, "User");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email already exists"))
            {
                return Conflict("Email already exists");
            }

            return Created("","User registered successfully.");
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.FindFirstValue(ClaimTypes.Email),
                Mobile = User.FindFirstValue(ClaimTypes.MobilePhone),
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }

    }
}

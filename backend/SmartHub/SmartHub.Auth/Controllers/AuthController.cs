using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartHub.Auth.Data;
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
        private readonly IWebHostEnvironment _env;
        public AuthController(ITokenService tokenService, IUserService userService, IWebHostEnvironment env)
        {
            _tokenService = tokenService;
            _userService = userService;
            _env = env;
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

                var token = _tokenService.GenerateAccessToken(Convert.ToInt32(user.Id), user.Email, user.Role);

                Response.Cookies.Append(
                    "access_token",
                    token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = !_env.IsDevelopment(),
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                    }
                );

                return Ok(new { message = "Login successful" });
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

            try
            {
                await _userService.RegisterUser(
                    request.Email,
                    request.Password
                );

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Role = User.FindFirstValue(ClaimTypes.Role),
                Email = User.FindFirstValue(ClaimTypes.Email)
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return Ok(new { message = "Logged out" });
        }


    }
}

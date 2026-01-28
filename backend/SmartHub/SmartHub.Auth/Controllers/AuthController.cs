using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHub.Auth.Interfaces;
using SmartHub.Auth.Models;

namespace SmartHub.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;
        public AuthController(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var loginResponse = await _userService.LoginUser(request.Email, request.Password);
                var token = loginResponse.AccessToken;

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

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(
                "access_token",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !_env.IsDevelopment(),
                    SameSite = SameSiteMode.Strict
                }
            );
            return Ok(new { message = "Logged out successfully" });
        }


    }
}

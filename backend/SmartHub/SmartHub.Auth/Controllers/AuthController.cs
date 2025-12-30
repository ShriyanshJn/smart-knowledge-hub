using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHub.Auth.Models;
using SmartHub.Core.Interfaces;

namespace SmartHub.Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return StatusCode(404,"Email and password must be provided.");
            }

            // TODO:- Validate User Credentials from DB
            var userId = Guid.NewGuid();
            var role = "User";

            var token = _tokenService.GenerateAccessToken(userId, request.Email, role);
            var loginReponse = new LoginResponse
            {
                AccessToken = token,
                ExpiresIn = 60 * 15 // 15 minutes
            };

            return StatusCode(200,loginReponse);
        }
    }
}

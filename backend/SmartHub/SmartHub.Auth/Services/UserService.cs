using Azure.Core;
using SmartHub.Auth.Entities;
using SmartHub.Auth.Interfaces;
using SmartHub.Auth.Models;
using SmartHub.Auth.Repositories;
using SmartHub.Core.CommonUtility;
using SmartHub.Core.Interfaces;

namespace SmartHub.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<UserAuth?> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return new UserAuth
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task RegisterUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new Exception("Email and password are required.");

            if (!Utility.IsPasswordValid(password))
                throw new Exception("Password must be at least 8 characters and contain letters and numbers.");

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = passwordHash,
                Role = "User",
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
        }

        public async Task<LoginResponse> LoginUser(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var accessToken = _tokenService.GenerateAccessToken(
                user.Id,
                user.Email,
                user.Role
            );

            return new LoginResponse
            {
                AccessToken = accessToken,
                ExpiresInMinutes = 15
            };
        }
    }

}

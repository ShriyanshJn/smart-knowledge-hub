using SmartHub.Auth.Entities;
using SmartHub.Auth.Interfaces;
using SmartHub.Auth.Models;
using SmartHub.Auth.Repositories;

namespace SmartHub.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
    }

}

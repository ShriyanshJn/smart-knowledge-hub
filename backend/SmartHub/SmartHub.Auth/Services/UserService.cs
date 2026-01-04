using SmartHub.Auth.Interfaces;
using SmartHub.Auth.Models;

namespace SmartHub.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserAuth> GetUserByEmail(string email)
        {
            var user = new UserAuth();
            try
            {
                user = await _userRepository.GetUserByEmail(email);
            }
            catch (Exception ex)
            {
                throw;
            }
            return user;
        }
        public async Task RegisterUser(string email, string passwordHash, string role)
        {
            try
            {
                await _userRepository.RegisterUser(email, passwordHash, role);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}

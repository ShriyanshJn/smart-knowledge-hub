using SmartHub.Auth.Models;

namespace SmartHub.Auth.Interfaces
{
    public interface IUserRepository
    {
        Task<UserAuth> GetUserByEmail(string email);
        Task RegisterUser(string email, string passwordHash, string role);
    }
}

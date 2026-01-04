using SmartHub.Auth.Models;

namespace SmartHub.Auth.Interfaces
{
    public interface IUserService
    {
        Task<UserAuth> GetUserByEmail(string email);
    }
}

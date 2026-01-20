using System.Data;
using Microsoft.Data.SqlClient;
using SmartHub.Auth.Interfaces;
using SmartHub.Auth.Models;

namespace SmartHub.Auth.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(IConfiguration configuration)
        {
           
        }
        public async Task<UserAuth> GetUserByEmail(string email)
        {
            return new UserAuth();
        }
        public async Task RegisterUser(string email, string passwordHash, string role)
        {
            
        }

    }
}

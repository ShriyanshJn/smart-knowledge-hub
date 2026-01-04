using System.Data;
using Microsoft.Data.SqlClient;
using SmartHub.Auth.Interfaces;
using SmartHub.Auth.Models;

namespace SmartHub.Auth.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<UserAuth> GetUserByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("usp_GetUserByEmail", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new UserAuth
            {
                Id = reader.GetInt32("Id"),
                Email = reader.GetString("Email"),
                PasswordHash = reader.GetString("PasswordHash"),
                Role = reader.GetString("Role")
            };
        }
        public async Task RegisterUser(string email, string passwordHash, string role)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand("usp_RegisterUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@Role", role);

            await conn.OpenAsync();

            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new InvalidOperationException("Email already exists", ex);
            }
        }

    }
}

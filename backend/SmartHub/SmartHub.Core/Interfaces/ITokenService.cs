namespace SmartHub.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(int userId, string email, string role);
    }
}

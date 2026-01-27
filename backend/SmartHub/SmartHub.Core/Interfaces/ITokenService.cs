namespace SmartHub.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(Guid userId, string email, string role);
    }
}

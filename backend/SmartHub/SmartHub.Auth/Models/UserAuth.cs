namespace SmartHub.Auth.Models
{
    public class UserAuth
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "";
    }

}

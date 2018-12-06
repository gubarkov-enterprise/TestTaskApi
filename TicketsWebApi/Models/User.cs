namespace TicketsWebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public UserRole Role { get; set; }
    }

    public class UserAuthData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public enum UserRole
    {
        Administrator,
        Auditor
    }
}
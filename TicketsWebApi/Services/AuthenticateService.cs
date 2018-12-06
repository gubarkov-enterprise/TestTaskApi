using System;
using System.Linq;
using System.Text;
using TicketsWebApi.Models;

namespace TicketsWebApi.Services
{
    public interface IAuthenticateService
    {
        void Create(User user, string password);
        User Authenticate(string email, string password);
    }

    public class AuthenticateService : IAuthenticateService
    {
        public AuthenticateService(AppDbContext context)
        {
            Context = context;
        }

        private AppDbContext Context { get; set; }

        public void Create(User user, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(user.Email))
                throw new ArgumentNullException();

            if (Context.Users.Any(x => x.Email == user.Email))
                return;

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            Context.Users.Add(user);
            Context.SaveChanges();
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = Context.Users.SingleOrDefault(x => x.Email == email);

            if (user == null)
                return null;

            using (var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i]) return null;
                }
            }

            return user;
        }
    }
}
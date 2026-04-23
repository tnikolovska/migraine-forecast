using System.Text;
//using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace MigraineForecast.API.Services
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            /*using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);*/
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            //return HashPassword(password) == hash;
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication3.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? NRIC { get; set; }
        public string? UserEmail { get; set; }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => _password = HashPassword(value);
        }

        private string? HashPassword(string? password)
        {
            if (password == null)
                return null;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        public string? DateOfBirth { get; set; }
        public string? ResumeFilePath { get; set; }
        public string? WhoAmI { get; set; }
        public string? EncryptionKey { get; set; }

        public Guid? GUID { get; set; } = Guid.NewGuid();

        public string? ResetPasswordToken { get; set; }
    }
}

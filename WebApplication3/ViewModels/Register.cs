using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApplication3.ViewModels
{
    public class Register
    {

        [Required]
        [DataType(DataType.Text)]
        public string? FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string? Gender { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string? NRIC { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
            ErrorMessage = "Password must be at least 12 characters long and include a combination of lowercase, uppercase, numbers, and special characters.")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string? ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public string? DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile? Resume { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string? WhoAmI { get; set; }

    }
}

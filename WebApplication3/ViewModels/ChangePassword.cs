using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class ChangePassword
    {
        [Required, DataType(DataType.Password), Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Password confirmation does not match password")]
        public string? ConfirmNewPassword { get; set; }
    }
}

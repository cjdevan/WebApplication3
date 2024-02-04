using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.Pages;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _resetPasswordEmail;
        private readonly ITokenGenerator _tokenGenerator;

        [BindProperty]
        public ForgotPassword FPModel { get; set; }

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender resetPasswordEmail, ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _resetPasswordEmail = resetPasswordEmail;
            _tokenGenerator = tokenGenerator;
        }

		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByNameAsync(FPModel.Email);

				if (user != null && await _userManager.IsEmailConfirmedAsync(user))
				{
					var token = _tokenGenerator.GenerateRandomToken(6);

					// Save the token to the user (e.g., in a database)
					user.ResetPasswordToken = token;
					await _userManager.UpdateAsync(user);

					// Send the email
					_resetPasswordEmail.SendEmail(user.UserName, "Reset Password Token", token);

					return RedirectToPage("ResetPassword");
				}
			}
			return Page();
		}
	}
}


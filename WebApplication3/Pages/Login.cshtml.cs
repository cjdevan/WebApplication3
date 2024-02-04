using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
/*		private readonly AuditServiceModel _auditLogService;*/

		[BindProperty]
        public Login LModel { get; set; }

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IEmailSender emailSender /*AuditServiceModel _auditLogService*/)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
          /*  _auditLogService = auditLogService;*/
        }

        public void OnGet()
        {
        }   

        public IActionResult OnGetForgotPassword()
        {
            return RedirectToPage("ForgotPassword");
        }

		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
        {
            var recaptchaResponse = Request.Form["g-recaptcha-response"];

            // Validate the reCAPTCHA response using Google's reCAPTCHA API
            var recaptchaSecretKey = "6Lc5HWQpAAAAAPYbWgyN_Ju8lPXyUCD094dZR9Xa";
            var client = new HttpClient();
            var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecretKey}&response={recaptchaResponse}");

            // Parse the response
            var recaptchaResult = JsonConvert.DeserializeObject<RecaptchaResponse>(response);

            // Check if the reCAPTCHA was successful
            if (recaptchaResult.Success && ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(LModel.EmailAddress);

                if (user == null)
                {
                    // Handle the case where the user is not found
                    ModelState.AddModelError(string.Empty, "Invalid email or password");
                    return Page();
                }

                if (user.GUID != null)
                {
                    // Redirect to SessionError page if user.GUID is not null
                    return RedirectToPage("/errors/SessionError");
                }

                var result = await _signInManager.PasswordSignInAsync(
                    LModel.EmailAddress,
                    LModel.Password,
                    LModel.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    user.GUID = Guid.NewGuid();

                    // Create the security context
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.UserName),
                        new Claim("Department", "HR"),
                    };

                    var guid = user.GUID.ToString();

                    Response.Cookies.Append("GUID", guid, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                    });

                    var i = new ClaimsIdentity(claims, "MyCookieAuth");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    await _signInManager.UserManager.UpdateAsync(user);

                    // Redirect to Index page after successful login
                    return RedirectToPage("Index");
                }
                else if (result.IsLockedOut)
                {
                    // Account is locked out due to multiple login failures
                    return RedirectToPage("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password");
                }
            }

            // Return the page if reCAPTCHA validation fails or ModelState is invalid
            return Page();
        }
    }
}

/*public async Task<IActionResult> OnPostChangePasswordAsync()
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(LModel.EmailAddress);

				if (user != null)
				{
					var changePasswordResult = await _userManager.ChangePasswordAsync(user, LModel.Password, LModel.ChangePassword);

					if (changePasswordResult.Succeeded)
					{
						// Password change successful, you may redirect to a success page or perform additional actions
						return RedirectToPage("PasswordChangeSuccess");
					}
					else
					{
						// Password change failed, add errors to ModelState
						foreach (var error in changePasswordResult.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}

						return Page();
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "User with this email is not registered");
				}
			}

			return Page();
		}*/
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;

namespace WebApplication3.Pages
{
	public class LogoutModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;
		private UserManager<ApplicationUser> userManager{ get; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LogoutModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
		{
		}

		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostLogoutAsync()
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                // Set the GUID to Guid.Empty or null, depending on your preference
                user.GUID = null;

                // Update user information
                await signInManager.UserManager.UpdateAsync(user);
            }

            // Sign out the user
            await signInManager.SignOutAsync();

            // Clear all sessions
            var session = _httpContextAccessor.HttpContext.Session;
            session.Clear();
            HttpContext.Session.Clear();

            // Remove the authentication cookie from the client
            await HttpContext.SignOutAsync("MyCookieAuth");

            return RedirectToPage("Login");
        }

        public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}

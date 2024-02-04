using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [BindProperty]
        public ChangePassword CPModel { get; set; }

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                // Check if the current password matches the user's actual password
                var currentPasswordCheckResult = await _signInManager.CheckPasswordSignInAsync(user, CPModel.CurrentPassword, false);

                if (currentPasswordCheckResult.Succeeded)
                {
                    // Check if the new password is different from the current password
                    if (CPModel.CurrentPassword == CPModel.NewPassword)
                    {
                        ModelState.AddModelError(string.Empty, "New password has to differ from the current password");
                    }
                    else
                    {
                        // Fetch all claims for the user
                        var allClaims = await _userManager.GetClaimsAsync(user);

                        // Filter claims based on the claim type
                        var lastChangedClaim = allClaims.FirstOrDefault(c => c.Type == "PasswordChanged");

                        if (lastChangedClaim != null)
                        {
                            var lastChangedDate = DateTime.Parse(lastChangedClaim.Value);
                            var daysSinceLastChange = (DateTime.UtcNow - lastChangedDate).TotalDays;

                            // Check if it's been at least 30 days since the last change
                            if (daysSinceLastChange >= 30)
                            {
                                // Proceed to change the password
                                var changePasswordResult = await _userManager.ChangePasswordAsync(user, CPModel.CurrentPassword, CPModel.NewPassword);

                                if (changePasswordResult.Succeeded)
                                {
                                    // Successfully changed the password
                                    await _signInManager.RefreshSignInAsync(user);

                                    // Update the "PasswordChanged" claim with the current date
                                    await _userManager.RemoveClaimAsync(user, lastChangedClaim);
                                    await _userManager.AddClaimAsync(user, new Claim("PasswordChanged", DateTime.UtcNow.ToString("o")));

                                    // Add a success message to TempData
                                    TempData["SuccessMessage"] = "Password changed successfully.";
                                }
                                else
                                {
                                    // Handle password change errors
                                    foreach (var error in changePasswordResult.Errors)
                                    {
                                        ModelState.AddModelError(string.Empty, error.Description);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "You can only change your password every 30 days.");
                            }
                        }
                        else
                        {
                            // If there is no "PasswordChanged" claim, this is the first time changing the password
                            // Proceed to change the password
                            var changePasswordResult = await _userManager.ChangePasswordAsync(user, CPModel.CurrentPassword, CPModel.NewPassword);

                            if (changePasswordResult.Succeeded)
                            {
                                // Successfully changed the password
                                await _signInManager.RefreshSignInAsync(user);

                                // Add the "PasswordChanged" claim with the current date
                                await _userManager.AddClaimAsync(user, new Claim("PasswordChanged", DateTime.UtcNow.ToString("o")));

                                // Add a success message to TempData
                                TempData["SuccessMessage"] = "Password changed successfully.";
                            }
                            else
                            {
                                // Handle password change errors
                                foreach (var error in changePasswordResult.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Current password is incorrect
                    ModelState.AddModelError(string.Empty, "Current password is incorrect");
                }
            }

            // If ModelState is not valid or any other error occurs, return to the page
            return Page();
        }
    }
}




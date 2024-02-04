using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AuthDbContext _context;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AuthDbContext context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
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
                var existingUser = await _userManager.FindByEmailAsync(RModel.EmailAddress);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email address is already registered.");
                    return Page();
                }

                if (RModel.Resume != null)
                {
                    var fileExtension = Path.GetExtension(RModel.Resume.FileName).ToLowerInvariant();
                    if (fileExtension != ".pdf" && fileExtension != ".docx")
                    {
                        ModelState.AddModelError("", "Invalid file format. Allowed formats are .pdf and .docx.");
                        return Page();
                    }
                }

                var filePath = await HandleFileUpload(RModel.Resume);

                var key = EncryptDecrypt.KeyGenerator.GenerateRandomKey(32);
                var keyBytes = Convert.FromBase64String(key);

                var user = new ApplicationUser()
                {
                    FirstName = RModel.FirstName,
                    LastName = RModel.LastName,
                    Gender = RModel.Gender,
                    NRIC = Convert.ToBase64String(EncryptDecrypt.EncryptData(RModel.NRIC, keyBytes)),
                    UserName = RModel.EmailAddress,
                    Password = RModel.Password, 
                    DateOfBirth = RModel.DateOfBirth,
                    ResumeFilePath = filePath,
                    WhoAmI = RModel.WhoAmI,
                    GUID = null,
                };

                var result = await _userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    _emailSender.SendEmail(user.UserName, "Ace Job Agency", null);
                    return RedirectToPage("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

		[Authorize]
		[ValidateAntiForgeryToken]
		private async Task<string> HandleFileUpload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return filePath;
            }

            return null;
        }
    }
}

/*public void SendEmail(string toEmail, string subject)
{
    // Ensure the toEmail is valid before sending the email
    if (string.IsNullOrWhiteSpace(toEmail))
    {
        return;
    }

    SmtpClient client = new SmtpClient("smtp.ethereal.email", 587);
    client.EnableSsl = true;
    client.UseDefaultCredentials = false;

    client.Credentials = new NetworkCredential("carljdevan@gmail.com", "T0510758f");

    // Create email message
    MailMessage mailMessage = new MailMessage();
    mailMessage.From = new MailAddress("carljdevan@gmail.com");
    mailMessage.To.Add(RModel.EmailAddress);
    mailMessage.Subject = "Registration Code";
    mailMessage.IsBodyHtml = true;
    StringBuilder mailBody = new StringBuilder();
    mailBody.AppendFormat("<h1>User Registered</h1>");
    mailBody.AppendFormat("<br />");
    mailBody.AppendFormat("<p>Thank you For registering account</p>");
    mailMessage.Body = mailBody.ToString();

    // Send email
    client.Send(mailMessage);
}
*/


/*public async Task<IActionResult> OnPostAsync()
       {
           if (ModelState.IsValid)
           {
               // Check for duplicate email
               var existingUser = await userManager.FindByEmailAsync(RModel.EmailAddress);
               if (existingUser != null)
               {
                   ModelState.AddModelError("", "Email address is already registered.");
                   return Page();
               }

               // Validate file extension
               if (RModel.Resume != null)
               {
                   var fileExtension = Path.GetExtension(RModel.Resume.FileName).ToLowerInvariant();
                   if (fileExtension != ".pdf" && fileExtension != ".docx")
                   {
                       ModelState.AddModelError("", "Invalid file format. Allowed formats are .pdf and .docx.");
                       return Page();
                   }
               }

               // Handle file upload
               string filePath = await HandleFileUpload(RModel.Resume);

               // Generate a random key
               var key = EncryptDecrypt.KeyGenerator.GenerateRandomKey(32);
               var keyBytes = Convert.FromBase64String(key);

               // Encrypt user data
               var encryptedUser = new ApplicationUser()
               {
                   FirstName = Convert.ToBase64String(EncryptDecrypt.EncryptData(RModel.FirstName, keyBytes)),
                   LastName = Convert.ToBase64String(EncryptDecrypt.EncryptData(RModel.LastName, keyBytes)),
                   Gender = Convert.ToBase64String(EncryptDecrypt.EncryptData(RModel.Gender, keyBytes)),
                   NRIC = Convert.ToBase64String(EncryptDecrypt.EncryptData(RModel.NRIC, keyBytes)),
                   UserName = RModel.EmailAddress,
                   Password = Convert.ToBase64String(EncryptDecrypt.EncryptData(RModel.Password, keyBytes)),
                   DateOfBirth = Convert.ToBase64String(EncryptDecrypt.EncryptData(RModel.DateOfBirth, keyBytes)),
                   ResumeFilePath = Convert.ToBase64String(EncryptDecrypt.EncryptData(filePath, keyBytes)),
                   WhoAmI = Convert.ToBase64String(EncryptDecrypt.EncryptData(RModel.WhoAmI, keyBytes)),
                   EncryptionKey = Convert.ToBase64String(keyBytes)
               };

               var result = await userManager.CreateAsync(encryptedUser, RModel.Password);
               if (result.Succeeded)
               {
                   await signInManager.SignInAsync(encryptedUser, false);
                   return RedirectToPage("Index");
               }

               foreach (var error in result.Errors)
               {
                   ModelState.AddModelError("", error.Description);
               }
           }

           return Page();
       }*/


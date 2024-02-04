using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly IHttpContextAccessor contxt;

		public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _userManager = userManager;
            contxt = httpContextAccessor;
        }

		public List<ViewModels.Index> Users { get; set; }

		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task OnGetAsync()
        {
            Users = new List<ViewModels.Index>();

            // Retrieve the current user from the database
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                var UserViewModel = new ViewModels.Index
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Gender = currentUser.Gender,
                    NRIC = currentUser.NRIC ,
                    UserEmail = currentUser.UserName,
                    Password = currentUser.Password,
                    DateOfBirth = currentUser.DateOfBirth,
                    ResumeFilePath = currentUser.ResumeFilePath,
                    WhoAmI = currentUser.WhoAmI,
                };

                Users.Add(UserViewModel);
            }
        }

		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnGetDownloadAsync(string ResumeFilePath)
        { 
            if (string.IsNullOrEmpty(ResumeFilePath))
            {
                return NotFound();
            }

            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ResumeFilePath);

            if (!System.IO.File.Exists(file))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(file, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(file), Path.GetFileName(file));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".pdf", "application/pdf"},
            };
        }
    }
}






/* public IEnumerable<string> GetSessionInfo()
 {
     List<string> sessionInfo = new List<string>();

     if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionVariables.SessionKeyUsername)))
     {
         HttpContext.Session.SetString(SessionKeyEnum.SessionKeyUsername.ToString(), "Current User");
         HttpContext.Session.SetString(SessionKeyEnum.SessionKeySessionID.ToString(), Guid.NewGuid().ToString());
     }

     var username = HttpContext.Session.GetString(SessionVariables.SessionKeyUsername);
     var sessionId = HttpContext.Session.GetString(SessionVariables.SessionKeySessionID);

     sessionInfo.Add(username);
     sessionInfo.Add(sessionId);

     return sessionInfo;
 }
*/

/*public async Task OnGetAsync()
{
    *//*var jsonUserData = Request.Cookies["carlSession"];
    if (jsonUserData != null)
    {
        var userFromSession = JsonConvert.DeserializeObject<ApplicationUser>(jsonUserData);
        var userEmail = userFromSession.Email;
        var allUsers = _userManager.Users;
        var specificUser = (ApplicationUser)null;
        foreach (var user in allUsers)
        {
            if (user.Email == userEmail)
            {
                Console.WriteLine("Found email");
                specificUser = user;
                break;
            }
        }
    }*//*

    Users = new List<ViewModels.Index>();

    // Retrieve the current user from the database
    var currentUser = await _userManager.GetUserAsync(User);

    if (currentUser != null && !string.IsNullOrEmpty(currentUser.EncryptionKey))
    {
        // Retrieve the encryption key from the user's Password property
        byte[] encryptionKeyBytes = Convert.FromBase64String(currentUser.EncryptionKey);

        // Decrypt the ApplicationUser properties from the user
        var UserViewModel = new ViewModels.Index
        {
            FirstName = EncryptDecrypt.DecryptData(currentUser.FirstName, encryptionKeyBytes),
            LastName = EncryptDecrypt.DecryptData(currentUser.LastName, encryptionKeyBytes),
            Gender = EncryptDecrypt.DecryptData(currentUser.Gender, encryptionKeyBytes),
            NRIC = EncryptDecrypt.DecryptData(currentUser.NRIC, encryptionKeyBytes),
            UserEmail = currentUser.UserName,
            Password = EncryptDecrypt.DecryptData(currentUser.Password, encryptionKeyBytes),
            DateOfBirth = EncryptDecrypt.DecryptData(currentUser.DateOfBirth, encryptionKeyBytes),
            ResumeFilePath = EncryptDecrypt.DecryptData(currentUser.ResumeFilePath, encryptionKeyBytes),
            WhoAmI = EncryptDecrypt.DecryptData(currentUser.WhoAmI, encryptionKeyBytes),
        };

        Users.Add(UserViewModel);
    }
}*/
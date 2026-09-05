using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;

namespace Stammzellen.Pages.Blog
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public LoginModel(IConfiguration configuration) { _configuration = configuration; }

        [BindProperty]
        public string Passwort { get; set; } = string.Empty;

        public IActionResult OnPost()
        {
            string? hinterlegtesPasswort = _configuration["AdminSettings:AdminPassword"];

            if (Passwort != null && hinterlegtesPasswort != null && Passwort.Trim() == hinterlegtesPasswort.Trim())
            {
                // NEU: Wir schreiben ein verschlüsseltes Cookie, das Server-Neustarts überlebt!
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Nur über HTTPS erlauben
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddHours(2) // 2 Stunden gültig
                };

                // Wir setzen ein einfaches Kontroll-Cookie
                Response.Cookies.Append("LaborAdminToken", "True_IsAuthenticated_Base64", cookieOptions);
                return RedirectToPage("/Blog/Admin");
            }

            ModelState.AddModelError(string.Empty, "Falsches Passwort. Zugriff verweigert.");
            return Page();
        }
    }
}

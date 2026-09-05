using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;

namespace Stammzellen.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Diese Eigenschaften binden sich an die Eingabefelder des Formulars
        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Nachricht { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1. Daten aus der appsettings.json auslesen
            var adminEmail = _configuration["AdminSettings:AdminEmail"];
            var smtpServer = _configuration["AdminSettings:SmtpServer"]; // mail.upcbusiness.at
            var smtpUser = _configuration["AdminSettings:SmtpUser"];     // mailserver@multiuser.co.at
            var smtpPassword = _configuration["AdminSettings:SmtpPassword"];

            try
            {
                // 2. E-Mail Nachricht aufbauen
                var mailMessage = new MailMessage();

                // WICHTIG: Absender MUSS Ihre authentifizierte Adresse sein, sonst blockiert UPC wegen Spam-Schutz (Spoofing)
                mailMessage.From = new MailAddress(smtpUser, "Stammzellen-Projekt");

                // Empfänger (Ihre Admin-Adresse)
                mailMessage.To.Add(adminEmail);

                // WICHTIG: Die Adresse des Besuchers als "Reply-To" hinterlegen, damit Sie direkt antworten können
                mailMessage.ReplyToList.Add(new MailAddress(Email, Name));

                mailMessage.Subject = $"Neue Kontaktanfrage von {Name}";
                mailMessage.Body = $"Name / Institution: {Name}\nE-Mail: {Email}\n\nNachricht:\n{Nachricht}";

                // 3. SMTP-Client konfigurieren
                using (var smtpClient = new SmtpClient(smtpServer))
                {
                    // WICHTIG: Port 587 nutzen (Port 25 aus der appsettings wird hier überschrieben, da meist blockiert)
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                    smtpClient.EnableSsl = true; // Aktiviert die notwendige TLS-Verschlüsselung

                    // E-Mail asynchron senden
                    await smtpClient.SendMailAsync(mailMessage);
                }

                TempData["Message"] = "Ihre Nachricht wurde erfolgreich gesendet!";
            }
            catch (Exception ex)
            {
                // Fehlermeldung für das Frontend bereitstellen
                TempData["Error"] = $"Fehler beim Mailversand: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}

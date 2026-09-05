using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stammzellen.Data;
using Stammzellen.Models;
using System;
using System.Threading.Tasks;

namespace Stammzellen.Pages.Blog
{
    public class CreateModel : PageModel
    {
        private readonly DataDbContext _context;

        public CreateModel(DataDbContext context)
        {
            _context = context;
        }

        // BindProperty verbindet das HTML-Formular direkt mit diesem C#-Objekt
        [BindProperty]
        public BlogPost NeuerPost { get; set; } = new();

        public void OnGet()
        {
            // Lädt die leere Seite
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Prüft, ob Titel und Inhalt ordnungsgemäß ausgefüllt wurden
            if (string.IsNullOrWhiteSpace(NeuerPost.Titel) || string.IsNullOrWhiteSpace(NeuerPost.Inhalt))
            {
                ModelState.AddModelError(string.Empty, "Bitte füllen Sie sowohl den Titel als auch den Inhalt aus.");
                return Page();
            }

            NeuerPost.ErstelltAm = DateTime.Now;

            // Fügt den neuen Beitrag der SQLite-Datenbank hinzu
            _context.BlogPosts.Add(NeuerPost);
            await _context.SaveChangesAsync();

            // Nach dem Speichern leiten wir den Admin direkt zur Admin-Übersicht weiter
            return RedirectToPage("/Blog/Admin");
        }
    }
}

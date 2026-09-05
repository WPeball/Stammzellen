using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stammzellen.Data;
using Stammzellen.Models;
using System.Threading.Tasks;

namespace Stammzellen.Pages.Blog
{
    public class EditModel : PageModel
    {
        private readonly DataDbContext _context;

        public EditModel(DataDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BlogPost BearbeiteterPost { get; set; } = default!;

        // Lädt den bestehenden Beitrag in das Formular
        public async Task<IActionResult> OnGetAsync(int id)
        {
            BearbeiteterPost = await _context.BlogPosts.FindAsync(id);

            if (BearbeiteterPost == null)
            {
                return NotFound();
            }
            return Page();
        }

        // Speichert die Änderungen in der SQLite-Datenbank
        public async Task<IActionResult> OnPostAsync()
        {
            // HIER ERGÄNZET: Prüft jetzt auch, ob der Autor ausgefüllt wurde
            if (string.IsNullOrWhiteSpace(BearbeiteterPost.Titel) ||
                string.IsNullOrWhiteSpace(BearbeiteterPost.Inhalt) ||
                string.IsNullOrWhiteSpace(BearbeiteterPost.Autor))
            {
                ModelState.AddModelError(string.Empty, "Bitte füllen Sie alle Felder (Autor, Titel und Inhalt) aus.");
                return Page();
            }

            // Sagt Entity Framework, dass dieser Post geändert wurde
            _context.Attach(BearbeiteterPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BlogPosts.Any(e => e.Id == BearbeiteterPost.Id))
                {
                    return NotFound();
                }
                throw;
            }

            TempData["Message"] = "Der Beitrag wurde erfolgreich aktualisiert.";
            return RedirectToPage("/Blog/Admin");
        }

    }
}

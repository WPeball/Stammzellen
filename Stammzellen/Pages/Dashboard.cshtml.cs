using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stammzellen.Data; // Nutzt deinen Daten-Ordner
using Stammzellen.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stammzellen.Pages
{
    public class DashboardModel : PageModel
    {
        // HIER GEÄNDERT: Nutzt jetzt deinen echten DataDbContext statt ApplicationDbContext
        private readonly DataDbContext _context;

        public DashboardModel(DataDbContext context)
        {
            _context = context;
        }

        // Liste für die Anzeige aller Proben in der Tabelle
        public IList<StemCellSample> StemCellSamples { get; set; } = new List<StemCellSample>();

        // BindProperty verbindet das HTML-Formular direkt mit diesem Objekt
        [BindProperty]
        public StemCellSample NewSample { get; set; } = new();

        // Wird aufgerufen, wenn die Seite geladen wird
        public async Task OnGetAsync()
        {
            StemCellSamples = await _context.StemCellSamples.ToListAsync();
        }

        // Wird aufgerufen, wenn das Formular abgeschickt wird (POST)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                StemCellSamples = await _context.StemCellSamples.ToListAsync();
                return Page();
            }

            // Speichert die neue Probe in der SQLite-Datenbank
            _context.StemCellSamples.Add(NewSample);
            await _context.SaveChangesAsync();

            // Lädt die Seite neu, um die Liste zu aktualisieren
            return RedirectToPage();
        }
    }
}

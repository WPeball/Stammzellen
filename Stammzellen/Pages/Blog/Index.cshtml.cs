using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stammzellen.Data;
using Stammzellen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stammzellen.Pages.Blog
{
    public class IndexModel : PageModel
    {
        private readonly DataDbContext _context;

        public IndexModel(DataDbContext context)
        {
            _context = context;
        }

        public IList<BlogPost> BlogPosts { get; set; } = default!;

        // Filter-Eigenschaften
        [BindProperty(SupportsGet = true)]
        public string? FilterPseudonym { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDatum { get; set; }

        // NEU: Filter für den Autor
        [BindProperty(SupportsGet = true)]
        public string? FilterAutor { get; set; }

        // NEU: Eigenschaften für das Blättern (Pagination)
        [BindProperty(SupportsGet = true)]
        public int AktuelleSeite { get; set; } = 1;
        public int GesamtSeiten { get; set; }
        private const int BeitraegeProSeite = 3; // Exakt 3 Einträge pro Seite

        [BindProperty]
        public Comment NeuerKommentar { get; set; } = new();

        public async Task OnGetAsync()
        {
            var query = _context.BlogPosts
                .Include(b => b.Kommentare.Where(c => c.IstFreigegeben))
                .AsQueryable();

            // 1. Filter nach Datum
            if (FilterDatum.HasValue)
            {
                query = query.Where(b => b.ErstelltAm.Date == FilterDatum.Value.Date);
            }

            // 2. Filter nach Kommentator/Pseudonym
            if (!string.IsNullOrEmpty(FilterPseudonym))
            {
                query = query.Where(b => b.Kommentare.Any(c => c.IstFreigegeben && c.Pseudonym.Contains(FilterPseudonym)));
            }

            // 3. NEU: Filter nach Autor
            if (!string.IsNullOrEmpty(FilterAutor))
            {
                query = query.Where(b => b.Autor.Contains(FilterAutor));
            }

            // NEU: Berechnung für das Blättern (Pagination)
            int gesamtEintraege = await query.CountAsync();
            GesamtSeiten = (int)Math.Ceiling(gesamtEintraege / (double)BeitraegeProSeite);

            if (AktuelleSeite < 1) AktuelleSeite = 1;

            // Holt nur die 3 Beiträge für die aktuelle Seite aus der SQLite-Datenbank
            BlogPosts = await query
                .OrderByDescending(b => b.ErstelltAm)
                .Skip((AktuelleSeite - 1) * BeitraegeProSeite)
                .Take(BeitraegeProSeite)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCommentAsync(int blogPostId)
        {
            if (string.IsNullOrWhiteSpace(NeuerKommentar.Pseudonym) || string.IsNullOrWhiteSpace(NeuerKommentar.Text))
            {
                return RedirectToPage();
            }

            NeuerKommentar.BlogPostId = blogPostId;
            NeuerKommentar.ErstelltAm = DateTime.Now;
            NeuerKommentar.IstFreigegeben = false;

            _context.Comments.Add(NeuerKommentar);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Vielen Dank! Ihr Kommentar wurde gespeichert und wird nach redaktioneller Prüfung freigegeben.";

            return RedirectToPage();
        }
    }
}

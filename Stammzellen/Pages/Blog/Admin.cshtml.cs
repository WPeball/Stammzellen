using Microsoft.AspNetCore.Http;
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
    public class AdminModel : PageModel
    {
        private readonly DataDbContext _context;
        public AdminModel(DataDbContext context) { _context = context; }

        public IList<BlogPost> AlleBeiträge { get; set; } = new List<BlogPost>();
        public IList<Comment> AlleKommentare { get; set; } = new List<Comment>();

        [BindProperty(SupportsGet = true)]
        public string? FilterAutor { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDatum { get; set; }

        // Pagination Beiträge
        [BindProperty(SupportsGet = true)]
        public int AktuelleSeite { get; set; } = 1;
        public int GesamtSeiten { get; set; }
        private const int BeitraegeProSeite = 5;

        // NEU: Pagination Kommentare (10 pro Seite)
        [BindProperty(SupportsGet = true)]
        public int KommentarSeite { get; set; } = 1;
        public int GesamtKommentarSeiten { get; set; }
        private const int KommentareProSeite = 10;

        // NEU: ID des Kommentars, der gerade im Editier-Modus ist
        [BindProperty(SupportsGet = true)]
        public int? EditKommentarId { get; set; }

        // NEU: Der editierte Text des Kommentars
        [BindProperty]
        public string? EditKommentarText { get; set; }

        [BindProperty]
        public Comment NeuerAdminKommentar { get; set; } = new();

        private bool IsAdmin() => Request.Cookies["LaborAdminToken"] == "True_IsAuthenticated_Base64";

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdmin()) return RedirectToPage("/Blog/Login");

            // --- BEITRÄGE FILTERN & BLÄTTERN (5er) ---
            var postQuery = _context.BlogPosts.AsQueryable();
            if (!string.IsNullOrEmpty(FilterAutor)) postQuery = postQuery.Where(b => b.Autor.Contains(FilterAutor));
            if (FilterDatum.HasValue) postQuery = postQuery.Where(b => b.ErstelltAm.Date == FilterDatum.Value.Date);

            int gesamtEintraege = await postQuery.CountAsync();
            GesamtSeiten = (int)Math.Ceiling(gesamtEintraege / (double)BeitraegeProSeite);
            if (AktuelleSeite < 1) AktuelleSeite = 1;

            AlleBeiträge = await postQuery
                .OrderByDescending(b => b.ErstelltAm)
                .Skip((AktuelleSeite - 1) * BeitraegeProSeite)
                .Take(BeitraegeProSeite)
                .ToListAsync();

            // --- NEU: ALL KOMMENTARE BLÄTTERN (10er) ---
            var commentQuery = _context.Comments.Include(c => c.BlogPost).AsQueryable();
            int gesamtKommentare = await commentQuery.CountAsync();
            GesamtKommentarSeiten = (int)Math.Ceiling(gesamtKommentare / (double)KommentareProSeite);
            if (KommentarSeite < 1) KommentarSeite = 1;

            AlleKommentare = await commentQuery
                .OrderByDescending(c => c.ErstelltAm)
                .Skip((KommentarSeite - 1) * KommentareProSeite)
                .Take(KommentareProSeite)
                .ToListAsync();

            // Wenn wir im Editier-Modus sind, laden wir den bestehenden Text vorab
            if (EditKommentarId.HasValue && string.IsNullOrEmpty(EditKommentarText))
            {
                var target = AlleKommentare.FirstOrDefault(c => c.Id == EditKommentarId.Value);
                if (target != null) EditKommentarText = target.Text;
            }

            return Page();
        }

        // NEU: Kommentar Änderungen speichern
        public async Task<IActionResult> OnPostSaveCommentAsync(int id)
        {
            if (!IsAdmin()) return RedirectToPage("/Blog/Login");

            var kommentar = await _context.Comments.FindAsync(id);
            if (kommentar != null && !string.IsNullOrWhiteSpace(EditKommentarText))
            {
                kommentar.Text = EditKommentarText;
                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = "Kommentar-Inhalt wurde erfolgreich aktualisiert.";
            }

            // Beendet den Editier-Modus und behält die Filter/Seiten bei
            return RedirectToPage(new { AktuelleSeite, KommentarSeite, FilterAutor, FilterDatum });
        }

        public async Task<IActionResult> OnPostAddCommentAsync(int blogPostId)
        {
            if (!IsAdmin()) return RedirectToPage("/Blog/Login");
            if (!string.IsNullOrWhiteSpace(NeuerAdminKommentar.Pseudonym) && !string.IsNullOrWhiteSpace(NeuerAdminKommentar.Text))
            {
                NeuerAdminKommentar.BlogPostId = blogPostId;
                NeuerAdminKommentar.ErstelltAm = DateTime.Now;
                NeuerAdminKommentar.IstFreigegeben = true;
                _context.Comments.Add(NeuerAdminKommentar);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { AktuelleSeite, KommentarSeite, FilterAutor, FilterDatum });
        }

        public async Task<IActionResult> OnPostToggleCommentAsync(int id)
        {
            if (!IsAdmin()) return RedirectToPage("/Blog/Login");
            var kommentar = await _context.Comments.FindAsync(id);
            if (kommentar != null) { kommentar.IstFreigegeben = !kommentar.IstFreigegeben; await _context.SaveChangesAsync(); }
            return RedirectToPage(new { AktuelleSeite, KommentarSeite, FilterAutor, FilterDatum });
        }

        public async Task<IActionResult> OnPostDeleteCommentAsync(int id)
        {
            if (!IsAdmin()) return RedirectToPage("/Blog/Login");
            var kommentar = await _context.Comments.FindAsync(id);
            if (kommentar != null) { _context.Comments.Remove(kommentar); await _context.SaveChangesAsync(); }
            return RedirectToPage(new { AktuelleSeite, KommentarSeite, FilterAutor, FilterDatum });
        }

        public async Task<IActionResult> OnPostDeletePostAsync(int id)
        {
            if (!IsAdmin()) return RedirectToPage("/Blog/Login");
            var post = await _context.BlogPosts.Include(b => b.Kommentare).FirstOrDefaultAsync(b => b.Id == id);
            if (post != null) { _context.BlogPosts.Remove(post); await _context.SaveChangesAsync(); }
            return RedirectToPage(new { AktuelleSeite, KommentarSeite, FilterAutor, FilterDatum });
        }

        public IActionResult OnPostLogout() { Response.Cookies.Delete("LaborAdminToken"); return RedirectToPage("/Blog/Index"); }
    }
}

using System;

namespace Stammzellen.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; } // Zu welchem Blogeintrag gehört der Kommentar?
        public string Pseudonym { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime ErstelltAm { get; set; } = DateTime.Now;

        // WICHTIG: Deine Anforderung, dass Kommentare erst freigegeben werden müssen
        public bool IstFreigegeben { get; set; } = false;

        // Navigationseigenschaft zurück zum Blogpost
        public BlogPost? BlogPost { get; set; }
    }
}

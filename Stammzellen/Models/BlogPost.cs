using System;
using System.Collections.Generic;

namespace Stammzellen.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Titel { get; set; } = string.Empty;
        public string Inhalt { get; set; } = string.Empty;
        public DateTime ErstelltAm { get; set; } = DateTime.Now;

        // NEU: Jeder Blogpost benötigt einen Autor
        public string Autor { get; set; } = "Unbekannter Forscher";

        public List<Comment> Kommentare { get; set; } = new();
    }
}

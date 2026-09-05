using Microsoft.EntityFrameworkCore;
using Stammzellen.Models; // Wichtig, damit die Modelle gefunden werden

namespace Stammzellen.Data
{
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options)
            : base(options)
        {
        }

        // Stelle sicher, dass diese drei Tabellen hier eingetragen sind:
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        // HIER DIESE ZEILE ERGÄNZEN:
        public DbSet<StemCellSample> StemCellSamples { get; set; }
    }
}

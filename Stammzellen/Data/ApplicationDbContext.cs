using Microsoft.EntityFrameworkCore;
using Stammzellen.Models;

namespace Stammzellen.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Deine drei Datenbanktabellen für SQLite:
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<StemCellSample> StemCellSamples { get; set; }
    }
}

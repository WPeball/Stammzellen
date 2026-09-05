using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Stammzellen.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Hier sagen wir dem Tool direkt, dass es SQLite nutzen soll
            optionsBuilder.UseSqlite("Data Source=stammzellen.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

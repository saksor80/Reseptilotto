using Microsoft.EntityFrameworkCore;

namespace Reseptilotto.Models
{
    public class ReseptiContext : DbContext
    {
        public ReseptiContext(DbContextOptions<ReseptiContext> options)
            : base(options)
        {
        }

        public DbSet<Resepti> InputItems { get; set; }

    }
}

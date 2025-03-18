using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class LicenseDbContext : DbContext
    {
        public LicenseDbContext(DbContextOptions<LicenseDbContext> options) : base(options) { }

        public DbSet<LicenseKey> LicenseKeys { get; set; }
    }
}

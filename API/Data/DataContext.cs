using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder){
            builder.Entity<AppUser>()
                .HasIndex(x => x.UserName)
                .IsUnique(true);
        }
    }
}

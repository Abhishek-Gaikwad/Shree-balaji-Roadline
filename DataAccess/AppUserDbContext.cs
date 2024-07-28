using Microsoft.EntityFrameworkCore;
using Trans9.Models;

namespace Trans9.DataAccess
{
    public class AppUserDbContext : DbContext
    {
        public AppUserDbContext(DbContextOptions<AppUserDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.id);
                entity.Property(e => e.roleName);
                entity.Property(e => e.pageIds);
            });

            builder.Entity<Pages>(entity =>
            {
                entity.Property(e => e.id);
                entity.Property(e => e.pageName);
                entity.Property(e => e.urlName);
                entity.Property(e => e.icon);
                entity.Property(e => e.priority);
                entity.Property(e => e.pgGroup);
            });
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserRole> RoleGroup { get; set; }
        public DbSet<Pages> Pages { get; set; } = null!;
    }
}

using Microsoft.EntityFrameworkCore;

using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.User.Entities;

namespace RestApiCase.Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }  // DbSet para User (raiz)
        public DbSet<Role> Roles { get; set; }  // DbSet para Role (entidade)

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();

                entity.HasMany(u => u.Roles)
                      .WithOne()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.RoleType).IsRequired();
                entity.Property(r => r.UserId).IsRequired();
            });
        }
    }
}
using Microsoft.EntityFrameworkCore;
using RestApiCase.Domain.Commons;
using RestApiCase.Domain.Logging.Entities;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.User.Entities;

namespace RestApiCase.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Description)
                      .HasMaxLength(500);
                entity.Property(e => e.CreatedAt)
                      .IsRequired();
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.DueDate);
            });

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
                entity.Navigation(u => u.Roles).UsePropertyAccessMode(PropertyAccessMode.Field);
                entity.HasOne<RefreshToken>()
                     .WithOne()
                     .HasForeignKey<RefreshToken>(r => r.UserId)
                     .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.RoleType).IsRequired();
                entity.Property(r => r.UserId).IsRequired();
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Token).IsRequired();
                entity.Property(r => r.ExpiresAt).IsRequired();
                entity.Property(r => r.UserId).IsRequired();
                entity.Property(r => r.IsRevoked).IsRequired();
            });

            modelBuilder.Entity<RequestLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Method).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Path).IsRequired().HasMaxLength(500);
                entity.Property(e => e.StatusCode).IsRequired();
                entity.Property(e => e.ElapsedMs).IsRequired();
                entity.Property(e => e.LogType).IsRequired();
                entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
                entity.Property(e => e.StackTrace).HasMaxLength(4000);
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
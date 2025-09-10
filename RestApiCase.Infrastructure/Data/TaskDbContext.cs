using Microsoft.EntityFrameworkCore;

using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.User.Entities;

namespace RestApiCase.Infrastructure.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(e => e.Password)
                      .IsRequired()
                      .HasMaxLength(100);
            });
        }
    }
}
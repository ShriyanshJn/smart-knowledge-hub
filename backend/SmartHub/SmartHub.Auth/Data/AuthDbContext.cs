using Microsoft.EntityFrameworkCore;
using SmartHub.Auth.Entities;

namespace SmartHub.Auth.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasColumnName("id");

            entity.Property(u => u.Email)
                .HasColumnName("email")
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();

            entity.Property(u => u.Role)
                .HasColumnName("role");

            entity.Property(u => u.CreatedDate)
                .HasColumnName("created_date");

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });
    }
}

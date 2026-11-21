using CarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<AppUser>();
        user.ToTable("app_users");
        user.HasKey(x => x.Id);
        user.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
        user.Property(x => x.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
        user.Property(x => x.NormalizedUsername).HasColumnName("normalized_username").HasMaxLength(50).IsRequired();
        user.Property(x => x.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
        user.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(500).IsRequired();
        user.Property(x => x.Role).HasColumnName("role").HasMaxLength(30).IsRequired().HasDefaultValue("User");
        user.HasIndex(x => x.NormalizedUsername).IsUnique();
        user.HasIndex(x => x.Email).IsUnique();
    }
}

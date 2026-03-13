using DevicesApi.Domain.Entities;
using DevicesApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevicesApi.Infrastructure.Persistence;

public sealed class DevicesDbContext(DbContextOptions<DevicesDbContext> options) : DbContext(options)
{
    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(d => d.Brand)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(d => d.State)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(d => d.CreationTime)
                .IsRequired()
                .ValueGeneratedNever();

        });
    }
}

using BookTheShow.Domain.Entities;
using BookTheShow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookTheShow.Infrastructure.Persistence.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("Venues");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.TotalCapacity)
            .IsRequired();

        builder.Property(v => v.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(v => v.Description)
            .HasMaxLength(1000);

        builder.Property(v => v.ImageUrl)
            .HasMaxLength(500);

        builder.Property(v => v.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(v => v.City);
        builder.HasIndex(v => v.Type);
        builder.HasIndex(v => v.IsActive);

        // Relationships
        builder.HasMany(v => v.Seats)
            .WithOne(s => s.Venue)
            .HasForeignKey(s => s.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Events)
            .WithOne(e => e.Venue)
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using BookTheShow.Domain.Entities;
using BookTheShow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookTheShow.Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.VenueId)
            .IsRequired();

        builder.Property(s => s.SectionCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.RowCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.SeatNumber)
            .IsRequired();

        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(s => s.PricingTier)
            .IsRequired()
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(1.0m);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(s => s.VenueId);
        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.IsActive);
        
        // Composite unique index for seat identification within a venue
        builder.HasIndex(s => new { s.VenueId, s.SectionCode, s.RowCode, s.SeatNumber })
            .IsUnique();

        // Relationships
        builder.HasOne(s => s.Venue)
            .WithMany(v => v.Seats)
            .HasForeignKey(s => s.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Reservations)
            .WithOne(r => r.Seat)
            .HasForeignKey(r => r.SeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

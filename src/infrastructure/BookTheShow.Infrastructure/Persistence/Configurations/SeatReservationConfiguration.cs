using BookTheShow.Domain.Entities;
using BookTheShow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookTheShow.Infrastructure.Persistence.Configurations;

public class SeatReservationConfiguration : IEntityTypeConfiguration<SeatReservation>
{
    public void Configure(EntityTypeBuilder<SeatReservation> builder)
    {
        builder.ToTable("SeatReservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.SeatId)
            .IsRequired();

        builder.Property(r => r.EventId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.BookingId);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.Property(r => r.Price)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(r => r.Notes)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        // Indexes for performance-critical queries
        builder.HasIndex(r => r.SeatId);
        builder.HasIndex(r => r.EventId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.BookingId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.ExpiresAt);
        
        // Composite indexes for common queries
        builder.HasIndex(r => new { r.EventId, r.Status });
        builder.HasIndex(r => new { r.SeatId, r.EventId, r.Status });
        builder.HasIndex(r => new { r.UserId, r.Status });

        // Relationships
        builder.HasOne(r => r.Seat)
            .WithMany(s => s.Reservations)
            .HasForeignKey(r => r.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Event)
            .WithMany(e => e.SeatReservations)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Booking)
            .WithMany(b => b.SeatReservations)
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

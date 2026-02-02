using BookTheShow.Domain.Entities;
using BookTheShow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookTheShow.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.EventId)
            .IsRequired();

        builder.Property(b => b.BookingReference)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(b => b.BookingReference)
            .IsUnique();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(b => b.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(b => b.ServiceFee)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(b => b.TaxAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(b => b.GrandTotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(b => b.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(b => b.PaymentTransactionId)
            .HasMaxLength(100);

        builder.Property(b => b.PaymentCompletedAt);

        builder.Property(b => b.BookingExpiresAt)
            .IsRequired();

        builder.Property(b => b.CustomerEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.SpecialRequests)
            .HasMaxLength(1000);

        builder.Property(b => b.CancellationReason)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired();

        // Indexes for performance-critical queries
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.EventId);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.BookingExpiresAt);
        builder.HasIndex(b => b.PaymentTransactionId);
        builder.HasIndex(b => b.CustomerEmail);
        
        // Composite indexes for common queries
        builder.HasIndex(b => new { b.UserId, b.Status });
        builder.HasIndex(b => new { b.EventId, b.Status });
        builder.HasIndex(b => new { b.Status, b.BookingExpiresAt });
        builder.HasIndex(b => new { b.CreatedAt, b.Status });

        // Relationships
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Event)
            .WithMany(e => e.Bookings)
            .HasForeignKey(b => b.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.SeatReservations)
            .WithOne(r => r.Booking)
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

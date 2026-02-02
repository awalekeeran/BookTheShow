using BookTheShow.Domain.Entities;
using BookTheShow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookTheShow.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.VenueId)
            .IsRequired();

        builder.Property(e => e.StartDateTime)
            .IsRequired();

        builder.Property(e => e.EndDateTime)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.BasePrice)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(e => e.TotalSeats)
            .IsRequired();

        builder.Property(e => e.AvailableSeats)
            .IsRequired();

        builder.Property(e => e.BookedSeats)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        builder.Property(e => e.OrganizerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.OrganizerEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.PublishedAt);

        builder.Property(e => e.BookingOpenDateTime)
            .IsRequired();

        builder.Property(e => e.BookingCloseDateTime)
            .IsRequired();

        builder.Property(e => e.AllowWaitlist)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.MaxSeatsPerBooking)
            .IsRequired()
            .HasDefaultValue(10);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.VenueId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.StartDateTime);
        builder.HasIndex(e => e.IsPublished);
        
        // Composite index for event listing queries
        builder.HasIndex(e => new { e.Status, e.StartDateTime });
        builder.HasIndex(e => new { e.Category, e.StartDateTime });

        // Relationships
        builder.HasOne(e => e.Venue)
            .WithMany(v => v.Events)
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Bookings)
            .WithOne(b => b.Event)
            .HasForeignKey(b => b.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.SeatReservations)
            .WithOne(r => r.Event)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

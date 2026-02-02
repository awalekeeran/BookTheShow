namespace BookTheShow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Event> Events { get; }
    DbSet<Venue> Venues { get; }
    DbSet<Seat> Seats { get; }
    DbSet<SeatReservation> SeatReservations { get; }
    DbSet<Booking> Bookings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

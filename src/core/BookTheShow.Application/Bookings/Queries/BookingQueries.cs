using BookTheShow.Application.Bookings.DTOs;
using MediatR;

namespace BookTheShow.Application.Bookings.Queries;

/// <summary>
/// Query to get a booking by ID
/// </summary>
public class GetBookingByIdQuery : IRequest<BookingDto?>
{
    public Guid BookingId { get; set; }
}

/// <summary>
/// Query to get all bookings for a user
/// </summary>
public class GetUserBookingsQuery : IRequest<List<BookingDto>>
{
    public Guid UserId { get; set; }
}

/// <summary>
/// Query to get all bookings for an event
/// </summary>
public class GetEventBookingsQuery : IRequest<List<BookingDto>>
{
    public Guid EventId { get; set; }
}

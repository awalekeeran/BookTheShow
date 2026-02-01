using BookTheShow.Application.Bookings.DTOs;
using BookTheShow.Domain.Enums;
using MediatR;

namespace BookTheShow.Application.Bookings.Commands;

/// <summary>
/// Command to create a new booking
/// </summary>
public class CreateBookingCommand : IRequest<BookingResultDto>
{
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public List<Guid> SeatIds { get; set; } = new();
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
}

/// <summary>
/// Command to confirm a booking after payment
/// </summary>
public class ConfirmBookingCommand : IRequest<BookingResultDto>
{
    public Guid BookingId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentTransactionId { get; set; } = string.Empty;
}

/// <summary>
/// Command to cancel a booking
/// </summary>
public class CancelBookingCommand : IRequest<BookingResultDto>
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

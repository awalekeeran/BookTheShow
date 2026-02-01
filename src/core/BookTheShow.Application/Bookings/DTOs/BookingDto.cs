using BookTheShow.Domain.Enums;

namespace BookTheShow.Application.Bookings.DTOs;

/// <summary>
/// Booking Data Transfer Object
/// </summary>
public class BookingDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public BookingStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentMethodDisplay { get; set; }
    public string? PaymentTransactionId { get; set; }
    public DateTime? PaymentCompletedAt { get; set; }
    public DateTime BookingExpiresAt { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Related data
    public string EventTitle { get; set; } = string.Empty;
    public DateTime EventStartDateTime { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public List<SeatReservationDto> SeatReservations { get; set; } = new();
    public int SeatCount { get; set; }
}

/// <summary>
/// Seat Reservation Data Transfer Object
/// </summary>
public class SeatReservationDto
{
    public Guid Id { get; set; }
    public Guid SeatId { get; set; }
    public string SeatCode { get; set; } = string.Empty;
    public string SectionCode { get; set; } = string.Empty;
    public string RowCode { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public SeatType SeatType { get; set; }
    public string SeatTypeDisplay { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public SeatReservationStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Create Booking Request DTO
/// </summary>
public class CreateBookingRequestDto
{
    public Guid EventId { get; set; }
    public List<Guid> SeatIds { get; set; } = new();
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? SpecialRequests { get; set; }
}

/// <summary>
/// Confirm Booking Request DTO
/// </summary>
public class ConfirmBookingRequestDto
{
    public Guid BookingId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string PaymentTransactionId { get; set; } = string.Empty;
}

/// <summary>
/// Cancel Booking Request DTO
/// </summary>
public class CancelBookingRequestDto
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Booking Result DTO
/// </summary>
public class BookingResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public BookingDto? Booking { get; set; }
    public List<string> TicketCodes { get; set; } = new();
}

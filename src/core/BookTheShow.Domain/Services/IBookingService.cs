using BookTheShow.Domain.Entities;
using BookTheShow.Domain.Enums;

namespace BookTheShow.Domain.Services;

/// <summary>
/// Domain service for managing complex booking workflows
/// Orchestrates seat reservations, payments, and booking confirmations
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Initiates a booking process with seat selection
    /// </summary>
    /// <param name="eventId">The event to book</param>
    /// <param name="userId">User making the booking</param>
    /// <param name="seatIds">Selected seat IDs</param>
    /// <param name="customerEmail">Customer email</param>
    /// <param name="customerPhone">Customer phone</param>
    /// <returns>Booking initiation result</returns>
    Task<BookingInitiationResult> InitiateBookingAsync(
        Guid eventId,
        Guid userId,
        List<Guid> seatIds,
        string customerEmail,
        string customerPhone);

    /// <summary>
    /// Confirms a booking after successful payment
    /// </summary>
    /// <param name="bookingId">Booking to confirm</param>
    /// <param name="paymentMethod">Payment method used</param>
    /// <param name="transactionId">Payment transaction ID</param>
    /// <returns>Booking confirmation result</returns>
    Task<BookingConfirmationResult> ConfirmBookingAsync(
        Guid bookingId,
        PaymentMethod paymentMethod,
        string transactionId);

    /// <summary>
    /// Cancels a booking and releases seats
    /// </summary>
    /// <param name="bookingId">Booking to cancel</param>
    /// <param name="reason">Cancellation reason</param>
    /// <returns>Cancellation result</returns>
    Task<BookingCancellationResult> CancelBookingAsync(
        Guid bookingId,
        string reason);

    /// <summary>
    /// Processes expired bookings and releases seats
    /// </summary>
    /// <returns>Number of expired bookings processed</returns>
    Task<int> ProcessExpiredBookingsAsync();

    /// <summary>
    /// Gets real-time seat availability for an event
    /// </summary>
    /// <param name="eventId">Event ID</param>
    /// <returns>Seat availability map</returns>
    Task<SeatAvailabilityMap> GetSeatAvailabilityAsync(Guid eventId);

    /// <summary>
    /// Calculates pricing for selected seats including fees
    /// </summary>
    /// <param name="eventId">Event ID</param>
    /// <param name="seatIds">Selected seat IDs</param>
    /// <returns>Pricing breakdown</returns>
    Task<BookingPriceCalculation> CalculateBookingPriceAsync(
        Guid eventId,
        List<Guid> seatIds);

    /// <summary>
    /// Extends reservation timeout for an active booking
    /// </summary>
    /// <param name="bookingId">Booking ID</param>
    /// <param name="additionalMinutes">Additional time in minutes</param>
    /// <returns>Extension result</returns>
    Task<bool> ExtendBookingTimeoutAsync(Guid bookingId, int additionalMinutes);
}

/// <summary>
/// Result of booking initiation
/// </summary>
public class BookingInitiationResult
{
    public bool Success { get; set; }
    public Guid? BookingId { get; set; }
    public string? ErrorMessage { get; set; }
    public List<SeatReservation> ReservedSeats { get; set; } = new();
    public BookingPriceCalculation? PriceCalculation { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? LockToken { get; set; }
}

/// <summary>
/// Result of booking confirmation
/// </summary>
public class BookingConfirmationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Booking? ConfirmedBooking { get; set; }
    public string? BookingReference { get; set; }
    public List<string> TicketCodes { get; set; } = new();
}

/// <summary>
/// Result of booking cancellation
/// </summary>
public class BookingCancellationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal RefundAmount { get; set; }
    public string? RefundTransactionId { get; set; }
    public List<Guid> ReleasedSeatIds { get; set; } = new();
}

/// <summary>
/// Seat availability map for an event
/// </summary>
public class SeatAvailabilityMap
{
    public Guid EventId { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public int ReservedSeats { get; set; }
    public int BookedSeats { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<SeatAvailabilityInfo> Seats { get; set; } = new();
    public Dictionary<string, int> AvailabilityBySection { get; set; } = new();
    public Dictionary<SeatType, int> AvailabilityByType { get; set; } = new();
}

/// <summary>
/// Individual seat availability information
/// </summary>
public class SeatAvailabilityInfo
{
    public Guid SeatId { get; set; }
    public string SectionCode { get; set; } = string.Empty;
    public string RowCode { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public SeatType SeatType { get; set; }
    public SeatAvailabilityStatus Status { get; set; }
    public decimal Price { get; set; }
    public Guid? ReservedByUserId { get; set; }
    public DateTime? ReservationExpiresAt { get; set; }
}

/// <summary>
/// Seat availability status
/// </summary>
public enum SeatAvailabilityStatus
{
    Available = 1,
    Reserved = 2,
    Booked = 3,
    Blocked = 4,
    Inactive = 5
}

/// <summary>
/// Booking price calculation with breakdown
/// </summary>
public class BookingPriceCalculation
{
    public Guid EventId { get; set; }
    public List<Guid> SeatIds { get; set; } = new();
    public decimal BaseAmount { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal ServiceFeePercentage { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal GrandTotal { get; set; }
    public DateTime CalculatedAt { get; set; }
    public List<SeatPriceInfo> SeatPrices { get; set; } = new();
}

/// <summary>
/// Individual seat pricing information
/// </summary>
public class SeatPriceInfo
{
    public Guid SeatId { get; set; }
    public string SeatCode { get; set; } = string.Empty;
    public SeatType SeatType { get; set; }
    public decimal BasePrice { get; set; }
    public decimal PricingTier { get; set; }
    public decimal FinalPrice { get; set; }
}

using BookTheShow.Domain.Enums;

namespace BookTheShow.Domain.Entities;

/// <summary>
/// Represents a seat reservation within a specific event
/// Handles temporary seat locking during booking process
/// </summary>
public class SeatReservation : BaseEntity
{
    public Guid SeatId { get; private set; }
    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? BookingId { get; private set; } // Linked when booking is confirmed
    public SeatReservationStatus Status { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public decimal Price { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public Seat Seat { get; private set; } = null!;
    public Event Event { get; private set; } = null!;
    public User User { get; private set; } = null!;
    public Booking? Booking { get; private set; }

    // Private constructor for EF Core
    private SeatReservation() { }

    public static SeatReservation Create(
        Guid seatId,
        Guid eventId,
        Guid userId,
        decimal price,
        int reservationTimeoutMinutes = 15)
    {
        if (seatId == Guid.Empty)
            throw new ArgumentException("Valid seat ID is required", nameof(seatId));

        if (eventId == Guid.Empty)
            throw new ArgumentException("Valid event ID is required", nameof(eventId));

        if (userId == Guid.Empty)
            throw new ArgumentException("Valid user ID is required", nameof(userId));

        if (price <= 0)
            throw new ArgumentException("Price must be positive", nameof(price));

        var reservation = new SeatReservation
        {
            Id = Guid.NewGuid(),
            SeatId = seatId,
            EventId = eventId,
            UserId = userId,
            Status = SeatReservationStatus.Reserved,
            Price = price,
            ExpiresAt = DateTime.UtcNow.AddMinutes(reservationTimeoutMinutes),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return reservation;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt && Status == SeatReservationStatus.Reserved;
    }

    public void Confirm(Guid bookingId)
    {
        if (Status != SeatReservationStatus.Reserved)
            throw new InvalidOperationException("Only reserved seats can be confirmed");

        if (IsExpired())
            throw new InvalidOperationException("Cannot confirm expired reservation");

        BookingId = bookingId;
        Status = SeatReservationStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Release(string? reason = null)
    {
        if (Status == SeatReservationStatus.Confirmed)
            throw new InvalidOperationException("Cannot release confirmed reservation");

        Status = SeatReservationStatus.Released;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string? reason = null)
    {
        Status = SeatReservationStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkExpired()
    {
        if (Status == SeatReservationStatus.Reserved && IsExpired())
        {
            Status = SeatReservationStatus.Expired;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ExtendReservation(int additionalMinutes)
    {
        if (Status != SeatReservationStatus.Reserved)
            throw new InvalidOperationException("Can only extend active reservations");

        if (IsExpired())
            throw new InvalidOperationException("Cannot extend expired reservation");

        ExpiresAt = ExpiresAt.AddMinutes(additionalMinutes);
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Represents a complete booking transaction containing multiple seat reservations
/// </summary>
public class Booking : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid EventId { get; private set; }
    public string BookingReference { get; private set; } = string.Empty;
    public BookingStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal ServiceFee { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public string? PaymentTransactionId { get; private set; }
    public DateTime? PaymentCompletedAt { get; private set; }
    public DateTime BookingExpiresAt { get; private set; }
    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string? SpecialRequests { get; private set; }
    public string? CancellationReason { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Event Event { get; private set; } = null!;
    
    private readonly List<SeatReservation> _seatReservations = new();
    public IReadOnlyCollection<SeatReservation> SeatReservations => _seatReservations.AsReadOnly();

    // Private constructor for EF Core
    private Booking() { }

    public static Booking Create(
        Guid userId,
        Guid eventId,
        string customerEmail,
        string customerPhone,
        List<SeatReservation> seatReservations,
        decimal serviceFeePercentage = 0.10m,
        decimal taxPercentage = 0.08m,
        int bookingTimeoutMinutes = 20)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Valid user ID is required", nameof(userId));

        if (eventId == Guid.Empty)
            throw new ArgumentException("Valid event ID is required", nameof(eventId));

        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email is required", nameof(customerEmail));

        if (seatReservations == null || !seatReservations.Any())
            throw new ArgumentException("At least one seat reservation is required", nameof(seatReservations));

        // Verify all reservations belong to the same event and user
        if (seatReservations.Any(r => r.EventId != eventId || r.UserId != userId))
            throw new InvalidOperationException("All seat reservations must belong to the same event and user");

        // Calculate totals
        var totalAmount = seatReservations.Sum(r => r.Price);
        var serviceFee = Math.Round(totalAmount * serviceFeePercentage, 2);
        var taxAmount = Math.Round((totalAmount + serviceFee) * taxPercentage, 2);
        var grandTotal = totalAmount + serviceFee + taxAmount;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventId = eventId,
            BookingReference = GenerateBookingReference(),
            Status = BookingStatus.Pending,
            TotalAmount = totalAmount,
            ServiceFee = serviceFee,
            TaxAmount = taxAmount,
            GrandTotal = grandTotal,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone ?? string.Empty,
            BookingExpiresAt = DateTime.UtcNow.AddMinutes(bookingTimeoutMinutes),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add seat reservations
        foreach (var reservation in seatReservations)
        {
            booking._seatReservations.Add(reservation);
        }

        return booking;
    }

    private static string GenerateBookingReference()
    {
        // Generate format: BTS-YYYYMMDD-HHMMSS-XXXX (where XXXX is random)
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var random = new Random().Next(1000, 9999);
        return $"BTS-{timestamp}-{random}";
    }

    public void ProcessPayment(PaymentMethod paymentMethod, string transactionId)
    {
        if (Status != BookingStatus.Pending && Status != BookingStatus.PaymentPending)
            throw new InvalidOperationException("Can only process payment for pending bookings");

        if (IsExpired())
            throw new InvalidOperationException("Cannot process payment for expired booking");

        Status = BookingStatus.PaymentPending;
        PaymentMethod = paymentMethod;
        PaymentTransactionId = transactionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmPayment()
    {
        if (Status != BookingStatus.PaymentPending)
            throw new InvalidOperationException("Can only confirm payment for payment-pending bookings");

        Status = BookingStatus.Confirmed;
        PaymentCompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // Confirm all seat reservations
        foreach (var reservation in _seatReservations)
        {
            if (reservation.Status == SeatReservationStatus.Reserved)
            {
                reservation.Confirm(Id);
            }
        }
    }

    public void Cancel(string reason)
    {
        if (Status == BookingStatus.Confirmed)
        {
            // For confirmed bookings, this would typically trigger a refund process
            Status = BookingStatus.Cancelled;
        }
        else if (Status == BookingStatus.Pending || Status == BookingStatus.PaymentPending)
        {
            Status = BookingStatus.Cancelled;
        }
        else
        {
            throw new InvalidOperationException($"Cannot cancel booking with status {Status}");
        }

        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;

        // Release all seat reservations
        foreach (var reservation in _seatReservations)
        {
            if (reservation.Status == SeatReservationStatus.Reserved)
            {
                reservation.Cancel(reason);
            }
        }
    }

    public void MarkExpired()
    {
        if ((Status == BookingStatus.Pending || Status == BookingStatus.PaymentPending) && IsExpired())
        {
            Status = BookingStatus.Expired;
            UpdatedAt = DateTime.UtcNow;

            // Release all seat reservations
            foreach (var reservation in _seatReservations)
            {
                if (reservation.Status == SeatReservationStatus.Reserved)
                {
                    reservation.MarkExpired();
                }
            }
        }
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > BookingExpiresAt && 
               (Status == BookingStatus.Pending || Status == BookingStatus.PaymentPending);
    }

    public int GetSeatCount()
    {
        return _seatReservations.Count;
    }

    public List<string> GetSeatCodes()
    {
        return _seatReservations.Select(r => r.Seat?.GetSeatCode() ?? "Unknown").ToList();
    }

    public void AddSpecialRequests(string requests)
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Can only add special requests to pending bookings");

        SpecialRequests = requests;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanBeCancelled()
    {
        // Business rule: Can cancel up to 24 hours before event or if booking is not confirmed
        if (Status != BookingStatus.Confirmed)
            return true;

        if (Event?.StartDateTime != null)
        {
            var cancellationDeadline = Event.StartDateTime.AddHours(-24);
            return DateTime.UtcNow < cancellationDeadline;
        }

        return false;
    }
}

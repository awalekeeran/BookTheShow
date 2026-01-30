namespace BookTheShow.Domain.Enums;

/// <summary>
/// Types of venues for event hosting
/// </summary>
public enum VenueType
{
    /// <summary>
    /// Concert halls and music venues
    /// </summary>
    ConcertHall = 1,

    /// <summary>
    /// Sports stadiums and arenas
    /// </summary>
    Stadium = 2,

    /// <summary>
    /// Theater and performing arts venues
    /// </summary>
    Theater = 3,

    /// <summary>
    /// Convention centers
    /// </summary>
    ConventionCenter = 4,

    /// <summary>
    /// Outdoor venues and parks
    /// </summary>
    Outdoor = 5,

    /// <summary>
    /// Multi-purpose arenas
    /// </summary>
    Arena = 6,

    /// <summary>
    /// Clubs and bars
    /// </summary>
    Club = 7,

    /// <summary>
    /// Exhibition halls
    /// </summary>
    ExhibitionHall = 8,

    /// <summary>
    /// Auditoriums
    /// </summary>
    Auditorium = 9,

    /// <summary>
    /// Other venue types
    /// </summary>
    Other = 10
}

/// <summary>
/// Types of seats within a venue
/// </summary>
public enum SeatType
{
    /// <summary>
    /// Regular seating
    /// </summary>
    Regular = 1,

    /// <summary>
    /// VIP premium seating
    /// </summary>
    VIP = 2,

    /// <summary>
    /// Wheelchair accessible seating
    /// </summary>
    Accessible = 3,

    /// <summary>
    /// Standing room only
    /// </summary>
    Standing = 4,

    /// <summary>
    /// Box seats
    /// </summary>
    Box = 5,

    /// <summary>
    /// Balcony seating
    /// </summary>
    Balcony = 6,

    /// <summary>
    /// Ground floor/orchestra seating
    /// </summary>
    Orchestra = 7,

    /// <summary>
    /// Mezzanine level seating
    /// </summary>
    Mezzanine = 8
}

/// <summary>
/// Status of seat reservations
/// </summary>
public enum SeatReservationStatus
{
    /// <summary>
    /// Seat is temporarily reserved (15-20 minutes)
    /// </summary>
    Reserved = 1,

    /// <summary>
    /// Reservation confirmed with payment
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Reservation has expired
    /// </summary>
    Expired = 3,

    /// <summary>
    /// Reservation manually released
    /// </summary>
    Released = 4,

    /// <summary>
    /// Reservation cancelled
    /// </summary>
    Cancelled = 5
}

/// <summary>
/// Status of booking transactions
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking initiated but not completed
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Payment processing
    /// </summary>
    PaymentPending = 2,

    /// <summary>
    /// Booking confirmed and paid
    /// </summary>
    Confirmed = 3,

    /// <summary>
    /// Booking cancelled by user
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Booking expired due to timeout
    /// </summary>
    Expired = 5,

    /// <summary>
    /// Booking failed during processing
    /// </summary>
    Failed = 6,

    /// <summary>
    /// Booking refunded
    /// </summary>
    Refunded = 7
}

/// <summary>
/// Payment methods available for bookings
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Credit card payment
    /// </summary>
    CreditCard = 1,

    /// <summary>
    /// Debit card payment
    /// </summary>
    DebitCard = 2,

    /// <summary>
    /// Digital wallet (PayPal, Apple Pay, etc.)
    /// </summary>
    DigitalWallet = 3,

    /// <summary>
    /// Bank transfer
    /// </summary>
    BankTransfer = 4,

    /// <summary>
    /// Cash payment (for offline bookings)
    /// </summary>
    Cash = 5,

    /// <summary>
    /// Gift card or voucher
    /// </summary>
    GiftCard = 6
}

using BookTheShow.Domain.Enums;
using BookTheShow.Domain.Services;

namespace BookTheShow.Domain.Extensions;

/// <summary>
/// Extension methods for EventStatus enum
/// </summary>
public static class EventStatusExtensions
{
    /// <summary>
    /// Gets the display name for the event status
    /// </summary>
    public static string GetDisplayName(this EventStatus status)
    {
        return status switch
        {
            EventStatus.Draft => "Draft",
            EventStatus.Live => "Live",
            EventStatus.Cancelled => "Cancelled",
            EventStatus.Completed => "Completed",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Gets the CSS class name for styling the status
    /// </summary>
    public static string GetCssClass(this EventStatus status)
    {
        return status switch
        {
            EventStatus.Draft => "status-draft",
            EventStatus.Live => "status-live",
            EventStatus.Cancelled => "status-cancelled",
            EventStatus.Completed => "status-completed",
            _ => "status-unknown"
        };
    }

    /// <summary>
    /// Checks if the event status allows editing
    /// </summary>
    public static bool CanEdit(this EventStatus status)
    {
        return status is EventStatus.Draft or EventStatus.Live;
    }

    /// <summary>
    /// Checks if the event status allows booking
    /// </summary>
    public static bool CanBook(this EventStatus status)
    {
        return status == EventStatus.Live;
    }

    /// <summary>
    /// Gets all valid transition states from current status
    /// </summary>
    public static EventStatus[] GetValidTransitions(this EventStatus currentStatus)
    {
        return currentStatus switch
        {
            EventStatus.Draft => new[] { EventStatus.Live, EventStatus.Cancelled },
            EventStatus.Live => new[] { EventStatus.Completed, EventStatus.Cancelled },
            EventStatus.Cancelled => Array.Empty<EventStatus>(), // Terminal state
            EventStatus.Completed => Array.Empty<EventStatus>(), // Terminal state
            _ => Array.Empty<EventStatus>()
        };
    }
}

/// <summary>
/// Extension methods for EventCategory enum
/// </summary>
public static class EventCategoryExtensions
{
    /// <summary>
    /// Gets the display name for the event category
    /// </summary>
    public static string GetDisplayName(this EventCategory category)
    {
        return category switch
        {
            EventCategory.Concert => "Concert",
            EventCategory.Sports => "Sports",
            EventCategory.Theater => "Theater",
            EventCategory.Comedy => "Comedy",
            EventCategory.Conference => "Conference",
            EventCategory.Workshop => "Workshop",
            EventCategory.Festival => "Festival",
            EventCategory.Exhibition => "Exhibition",
            EventCategory.Dance => "Dance",
            EventCategory.Movie => "Movie",
            EventCategory.Other => "Other",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Gets the icon class name for the category (Font Awesome or similar)
    /// </summary>
    public static string GetIconClass(this EventCategory category)
    {
        return category switch
        {
            EventCategory.Concert => "fas fa-music",
            EventCategory.Sports => "fas fa-futbol",
            EventCategory.Theater => "fas fa-theater-masks",
            EventCategory.Comedy => "fas fa-laugh",
            EventCategory.Conference => "fas fa-users",
            EventCategory.Workshop => "fas fa-tools",
            EventCategory.Festival => "fas fa-calendar-alt",
            EventCategory.Exhibition => "fas fa-palette",
            EventCategory.Dance => "fas fa-running",
            EventCategory.Movie => "fas fa-film",
            EventCategory.Other => "fas fa-ticket-alt",
            _ => "fas fa-question"
        };
    }

    /// <summary>
    /// Gets the primary color hex code for the category
    /// </summary>
    public static string GetPrimaryColor(this EventCategory category)
    {
        return category switch
        {
            EventCategory.Concert => "#E91E63", // Pink
            EventCategory.Sports => "#4CAF50", // Green
            EventCategory.Theater => "#9C27B0", // Purple
            EventCategory.Comedy => "#FF9800", // Orange
            EventCategory.Conference => "#2196F3", // Blue
            EventCategory.Workshop => "#795548", // Brown
            EventCategory.Festival => "#FF5722", // Deep Orange
            EventCategory.Exhibition => "#607D8B", // Blue Grey
            EventCategory.Dance => "#F44336", // Red
            EventCategory.Movie => "#3F51B5", // Indigo
            EventCategory.Other => "#9E9E9E", // Grey
            _ => "#000000" // Black
        };
    }

    /// <summary>
    /// Gets the description for the category
    /// </summary>
    public static string GetDescription(this EventCategory category)
    {
        return category switch
        {
            EventCategory.Concert => "Live music performances and concerts",
            EventCategory.Sports => "Sporting events and athletic competitions",
            EventCategory.Theater => "Theatrical plays and dramatic performances",
            EventCategory.Comedy => "Comedy shows and stand-up performances",
            EventCategory.Conference => "Professional conferences and business events",
            EventCategory.Workshop => "Educational workshops and training sessions",
            EventCategory.Festival => "Cultural festivals and celebrations",
            EventCategory.Exhibition => "Art exhibitions and gallery shows",
            EventCategory.Dance => "Dance performances and recitals",
            EventCategory.Movie => "Movie screenings and film premieres",
            EventCategory.Other => "Other miscellaneous events",
            _ => "Unknown category"
        };
    }

    /// <summary>
    /// Gets typical duration in hours for the category
    /// </summary>
    public static int GetTypicalDurationHours(this EventCategory category)
    {
        return category switch
        {
            EventCategory.Concert => 3,
            EventCategory.Sports => 2,
            EventCategory.Theater => 2,
            EventCategory.Comedy => 2,
            EventCategory.Conference => 8,
            EventCategory.Workshop => 4,
            EventCategory.Festival => 12,
            EventCategory.Exhibition => 4,
            EventCategory.Dance => 2,
            EventCategory.Movie => 2,
            EventCategory.Other => 3,
            _ => 2
        };
    }

    /// <summary>
    /// Gets all categories suitable for families with children
    /// </summary>
    public static EventCategory[] GetFamilyFriendlyCategories()
    {
        return new[]
        {
            EventCategory.Concert,
            EventCategory.Sports,
            EventCategory.Theater,
            EventCategory.Festival,
            EventCategory.Exhibition,
            EventCategory.Dance,
            EventCategory.Movie
        };
    }

    /// <summary>
    /// Checks if the category is typically family-friendly
    /// </summary>
    public static bool IsFamilyFriendly(this EventCategory category)
    {
        return GetFamilyFriendlyCategories().Contains(category);
    }

    // VenueType Extensions
    public static string GetDisplayName(this VenueType venueType)
    {
        return venueType switch
        {
            VenueType.ConcertHall => "Concert Hall",
            VenueType.Stadium => "Stadium",
            VenueType.Theater => "Theater",
            VenueType.ConventionCenter => "Convention Center",
            VenueType.Outdoor => "Outdoor Venue",
            VenueType.Arena => "Arena",
            VenueType.Club => "Club",
            VenueType.ExhibitionHall => "Exhibition Hall",
            VenueType.Auditorium => "Auditorium",
            VenueType.Other => "Other",
            _ => venueType.ToString()
        };
    }

    public static string GetIconClass(this VenueType venueType)
    {
        return venueType switch
        {
            VenueType.ConcertHall => "fas fa-music",
            VenueType.Stadium => "fas fa-futbol",
            VenueType.Theater => "fas fa-theater-masks",
            VenueType.ConventionCenter => "fas fa-building",
            VenueType.Outdoor => "fas fa-tree",
            VenueType.Arena => "fas fa-dumbbell",
            VenueType.Club => "fas fa-glass-cheers",
            VenueType.ExhibitionHall => "fas fa-palette",
            VenueType.Auditorium => "fas fa-podium",
            _ => "fas fa-map-marker-alt"
        };
    }

    // SeatType Extensions
    public static string GetDisplayName(this SeatType seatType)
    {
        return seatType switch
        {
            SeatType.Regular => "Regular",
            SeatType.VIP => "VIP",
            SeatType.Accessible => "Accessible",
            SeatType.Standing => "Standing",
            SeatType.Box => "Box",
            SeatType.Balcony => "Balcony",
            SeatType.Orchestra => "Orchestra",
            SeatType.Mezzanine => "Mezzanine",
            _ => seatType.ToString()
        };
    }

    public static string GetCssClass(this SeatType seatType)
    {
        return seatType switch
        {
            SeatType.Regular => "seat-regular",
            SeatType.VIP => "seat-vip",
            SeatType.Accessible => "seat-accessible",
            SeatType.Standing => "seat-standing",
            SeatType.Box => "seat-box",
            SeatType.Balcony => "seat-balcony",
            SeatType.Orchestra => "seat-orchestra",
            SeatType.Mezzanine => "seat-mezzanine",
            _ => "seat-default"
        };
    }

    public static string GetPrimaryColor(this SeatType seatType)
    {
        return seatType switch
        {
            SeatType.Regular => "#6B73FF",
            SeatType.VIP => "#FFD700",
            SeatType.Accessible => "#28A745",
            SeatType.Standing => "#17A2B8",
            SeatType.Box => "#DC3545",
            SeatType.Balcony => "#6610F2",
            SeatType.Orchestra => "#E83E8C",
            SeatType.Mezzanine => "#FD7E14",
            _ => "#6C757D"
        };
    }

    // BookingStatus Extensions
    public static string GetDisplayName(this BookingStatus status)
    {
        return status switch
        {
            BookingStatus.Pending => "Pending",
            BookingStatus.PaymentPending => "Payment Pending",
            BookingStatus.Confirmed => "Confirmed",
            BookingStatus.Cancelled => "Cancelled",
            BookingStatus.Expired => "Expired",
            BookingStatus.Failed => "Failed",
            BookingStatus.Refunded => "Refunded",
            _ => status.ToString()
        };
    }

    public static string GetCssClass(this BookingStatus status)
    {
        return status switch
        {
            BookingStatus.Pending => "booking-pending",
            BookingStatus.PaymentPending => "booking-payment-pending",
            BookingStatus.Confirmed => "booking-confirmed",
            BookingStatus.Cancelled => "booking-cancelled",
            BookingStatus.Expired => "booking-expired",
            BookingStatus.Failed => "booking-failed",
            BookingStatus.Refunded => "booking-refunded",
            _ => "booking-unknown"
        };
    }

    public static string GetPrimaryColor(this BookingStatus status)
    {
        return status switch
        {
            BookingStatus.Pending => "#FFC107",
            BookingStatus.PaymentPending => "#17A2B8",
            BookingStatus.Confirmed => "#28A745",
            BookingStatus.Cancelled => "#6C757D",
            BookingStatus.Expired => "#DC3545",
            BookingStatus.Failed => "#DC3545",
            BookingStatus.Refunded => "#6610F2",
            _ => "#6C757D"
        };
    }

    public static bool IsActive(this BookingStatus status)
    {
        return status == BookingStatus.Pending || 
               status == BookingStatus.PaymentPending || 
               status == BookingStatus.Confirmed;
    }

    public static bool CanBeCancelled(this BookingStatus status)
    {
        return status == BookingStatus.Pending || 
               status == BookingStatus.PaymentPending || 
               status == BookingStatus.Confirmed;
    }

    // SeatReservationStatus Extensions
    public static string GetDisplayName(this SeatReservationStatus status)
    {
        return status switch
        {
            SeatReservationStatus.Reserved => "Reserved",
            SeatReservationStatus.Confirmed => "Confirmed",
            SeatReservationStatus.Expired => "Expired",
            SeatReservationStatus.Released => "Released",
            SeatReservationStatus.Cancelled => "Cancelled",
            _ => status.ToString()
        };
    }

    public static bool IsActive(this SeatReservationStatus status)
    {
        return status == SeatReservationStatus.Reserved || 
               status == SeatReservationStatus.Confirmed;
    }

    // SeatAvailabilityStatus Extensions
    public static string GetDisplayName(this SeatAvailabilityStatus status)
    {
        return status switch
        {
            SeatAvailabilityStatus.Available => "Available",
            SeatAvailabilityStatus.Reserved => "Reserved",
            SeatAvailabilityStatus.Booked => "Booked",
            SeatAvailabilityStatus.Blocked => "Blocked",
            SeatAvailabilityStatus.Inactive => "Inactive",
            _ => status.ToString()
        };
    }

    public static string GetCssClass(this SeatAvailabilityStatus status)
    {
        return status switch
        {
            SeatAvailabilityStatus.Available => "seat-available",
            SeatAvailabilityStatus.Reserved => "seat-reserved",
            SeatAvailabilityStatus.Booked => "seat-booked",
            SeatAvailabilityStatus.Blocked => "seat-blocked",
            SeatAvailabilityStatus.Inactive => "seat-inactive",
            _ => "seat-unknown"
        };
    }

    public static string GetPrimaryColor(this SeatAvailabilityStatus status)
    {
        return status switch
        {
            SeatAvailabilityStatus.Available => "#28A745",
            SeatAvailabilityStatus.Reserved => "#FFC107",
            SeatAvailabilityStatus.Booked => "#DC3545",
            SeatAvailabilityStatus.Blocked => "#6C757D",
            SeatAvailabilityStatus.Inactive => "#E9ECEF",
            _ => "#6C757D"
        };
    }

    // PaymentMethod Extensions
    public static string GetDisplayName(this PaymentMethod paymentMethod)
    {
        return paymentMethod switch
        {
            PaymentMethod.CreditCard => "Credit Card",
            PaymentMethod.DebitCard => "Debit Card",
            PaymentMethod.DigitalWallet => "Digital Wallet",
            PaymentMethod.BankTransfer => "Bank Transfer",
            PaymentMethod.Cash => "Cash",
            PaymentMethod.GiftCard => "Gift Card",
            _ => paymentMethod.ToString()
        };
    }

    public static string GetIconClass(this PaymentMethod paymentMethod)
    {
        return paymentMethod switch
        {
            PaymentMethod.CreditCard => "fas fa-credit-card",
            PaymentMethod.DebitCard => "fas fa-credit-card",
            PaymentMethod.DigitalWallet => "fas fa-wallet",
            PaymentMethod.BankTransfer => "fas fa-university",
            PaymentMethod.Cash => "fas fa-money-bill-wave",
            PaymentMethod.GiftCard => "fas fa-gift",
            _ => "fas fa-money-check-alt"
        };
    }
}

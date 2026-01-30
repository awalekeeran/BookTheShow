using BookTheShow.Domain.Enums;

namespace BookTheShow.Domain.Entities;

/// <summary>
/// Represents a venue where events take place
/// </summary>
public class Venue : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public int TotalCapacity { get; private set; }
    public VenueType Type { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    // Navigation properties
    private readonly List<Seat> _seats = new();
    public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();

    private readonly List<Event> _events = new();
    public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

    // Private constructor for EF Core
    private Venue() { }

    public static Venue Create(
        string name,
        string address,
        string city,
        string country,
        VenueType type,
        string description = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Venue name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required", nameof(city));

        var venue = new Venue
        {
            Id = Guid.NewGuid(),
            Name = name,
            Address = address ?? string.Empty,
            City = city,
            Country = country ?? "Unknown",
            Type = type,
            Description = description ?? string.Empty,
            TotalCapacity = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return venue;
    }

    public void AddSeat(Seat seat)
    {
        if (seat == null)
            throw new ArgumentNullException(nameof(seat));

        if (_seats.Any(s => s.SectionCode == seat.SectionCode && 
                           s.RowCode == seat.RowCode && 
                           s.SeatNumber == seat.SeatNumber))
            throw new InvalidOperationException("Seat already exists in this venue");

        _seats.Add(seat);
        TotalCapacity = _seats.Count;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveSeat(Guid seatId)
    {
        var seat = _seats.FirstOrDefault(s => s.Id == seatId);
        if (seat != null)
        {
            _seats.Remove(seat);
            TotalCapacity = _seats.Count;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public List<Seat> GetAvailableSeats(Guid eventId)
    {
        return _seats.Where(s => s.IsAvailableForEvent(eventId)).ToList();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Represents individual seats within a venue
/// </summary>
public class Seat : BaseEntity
{
    public Guid VenueId { get; private set; }
    public string SectionCode { get; private set; } = string.Empty; // e.g., "A", "VIP", "GENERAL"
    public string RowCode { get; private set; } = string.Empty;     // e.g., "1", "AA", "FRONT"
    public int SeatNumber { get; private set; }                     // e.g., 15, 22
    public SeatType Type { get; private set; }
    public decimal PricingTier { get; private set; } = 1.0m;        // Multiplier for base price
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    // Navigation properties
    public Venue Venue { get; private set; } = null!;
    
    private readonly List<SeatReservation> _reservations = new();
    public IReadOnlyCollection<SeatReservation> Reservations => _reservations.AsReadOnly();

    // Private constructor for EF Core
    private Seat() { }

    public static Seat Create(
        Guid venueId,
        string sectionCode,
        string rowCode,
        int seatNumber,
        SeatType type,
        decimal pricingTier = 1.0m)
    {
        if (venueId == Guid.Empty)
            throw new ArgumentException("Valid venue ID is required", nameof(venueId));

        if (string.IsNullOrWhiteSpace(sectionCode))
            throw new ArgumentException("Section code is required", nameof(sectionCode));

        if (seatNumber <= 0)
            throw new ArgumentException("Seat number must be positive", nameof(seatNumber));

        if (pricingTier <= 0)
            throw new ArgumentException("Pricing tier must be positive", nameof(pricingTier));

        var seat = new Seat
        {
            Id = Guid.NewGuid(),
            VenueId = venueId,
            SectionCode = sectionCode.ToUpperInvariant(),
            RowCode = rowCode?.ToUpperInvariant() ?? string.Empty,
            SeatNumber = seatNumber,
            Type = type,
            PricingTier = pricingTier,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return seat;
    }

    public string GetSeatCode()
    {
        return string.IsNullOrEmpty(RowCode) 
            ? $"{SectionCode}-{SeatNumber}"
            : $"{SectionCode}-{RowCode}-{SeatNumber}";
    }

    public bool IsAvailableForEvent(Guid eventId)
    {
        if (!IsActive) return false;

        return !_reservations.Any(r => r.EventId == eventId && 
                                      r.Status != SeatReservationStatus.Released &&
                                      r.Status != SeatReservationStatus.Expired);
    }

    public bool IsReservedForEvent(Guid eventId)
    {
        return _reservations.Any(r => r.EventId == eventId && 
                                    (r.Status == SeatReservationStatus.Reserved || 
                                     r.Status == SeatReservationStatus.Confirmed));
    }

    public SeatReservation? GetActiveReservation(Guid eventId)
    {
        return _reservations.FirstOrDefault(r => r.EventId == eventId && 
                                                r.Status == SeatReservationStatus.Reserved &&
                                                r.ExpiresAt > DateTime.UtcNow);
    }

    public void Deactivate(string? reason = null)
    {
        IsActive = false;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }
}

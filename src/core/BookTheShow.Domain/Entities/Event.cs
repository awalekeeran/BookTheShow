using BookTheShow.Domain.Enums;

namespace BookTheShow.Domain.Entities;

public class Event : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid VenueId { get; private set; }
    public DateTime StartDateTime { get; private set; }
    public DateTime EndDateTime { get; private set; }
    public EventStatus Status { get; private set; }
    public EventCategory Category { get; private set; }
    public decimal BasePrice { get; private set; }
    public int TotalSeats { get; private set; }
    public int AvailableSeats { get; private set; }
    public int BookedSeats { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string OrganizerName { get; private set; } = string.Empty;
    public string OrganizerEmail { get; private set; } = string.Empty;
    public bool IsPublished { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime BookingOpenDateTime { get; private set; }
    public DateTime BookingCloseDateTime { get; private set; }
    public bool AllowWaitlist { get; private set; }
    public int MaxSeatsPerBooking { get; private set; }

    // Navigation properties
    public Venue Venue { get; private set; } = null!;

    private readonly List<Booking> _bookings = new();
    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    private readonly List<SeatReservation> _seatReservations = new();
    public IReadOnlyCollection<SeatReservation> SeatReservations => _seatReservations.AsReadOnly();

    // Private constructor for EF Core
    private Event() { }

    // Factory method - Domain-driven design pattern
    public static Event Create(
        string title,
        string description,
        Guid venueId,
        DateTime startDateTime,
        DateTime endDateTime,
        EventCategory category,
        decimal basePrice,
        int totalSeats,
        string organizerName,
        string organizerEmail,
        DateTime? bookingOpenDateTime = null,
        DateTime? bookingCloseDateTime = null,
        int maxSeatsPerBooking = 10)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Event title is required", nameof(title));

        if (venueId == Guid.Empty)
            throw new ArgumentException("Valid venue ID is required", nameof(venueId));

        if (startDateTime <= DateTime.UtcNow.AddHours(1))
            throw new ArgumentException("Event start time must be at least 1 hour in the future", nameof(startDateTime));

        if (endDateTime <= startDateTime)
            throw new ArgumentException("Event end time must be after start time", nameof(endDateTime));

        if (basePrice < 0)
            throw new ArgumentException("Base price cannot be negative", nameof(basePrice));

        if (totalSeats <= 0)
            throw new ArgumentException("Total seats must be greater than 0", nameof(totalSeats));

        if (string.IsNullOrWhiteSpace(organizerName))
            throw new ArgumentException("Organizer name is required", nameof(organizerName));

        if (maxSeatsPerBooking <= 0 || maxSeatsPerBooking > 20)
            throw new ArgumentException("Max seats per booking must be between 1 and 20", nameof(maxSeatsPerBooking));

        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description ?? string.Empty,
            VenueId = venueId,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            Status = EventStatus.Draft,
            Category = category,
            BasePrice = basePrice,
            TotalSeats = totalSeats,
            AvailableSeats = totalSeats,
            BookedSeats = 0,
            OrganizerName = organizerName,
            OrganizerEmail = organizerEmail ?? string.Empty,
            IsPublished = false,
            BookingOpenDateTime = bookingOpenDateTime ?? startDateTime.AddDays(-7), // Default: 7 days before event
            BookingCloseDateTime = bookingCloseDateTime ?? startDateTime.AddHours(-1), // Default: 1 hour before event
            AllowWaitlist = true,
            MaxSeatsPerBooking = maxSeatsPerBooking,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return eventEntity;
    }

    // Business methods
    public void UpdateDetails(
        string title,
        string description,
        EventCategory category,
        string imageUrl = "")
    {
        if (Status == EventStatus.Completed || Status == EventStatus.Cancelled)
            throw new InvalidOperationException("Cannot update details of completed or cancelled events");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Event title is required", nameof(title));

        Title = title;
        Description = description ?? string.Empty;
        Category = category;
        ImageUrl = imageUrl ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDateTime(DateTime startDateTime, DateTime endDateTime)
    {
        if (Status == EventStatus.Live && BookedSeats > 0)
            throw new InvalidOperationException("Cannot change date/time for live events with bookings");

        if (startDateTime <= DateTime.UtcNow.AddHours(1))
            throw new ArgumentException("Event start time must be at least 1 hour in the future", nameof(startDateTime));

        if (endDateTime <= startDateTime)
            throw new ArgumentException("Event end time must be after start time", nameof(endDateTime));

        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePricing(decimal basePrice)
    {
        if (Status == EventStatus.Live && BookedSeats > 0)
            throw new InvalidOperationException("Cannot change pricing for live events with existing bookings");

        if (basePrice < 0)
            throw new ArgumentException("Base price cannot be negative", nameof(basePrice));

        BasePrice = basePrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status != EventStatus.Draft)
            throw new InvalidOperationException("Only draft events can be published");

        if (string.IsNullOrWhiteSpace(Title) || VenueId == Guid.Empty)
            throw new InvalidOperationException("Event must have title and venue before publishing");

        Status = EventStatus.Live;
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason = "")
    {
        if (Status == EventStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed events");

        if (Status == EventStatus.Cancelled)
            throw new InvalidOperationException("Event is already cancelled");

        Status = EventStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        // Note: In a real system, you'd raise a domain event here for refunds
        // DomainEvents.Raise(new EventCancelledEvent(Id, reason));
    }

    public void Complete()
    {
        if (Status != EventStatus.Live)
            throw new InvalidOperationException("Only live events can be completed");

        if (DateTime.UtcNow < EndDateTime)
            throw new InvalidOperationException("Cannot complete event before end time");

        Status = EventStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool ReserveSeats(int seatCount)
    {
        if (Status != EventStatus.Live)
            return false;

        if (seatCount <= 0 || seatCount > MaxSeatsPerBooking)
            return false;

        if (AvailableSeats < seatCount)
            return false;

        if (DateTime.UtcNow < BookingOpenDateTime || DateTime.UtcNow > BookingCloseDateTime)
            return false;

        AvailableSeats -= seatCount;
        BookedSeats += seatCount;
        UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public void ReleaseSeats(int seatCount)
    {
        if (seatCount <= 0 || seatCount > BookedSeats)
            throw new ArgumentException("Invalid seat count to release");

        AvailableSeats += seatCount;
        BookedSeats -= seatCount;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsBookingOpen()
    {
        return Status == EventStatus.Live 
               && DateTime.UtcNow >= BookingOpenDateTime 
               && DateTime.UtcNow <= BookingCloseDateTime
               && AvailableSeats > 0;
    }

    public bool CanAcceptWaitlist()
    {
        return Status == EventStatus.Live 
               && AllowWaitlist 
               && AvailableSeats == 0
               && DateTime.UtcNow <= BookingCloseDateTime;
    }

    public string GetDisplayStatus()
    {
        return Status switch
        {
            EventStatus.Draft => "Draft",
            EventStatus.Live when !IsBookingOpen() && AvailableSeats == 0 => "Sold Out",
            EventStatus.Live when DateTime.UtcNow < BookingOpenDateTime => "Coming Soon",
            EventStatus.Live when DateTime.UtcNow > BookingCloseDateTime => "Booking Closed",
            EventStatus.Live => "Available",
            EventStatus.Cancelled => "Cancelled",
            EventStatus.Completed => "Completed",
            _ => "Unknown"
        };
    }

    public decimal GetCurrentPrice()
    {
        // Future: Implement dynamic pricing based on demand, time to event, etc.
        // For now, return base price
        return BasePrice;
    }

    public int GetBookingProgress()
    {
        if (TotalSeats == 0) return 0;
        return (int)Math.Round((double)BookedSeats / TotalSeats * 100);
    }

    // Seat Management Methods
    public List<Seat> GetAvailableSeats()
    {
        return Venue?.GetAvailableSeats(Id).ToList() ?? new List<Seat>();
    }

    public bool CanReserveSeats(List<Guid> seatIds)
    {
        if (Status != EventStatus.Live)
            return false;

        if (!IsBookingOpen())
            return false;

        var availableSeats = GetAvailableSeats();
        return seatIds.All(seatId => availableSeats.Any(s => s.Id == seatId));
    }

    public List<SeatReservation> ReserveSeats(List<Guid> seatIds, Guid userId, int reservationTimeoutMinutes = 15)
    {
        if (!CanReserveSeats(seatIds))
            throw new InvalidOperationException("Cannot reserve requested seats");

        if (seatIds.Count > MaxSeatsPerBooking)
            throw new InvalidOperationException($"Cannot reserve more than {MaxSeatsPerBooking} seats per booking");

        var reservations = new List<SeatReservation>();
        var availableSeats = GetAvailableSeats();

        foreach (var seatId in seatIds)
        {
            var seat = availableSeats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null)
                throw new InvalidOperationException($"Seat {seatId} is not available");

            // Calculate dynamic pricing
            var seatPrice = BasePrice * seat.PricingTier;
            
            var reservation = SeatReservation.Create(
                seatId, Id, userId, seatPrice, reservationTimeoutMinutes);
            
            reservations.Add(reservation);
        }

        // Update available seats count
        AvailableSeats -= reservations.Count;
        UpdatedAt = DateTime.UtcNow;

        return reservations;
    }

    public void ConfirmBooking(Booking booking)
    {
        if (booking.EventId != Id)
            throw new InvalidOperationException("Booking does not belong to this event");

        BookedSeats += booking.GetSeatCount();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReleaseSeatReservations(List<SeatReservation> reservations)
    {
        var eventReservations = reservations.Where(r => r.EventId == Id).ToList();
        AvailableSeats += eventReservations.Count;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal CalculateSeatPrice(Seat seat)
    {
        return BasePrice * seat.PricingTier;
    }

    public bool IsHighDemandEvent()
    {
        // High demand if more than 80% booked
        return (double)BookedSeats / TotalSeats > 0.8;
    }
}

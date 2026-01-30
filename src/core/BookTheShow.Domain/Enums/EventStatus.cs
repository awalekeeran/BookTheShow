namespace BookTheShow.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of an event
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// Event created but not yet published for public booking
    /// </summary>
    Draft = 1,
    
    /// <summary>
    /// Event is published and actively accepting bookings
    /// </summary>
    Live = 2,
    
    /// <summary>
    /// Event has been cancelled (refunds may be processed)
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Event has finished/completed successfully
    /// </summary>
    Completed = 4
}

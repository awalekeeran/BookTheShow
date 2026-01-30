namespace BookTheShow.Domain.Services;

/// <summary>
/// Service for managing distributed locks during seat reservations
/// Critical for high-volume booking scenarios like Ticketmaster
/// </summary>
public interface IDistributedLockService
{
    /// <summary>
    /// Acquires a distributed lock for seat reservation
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <param name="seatIds">List of seat IDs to lock</param>
    /// <param name="userId">User attempting to reserve</param>
    /// <param name="timeoutMs">Lock timeout in milliseconds</param>
    /// <returns>Lock acquisition result with lock token</returns>
    Task<LockAcquisitionResult> AcquireSeatLockAsync(
        Guid eventId, 
        List<Guid> seatIds, 
        Guid userId, 
        int timeoutMs = 15000);

    /// <summary>
    /// Releases a distributed lock
    /// </summary>
    /// <param name="lockToken">Token from successful lock acquisition</param>
    /// <returns>True if successfully released</returns>
    Task<bool> ReleaseLockAsync(string lockToken);

    /// <summary>
    /// Checks if seats are currently locked by another user
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <param name="seatIds">Seat IDs to check</param>
    /// <param name="excludeUserId">User ID to exclude from lock check</param>
    /// <returns>Dictionary of seat lock status</returns>
    Task<Dictionary<Guid, SeatLockInfo>> GetSeatLockStatusAsync(
        Guid eventId, 
        List<Guid> seatIds, 
        Guid? excludeUserId = null);

    /// <summary>
    /// Extends an existing lock duration
    /// </summary>
    /// <param name="lockToken">Existing lock token</param>
    /// <param name="additionalTimeMs">Additional time in milliseconds</param>
    /// <returns>True if successfully extended</returns>
    Task<bool> ExtendLockAsync(string lockToken, int additionalTimeMs);
}

/// <summary>
/// Result of lock acquisition attempt
/// </summary>
public class LockAcquisitionResult
{
    public bool Success { get; set; }
    public string? LockToken { get; set; }
    public string? ErrorMessage { get; set; }
    public List<Guid> ConflictingSeats { get; set; } = new();
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Information about seat lock status
/// </summary>
public class SeatLockInfo
{
    public Guid SeatId { get; set; }
    public bool IsLocked { get; set; }
    public Guid? LockedByUserId { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? LockToken { get; set; }
}

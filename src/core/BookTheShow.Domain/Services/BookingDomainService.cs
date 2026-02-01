using BookTheShow.Domain.Entities;
using BookTheShow.Domain.Enums;

namespace BookTheShow.Domain.Services;

/// <summary>
/// Domain service for booking business logic orchestration
/// Handles seat reservation, booking creation, and validation
/// </summary>
public class BookingDomainService : IBookingDomainService
{
    public BookingInitiationResult ReserveSeatsForBooking(
        Event @event,
        User user,
        List<Guid> seatIds,
        string customerEmail,
        string customerPhone)
    {
        var result = new BookingInitiationResult();

        try
        {
            // Validation 1: Event must be live and booking must be open
            if (@event.Status != EventStatus.Live)
            {
                result.Success = false;
                result.ErrorMessage = "Event is not currently live for booking";
                return result;
            }

            if (!@event.IsBookingOpen())
            {
                result.Success = false;
                result.ErrorMessage = "Booking is not currently open for this event";
                return result;
            }

            // Validation 2: Check seat count limits
            if (seatIds.Count > @event.MaxSeatsPerBooking)
            {
                result.Success = false;
                result.ErrorMessage = $"Cannot book more than {@event.MaxSeatsPerBooking} seats per booking";
                return result;
            }

            if (seatIds.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "At least one seat must be selected";
                return result;
            }

            // Validation 3: Check if seats can be reserved
            if (!@event.CanReserveSeats(seatIds))
            {
                result.Success = false;
                result.ErrorMessage = "One or more selected seats are not available";
                return result;
            }

            // Reserve seats - this will throw if seats are not available
            var reservations = @event.ReserveSeats(seatIds, user.Id);
            result.ReservedSeats = reservations;

            // Calculate pricing
            var priceCalculation = CalculateBookingPrice(@event, reservations);
            result.PriceCalculation = priceCalculation;

            // Create booking
            var booking = Booking.Create(
                user.Id,
                @event.Id,
                customerEmail,
                customerPhone,
                reservations,
                serviceFeePercentage: 0.10m,
                taxPercentage: 0.08m,
                bookingTimeoutMinutes: 20
            );

            result.Success = true;
            result.BookingId = booking.Id;
            result.ExpiresAt = booking.BookingExpiresAt;

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public BookingConfirmationResult ConfirmBooking(
        Booking booking,
        PaymentMethod paymentMethod,
        string paymentTransactionId)
    {
        var result = new BookingConfirmationResult();

        try
        {
            // Validation: Booking must be in pending or payment-pending state
            if (booking.Status != BookingStatus.Pending && 
                booking.Status != BookingStatus.PaymentPending)
            {
                result.Success = false;
                result.ErrorMessage = $"Cannot confirm booking with status: {booking.Status}";
                return result;
            }

            // Check if booking has expired
            if (booking.IsExpired())
            {
                booking.MarkExpired();
                result.Success = false;
                result.ErrorMessage = "Booking has expired";
                return result;
            }

            // Process payment
            booking.ProcessPayment(paymentMethod, paymentTransactionId);

            // Confirm the booking (this also confirms seat reservations)
            booking.ConfirmPayment();

            // Generate ticket codes (simple implementation)
            var ticketCodes = GenerateTicketCodes(booking);

            result.Success = true;
            result.ConfirmedBooking = booking;
            result.BookingReference = booking.BookingReference;
            result.TicketCodes = ticketCodes;

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public BookingCancellationResult CancelBooking(
        Booking booking,
        string reason)
    {
        var result = new BookingCancellationResult();

        try
        {
            // Check if booking can be cancelled
            if (!booking.CanBeCancelled())
            {
                result.Success = false;
                result.ErrorMessage = "This booking cannot be cancelled. Please check cancellation policy.";
                return result;
            }

            // Calculate refund amount
            decimal refundAmount = 0;
            if (booking.Status == BookingStatus.Confirmed)
            {
                // Full refund for confirmed bookings (if within cancellation window)
                refundAmount = booking.GrandTotal;
            }

            // Cancel the booking
            booking.Cancel(reason);

            // Collect released seat IDs
            var releasedSeatIds = booking.SeatReservations
                .Select(r => r.SeatId)
                .ToList();

            result.Success = true;
            result.RefundAmount = refundAmount;
            result.ReleasedSeatIds = releasedSeatIds;

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    public int ProcessExpiredBookings(List<Booking> bookings, DateTime currentTime)
    {
        int expiredCount = 0;

        foreach (var booking in bookings)
        {
            if (booking.IsExpired())
            {
                booking.MarkExpired();
                expiredCount++;
            }
        }

        return expiredCount;
    }

    // Helper Methods

    private BookingPriceCalculation CalculateBookingPrice(
        Event @event,
        List<SeatReservation> reservations)
    {
        var calculation = new BookingPriceCalculation
        {
            EventId = @event.Id,
            SeatIds = reservations.Select(r => r.SeatId).ToList(),
            BaseAmount = reservations.Sum(r => r.Price),
            ServiceFeePercentage = 0.10m,
            TaxPercentage = 0.08m,
            CalculatedAt = DateTime.UtcNow
        };

        calculation.ServiceFee = Math.Round(calculation.BaseAmount * calculation.ServiceFeePercentage, 2);
        calculation.TaxAmount = Math.Round((calculation.BaseAmount + calculation.ServiceFee) * calculation.TaxPercentage, 2);
        calculation.GrandTotal = calculation.BaseAmount + calculation.ServiceFee + calculation.TaxAmount;

        // Build seat price info
        calculation.SeatPrices = reservations.Select(r => new SeatPriceInfo
        {
            SeatId = r.SeatId,
            SeatCode = r.Seat?.GetSeatCode() ?? "Unknown",
            SeatType = r.Seat?.Type ?? SeatType.Regular,
            BasePrice = @event.BasePrice,
            PricingTier = r.Seat?.PricingTier ?? 1.0m,
            FinalPrice = r.Price
        }).ToList();

        return calculation;
    }

    private List<string> GenerateTicketCodes(Booking booking)
    {
        var ticketCodes = new List<string>();

        foreach (var reservation in booking.SeatReservations)
        {
            // Generate format: TICKET-{BookingRef}-{SeatCode}-{Random}
            var seatCode = reservation.Seat?.GetSeatCode() ?? "UNKNOWN";
            var random = new Random().Next(1000, 9999);
            var ticketCode = $"TICKET-{booking.BookingReference}-{seatCode}-{random}";
            
            ticketCodes.Add(ticketCode);
        }

        return ticketCodes;
    }
}

/// <summary>
/// Interface for booking domain service
/// </summary>
public interface IBookingDomainService
{
    /// <summary>
    /// Validates and reserves seats for a booking request
    /// </summary>
    BookingInitiationResult ReserveSeatsForBooking(
        Event @event,
        User user,
        List<Guid> seatIds,
        string customerEmail,
        string customerPhone);

    /// <summary>
    /// Confirms a booking after successful payment
    /// </summary>
    BookingConfirmationResult ConfirmBooking(
        Booking booking,
        PaymentMethod paymentMethod,
        string paymentTransactionId);

    /// <summary>
    /// Cancels a booking and releases seats
    /// </summary>
    BookingCancellationResult CancelBooking(
        Booking booking,
        string reason);

    /// <summary>
    /// Processes expired bookings and releases seats
    /// </summary>
    int ProcessExpiredBookings(List<Booking> bookings, DateTime currentTime);
}

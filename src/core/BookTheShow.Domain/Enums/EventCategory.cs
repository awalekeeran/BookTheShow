namespace BookTheShow.Domain.Enums;

/// <summary>
/// Represents different categories of events available for booking
/// </summary>
public enum EventCategory
{
    /// <summary>
    /// Music concerts and live performances
    /// </summary>
    Concert = 1,
    
    /// <summary>
    /// Sporting events and competitions
    /// </summary>
    Sports = 2,
    
    /// <summary>
    /// Theater plays and dramatic performances
    /// </summary>
    Theater = 3,
    
    /// <summary>
    /// Stand-up comedy shows and comedic performances
    /// </summary>
    Comedy = 4,
    
    /// <summary>
    /// Business conferences and professional events
    /// </summary>
    Conference = 5,
    
    /// <summary>
    /// Educational workshops and training sessions
    /// </summary>
    Workshop = 6,
    
    /// <summary>
    /// Multi-day festivals and cultural celebrations
    /// </summary>
    Festival = 7,
    
    /// <summary>
    /// Art exhibitions and gallery showings
    /// </summary>
    Exhibition = 8,
    
    /// <summary>
    /// Dance performances and recitals
    /// </summary>
    Dance = 9,
    
    /// <summary>
    /// Movie screenings and film premieres
    /// </summary>
    Movie = 10,
    
    /// <summary>
    /// Miscellaneous events that don't fit other categories
    /// </summary>
    Other = 99
}

namespace BookTheShow.API;

/// <summary>
/// Configuration settings for controlling logging verbosity and diagnostics
/// </summary>
public class DiagnosticsSettings
{
    public const string SectionName = "DiagnosticsSettings";

    /// <summary>
    /// Enable detailed diagnostic logging (includes all Microsoft framework logs)
    /// </summary>
    public bool EnableDetailedDiagnostics { get; set; } = false;

    /// <summary>
    /// Enable HTTP request logging
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    /// Log requests for static files (CSS, JS, images, etc.)
    /// </summary>
    public bool LogStaticFiles { get; set; } = false;

    /// <summary>
    /// Log health check endpoint requests
    /// </summary>
    public bool LogHealthChecks { get; set; } = false;
}

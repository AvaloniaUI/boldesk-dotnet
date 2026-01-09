namespace BoldDesk.Exceptions;

/// <summary>
/// Exception thrown when a BoldDesk API request times out.
/// </summary>
public class BoldDeskTimeoutException : BoldDeskApiException
{
    /// <summary>
    /// The URL that timed out.
    /// </summary>
    public string? RequestUrl { get; }

    /// <summary>
    /// The elapsed time in milliseconds before the timeout occurred.
    /// </summary>
    public long ElapsedMilliseconds { get; }

    public BoldDeskTimeoutException(string message)
        : base(message)
    {
    }

    public BoldDeskTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public BoldDeskTimeoutException(string message, string? requestUrl, long elapsedMilliseconds, Exception innerException)
        : base(message, innerException)
    {
        RequestUrl = requestUrl;
        ElapsedMilliseconds = elapsedMilliseconds;
    }
}

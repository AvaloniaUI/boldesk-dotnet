using System.Net;
using BoldDesk.Models;

namespace BoldDesk.Exceptions;

public class BoldDeskRateLimitException : BoldDeskApiException
{
    public DateTime? ResetTime { get; }
    public int? RemainingCalls { get; }
    public int? RateLimitPeriod { get; }

    public BoldDeskRateLimitException(string message, RateLimitInfo? rateLimitInfo = null) 
        : base(message, HttpStatusCode.TooManyRequests)
    {
        if (rateLimitInfo != null)
        {
            ResetTime = rateLimitInfo.Reset;
            RemainingCalls = rateLimitInfo.Remaining;
            RateLimitPeriod = rateLimitInfo.Limit;
        }
    }

    public BoldDeskRateLimitException(BoldDeskErrorResponse errorResponse, RateLimitInfo? rateLimitInfo = null) 
        : base(errorResponse)
    {
        if (rateLimitInfo != null)
        {
            ResetTime = rateLimitInfo.Reset;
            RemainingCalls = rateLimitInfo.Remaining;
            RateLimitPeriod = rateLimitInfo.Limit;
        }
    }

    public TimeSpan? GetWaitTime()
    {
        if (ResetTime.HasValue)
        {
            var waitTime = ResetTime.Value - DateTime.UtcNow;
            return waitTime > TimeSpan.Zero ? waitTime : null;
        }
        return null;
    }

    public override string ToString()
    {
        var baseString = base.ToString();
        var rateLimitInfo = $"Rate Limit: {RateLimitPeriod} calls/min, Remaining: {RemainingCalls}, Reset: {ResetTime:yyyy-MM-dd HH:mm:ss UTC}";
        
        if (GetWaitTime() is TimeSpan waitTime)
        {
            rateLimitInfo += $", Wait: {waitTime.TotalSeconds:F1} seconds";
        }
        
        return $"{baseString}{Environment.NewLine}{rateLimitInfo}";
    }
}
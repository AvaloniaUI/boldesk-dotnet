using BoldDesk.Models;

namespace BoldDesk;

public interface IBoldDeskService
{
    RateLimitInfo? LastRateLimitInfo { get; }

    /// <summary>
    /// Fetches a single page of tickets from the BoldDesk API
    /// </summary>
    Task<BoldDeskResponse<Ticket>> GetTicketsAsync(TicketQueryParameters? parameters = null);

    /// <summary>
    /// Fetches all tickets using pagination, respecting rate limits
    /// </summary>
    IAsyncEnumerable<Ticket> GetAllTicketsAsync(TicketQueryParameters? parameters = null, IProgress<string>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a count of tickets matching the query without fetching all data
    /// </summary>
    Task<int> GetTicketCountAsync(TicketQueryParameters? parameters = null);

    /// <summary>
    /// Fetches a single page of worklogs from the BoldDesk API
    /// </summary>
    Task<BoldDeskResponse<Worklog>> GetWorklogsAsync(WorklogQueryParameters? parameters = null);

    /// <summary>
    /// Fetches all worklogs using pagination, respecting rate limits
    /// </summary>
    IAsyncEnumerable<Worklog> GetAllWorklogsAsync(WorklogQueryParameters? parameters = null, IProgress<string>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a count of worklogs matching the query without fetching all data
    /// </summary>
    Task<int> GetWorklogCountAsync(WorklogQueryParameters? parameters = null);

    /// <summary>
    /// Gets the date range from existing tickets to use for worklog queries
    /// </summary>
    Task<(DateTime? oldestDate, DateTime? newestDate)> GetTicketDateRangeAsync();

    /// <summary>
    /// Fetches a list of brands for your organization
    /// </summary>
    Task<BoldDeskResponse<Brand>> GetBrandsAsync();

    /// <summary>
    /// Fetches a list of brands based on user preferences
    /// </summary>
    Task<BoldDeskResponse<UserBrand>> GetUserBrandsAsync(UserBrandQueryParameters? parameters = null);

    void Dispose();
}
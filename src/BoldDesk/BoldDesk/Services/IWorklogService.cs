using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Service for managing BoldDesk worklogs
/// </summary>
public interface IWorklogService
{
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
}
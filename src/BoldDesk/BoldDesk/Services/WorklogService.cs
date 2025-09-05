using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Implementation of BoldDesk worklog operations
/// </summary>
public class WorklogService : BaseService, IWorklogService
{
    public WorklogService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions) : base(httpClient, baseUrl, jsonOptions)
    {
    }

    /// <summary>
    /// Fetches a single page of worklogs from the BoldDesk API
    /// </summary>
    public async Task<BoldDeskResponse<Worklog>> GetWorklogsAsync(WorklogQueryParameters? parameters = null)
    {
        parameters ??= new WorklogQueryParameters();
        var url = BuildWorklogsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<Worklog>>(url);
    }

    /// <summary>
    /// Fetches all worklogs using pagination, respecting rate limits
    /// </summary>
    public async IAsyncEnumerable<Worklog> GetAllWorklogsAsync(WorklogQueryParameters? parameters = null, IProgress<string>? progress = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new WorklogQueryParameters();
        var currentPage = parameters.Page;
        var totalFetched = 0;
        var hasMorePages = true;

        while (hasMorePages && !cancellationToken.IsCancellationRequested)
        {
            await EnsureRateLimitCompliance();

            parameters.Page = currentPage;
            
            progress?.Report($"Fetching worklog page {currentPage}...");
            
            var response = await GetWorklogsAsync(parameters);
            
            if (response.Result.Count == 0)
            {
                hasMorePages = false;
                break;
            }

            foreach (var worklog in response.Result)
            {
                totalFetched++;
                yield return worklog;
            }

            if (response.Count > 0 && totalFetched >= response.Count)
            {
                hasMorePages = false;
            }
            else if (response.Result.Count < parameters.PerPage)
            {
                hasMorePages = false;
            }
            else
            {
                currentPage++;
            }

            progress?.Report($"Fetched {totalFetched} worklogs so far...");
        }

        progress?.Report($"Completed. Total worklogs fetched: {totalFetched}");
    }

    /// <summary>
    /// Gets a count of worklogs matching the query without fetching all data
    /// </summary>
    public async Task<int> GetWorklogCountAsync(WorklogQueryParameters? parameters = null)
    {
        parameters ??= new WorklogQueryParameters();
        
        // Set up parameters to get count with minimal data
        var countParams = new WorklogQueryParameters
        {
            Page = 1,
            PerPage = 1,
            RequiresCounts = true,
            OrderBy = parameters.OrderBy,
            LastCreatedDateFrom = parameters.LastCreatedDateFrom,
            LastCreatedDateTo = parameters.LastCreatedDateTo,
            LastUpdatedDateFrom = parameters.LastUpdatedDateFrom,
            LastUpdatedDateTo = parameters.LastUpdatedDateTo,
            IncludeDeletedWorklogs = parameters.IncludeDeletedWorklogs
        };

        var response = await GetWorklogsAsync(countParams);
        return response.Count;
    }

    private string BuildWorklogsUrl(WorklogQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/worklogs");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["orderBy"] = parameters.OrderBy;
        }

        if (parameters.LastCreatedDateFrom.HasValue)
        {
            query["lastCreatedDateFrom"] = parameters.LastCreatedDateFrom.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        }

        if (parameters.LastCreatedDateTo.HasValue)
        {
            query["lastCreatedDateTo"] = parameters.LastCreatedDateTo.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        }

        if (parameters.LastUpdatedDateFrom.HasValue)
        {
            query["lastUpdatedDateFrom"] = parameters.LastUpdatedDateFrom.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        }

        if (parameters.LastUpdatedDateTo.HasValue)
        {
            query["lastUpdatedDateTo"] = parameters.LastUpdatedDateTo.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        }

        if (parameters.IncludeDeletedWorklogs)
        {
            query["includeDeletedWorklogs"] = "true";
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}
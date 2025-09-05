using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using BoldDesk.Extensions;
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Implementation of BoldDesk ticket operations
/// </summary>
public class TicketService : BaseService, ITicketService
{
    public TicketService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions) 
        : base(httpClient, baseUrl, jsonOptions)
    {
    }

    /// <summary>
    /// Fetches a single page of tickets from the BoldDesk API
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> GetTicketsAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        var url = BuildTicketsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<Ticket>>(url);
    }

    /// <summary>
    /// Fetches all tickets using pagination, respecting rate limits
    /// </summary>
    public async IAsyncEnumerable<Ticket> GetAllTicketsAsync(
        TicketQueryParameters? parameters = null, 
        IProgress<string>? progress = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new TicketQueryParameters();
        var currentPage = parameters.Page;
        var totalFetched = 0;
        var hasMorePages = true;

        while (hasMorePages && !cancellationToken.IsCancellationRequested)
        {
            await EnsureRateLimitCompliance();

            parameters.Page = currentPage;
            
            progress?.Report($"Fetching page {currentPage}...");
            
            var response = await GetTicketsAsync(parameters);
            
            if (response.Result.Count == 0)
            {
                hasMorePages = false;
                break;
            }

            foreach (var ticket in response.Result)
            {
                totalFetched++;
                yield return ticket;
            }

            // Check if we have more pages
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

            progress?.Report($"Fetched {totalFetched} tickets so far...");
        }

        progress?.Report($"Completed. Total tickets fetched: {totalFetched}");
    }

    /// <summary>
    /// Gets a count of tickets matching the query without fetching all data
    /// </summary>
    public async Task<int> GetTicketCountAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        
        // Set up parameters to get count with minimal data
        var countParams = new TicketQueryParameters
        {
            Page = 1,
            PerPage = 1,
            RequiresCounts = true,
            Q = parameters.Q,
            SortBy = parameters.SortBy,
            OrderBy = parameters.OrderBy
        };

        var response = await GetTicketsAsync(countParams);
        return response.Count;
    }

    /// <summary>
    /// Retrieves a single ticket by its ID
    /// </summary>
    public async Task<Ticket> GetTicketAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}";
        return await ExecuteRequestAsync<Ticket>(url);
    }

    /// <summary>
    /// Gets the date range from existing tickets
    /// </summary>
    public async Task<(DateTime? oldestDate, DateTime? newestDate)> GetTicketDateRangeAsync()
    {
        try
        {
            // Get oldest ticket
            var oldestParams = new TicketQueryParameters
            {
                Page = 1,
                PerPage = 1,
                SortBy = "createdon",
                OrderBy = OrderBy.Ascending
            };

            // Get newest ticket
            var newestParams = new TicketQueryParameters
            {
                Page = 1,
                PerPage = 1,
                SortBy = "createdon",
                OrderBy = OrderBy.Descending
            };

            var oldestTask = GetTicketsAsync(oldestParams);
            var newestTask = GetTicketsAsync(newestParams);

            await Task.WhenAll(oldestTask, newestTask);

            var oldestResponse = await oldestTask;
            var newestResponse = await newestTask;

            var oldestDate = oldestResponse.Result.FirstOrDefault()?.CreatedOn;
            var newestDate = newestResponse.Result.FirstOrDefault()?.CreatedOn;

            return (oldestDate, newestDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not determine ticket date range: {ex.Message}");
            return (null, null);
        }
    }

    private string BuildTicketsUrl(TicketQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString(); // Enforce API limit
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.Q))
        {
            query["q"] = parameters.Q;
        }

        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query["sortBy"] = parameters.SortBy;
        }

        if (parameters.OrderBy.HasValue)
        {
            query["orderBy"] = parameters.OrderBy.Value.ToApiString();
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}

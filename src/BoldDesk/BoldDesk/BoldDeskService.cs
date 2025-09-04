using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using BoldDesk.Exceptions;
using BoldDesk.Models;

namespace BoldDesk;

public class BoldDeskService : IDisposable, IBoldDeskService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions;
    private RateLimitInfo? _lastRateLimitInfo;

    public BoldDeskService(string domain, string apiKey)
    {
        _baseUrl = $"https://{domain}/api/v1.0";
        _apiKey = apiKey;
        
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public RateLimitInfo? LastRateLimitInfo => _lastRateLimitInfo;

    /// <summary>
    /// Fetches a single page of tickets from the BoldDesk API
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> GetTicketsAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        
        var url = BuildTicketsUrl(parameters);
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            
            // Parse rate limit headers
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return new BoldDeskResponse<Ticket>();
            }
            
            var result = JsonSerializer.Deserialize<BoldDeskResponse<Ticket>>(content, _jsonOptions);
            
            return result ?? new BoldDeskResponse<Ticket>();
        }
        catch (BoldDeskApiException)
        {
            // Re-throw BoldDesk exceptions
            throw;
        }
        catch (HttpRequestException ex)
        {
            // Wrap HTTP exceptions in BoldDeskApiException
            throw new BoldDeskApiException($"Network error while calling BoldDesk API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new BoldDeskApiException($"Failed to parse API response: {ex.Message}. The API may have returned an unexpected format.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new BoldDeskApiException($"API request timed out: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Fetches all tickets using pagination, respecting rate limits
    /// </summary>
    public async IAsyncEnumerable<Ticket> GetAllTicketsAsync(TicketQueryParameters? parameters = null, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
    {
        parameters ??= new TicketQueryParameters();
        var currentPage = parameters.Page;
        var totalFetched = 0;
        var hasMorePages = true;

        while (hasMorePages && !cancellationToken.IsCancellationRequested)
        {
            // Check rate limit before making request
            await EnsureRateLimitCompliance();

            parameters.Page = currentPage;
            
            progress?.Report($"Fetching page {currentPage}...");
            
            var response = await GetTicketsAsync(parameters);
            
            if (response.Result.Count == 0)
            {
                hasMorePages = false;
                break;
            }

            // Process tickets directly from result array
            foreach (var ticket in response.Result)
            {
                totalFetched++;
                yield return ticket;
            }

            // Check if we have more pages
            hasMorePages = response.Result.Count == parameters.PerPage;

            currentPage++;
            
            progress?.Report($"Fetched {totalFetched} tickets so far...");
            
            // Small delay to be respectful to the API
            await Task.Delay(100, cancellationToken);
        }
        
        progress?.Report($"Completed! Total tickets fetched: {totalFetched}");
    }

    /// <summary>
    /// Gets a count of tickets matching the query without fetching all data
    /// </summary>
    public async Task<int> GetTicketCountAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        parameters.PerPage = 1; // Minimize data transfer
        parameters.RequiresCounts = true;

        var response = await GetTicketsAsync(parameters);
        return response.Count;
    }

    /// <summary>
    /// Fetches a single page of worklogs from the BoldDesk API
    /// </summary>
    public async Task<BoldDeskResponse<Worklog>> GetWorklogsAsync(WorklogQueryParameters? parameters = null)
    {
        parameters ??= new WorklogQueryParameters();
        
        var url = BuildWorklogsUrl(parameters);
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            
            // Parse rate limit headers
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return new BoldDeskResponse<Worklog>();
            }
            
            var result = JsonSerializer.Deserialize<BoldDeskResponse<Worklog>>(content, _jsonOptions);
            
            return result ?? new BoldDeskResponse<Worklog>();
        }
        catch (BoldDeskApiException)
        {
            // Re-throw BoldDesk exceptions
            throw;
        }
        catch (HttpRequestException ex)
        {
            // Wrap HTTP exceptions in BoldDeskApiException
            throw new BoldDeskApiException($"Network error while calling BoldDesk API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new BoldDeskApiException($"Failed to parse API response: {ex.Message}. The API may have returned an unexpected format.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new BoldDeskApiException($"API request timed out: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Fetches all worklogs using pagination, respecting rate limits
    /// </summary>
    public async IAsyncEnumerable<Worklog> GetAllWorklogsAsync(WorklogQueryParameters? parameters = null, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
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

            hasMorePages = response.Result.Count == parameters.PerPage;
            currentPage++;
            
            progress?.Report($"Fetched {totalFetched} worklogs so far...");
            
            await Task.Delay(100, cancellationToken);
        }
        
        progress?.Report($"Completed! Total worklogs fetched: {totalFetched}");
    }

    /// <summary>
    /// Gets a count of worklogs matching the query without fetching all data
    /// </summary>
    public async Task<int> GetWorklogCountAsync(WorklogQueryParameters? parameters = null)
    {
        parameters ??= new WorklogQueryParameters();
        parameters.PerPage = 1;
        parameters.RequiresCounts = true;

        var response = await GetWorklogsAsync(parameters);
        return response.Count;
    }

    /// <summary>
    /// Gets the date range from existing tickets to use for worklog queries
    /// </summary>
    public async Task<(DateTime? oldestDate, DateTime? newestDate)> GetTicketDateRangeAsync()
    {
        try
        {
            // Get oldest ticket (ascending order)
            var oldestResponse = await GetTicketsAsync(new TicketQueryParameters 
            { 
                Page = 1, 
                PerPage = 1, 
                SortOrder = "asc",
                SortBy = "createdOn"
            });

            // Get newest ticket (descending order) 
            var newestResponse = await GetTicketsAsync(new TicketQueryParameters 
            { 
                Page = 1, 
                PerPage = 1, 
                SortOrder = "desc",
                SortBy = "createdOn"
            });

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

    /// <summary>
    /// Fetches a list of brands for your organization
    /// </summary>
    public async Task<BoldDeskResponse<Brand>> GetBrandsAsync()
    {
        var url = $"{_baseUrl}/brands";
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            
            // Parse rate limit headers
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return new BoldDeskResponse<Brand>();
            }
            
            var result = JsonSerializer.Deserialize<BoldDeskResponse<Brand>>(content, _jsonOptions);
            
            return result ?? new BoldDeskResponse<Brand>();
        }
        catch (BoldDeskApiException)
        {
            // Re-throw BoldDesk exceptions
            throw;
        }
        catch (HttpRequestException ex)
        {
            // Wrap HTTP exceptions in BoldDeskApiException
            throw new BoldDeskApiException($"Network error while calling BoldDesk API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new BoldDeskApiException($"Failed to parse API response: {ex.Message}. The API may have returned an unexpected format.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new BoldDeskApiException($"API request timed out: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Fetches a list of brands based on user preferences
    /// </summary>
    public async Task<BoldDeskResponse<UserBrand>> GetUserBrandsAsync(UserBrandQueryParameters? parameters = null)
    {
        parameters ??= new UserBrandQueryParameters();
        
        var url = BuildUserBrandsUrl(parameters);
        
        try
        {
            var response = await _httpClient.GetAsync(url);
            
            // Parse rate limit headers
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return new BoldDeskResponse<UserBrand>();
            }
            
            var result = JsonSerializer.Deserialize<BoldDeskResponse<UserBrand>>(content, _jsonOptions);
            
            return result ?? new BoldDeskResponse<UserBrand>();
        }
        catch (BoldDeskApiException)
        {
            // Re-throw BoldDesk exceptions
            throw;
        }
        catch (HttpRequestException ex)
        {
            // Wrap HTTP exceptions in BoldDeskApiException
            throw new BoldDeskApiException($"Network error while calling BoldDesk API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new BoldDeskApiException($"Failed to parse API response: {ex.Message}. The API may have returned an unexpected format.", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new BoldDeskApiException($"API request timed out: {ex.Message}", ex);
        }
    }

    private string BuildTicketsUrl(TicketQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{_baseUrl}/tickets");
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

        if (!string.IsNullOrWhiteSpace(parameters.SortOrder))
        {
            query["sortOrder"] = parameters.SortOrder;
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildWorklogsUrl(WorklogQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{_baseUrl}/tickets/worklogs");
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

    private string BuildUserBrandsUrl(UserBrandQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{_baseUrl}/user_brands");
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrWhiteSpace(parameters.Filter))
        {
            query["filter"] = parameters.Filter;
        }

        if (parameters.NeedToIncludeDeactivatedBrands)
        {
            query["needToIncludeDeactivatedBrands"] = "true";
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private void ParseRateLimitHeaders(HttpResponseHeaders headers)
    {
        _lastRateLimitInfo = new RateLimitInfo();

        if (headers.TryGetValues("x-rate-limit-limit", out var limitValues))
        {
            if (int.TryParse(limitValues.FirstOrDefault(), out var limit))
            {
                _lastRateLimitInfo.Limit = limit;
            }
        }

        if (headers.TryGetValues("x-rate-limit-remaining", out var remainingValues))
        {
            if (int.TryParse(remainingValues.FirstOrDefault(), out var remaining))
            {
                _lastRateLimitInfo.Remaining = remaining;
            }
        }

        if (headers.TryGetValues("x-rate-limit-reset", out var resetValues))
        {
            if (DateTime.TryParse(resetValues.FirstOrDefault(), out var reset))
            {
                _lastRateLimitInfo.Reset = reset;
            }
        }
    }

    private async Task EnsureRateLimitCompliance()
    {
        if (_lastRateLimitInfo == null)
            return;

        // If we're getting close to the rate limit, wait
        if (_lastRateLimitInfo.Remaining <= 5)
        {
            var waitTime = _lastRateLimitInfo.Reset - DateTime.UtcNow;
            if (waitTime > TimeSpan.Zero && waitTime < TimeSpan.FromMinutes(2))
            {
                Console.WriteLine($"Rate limit nearly exceeded. Waiting {waitTime.TotalSeconds:F0} seconds...");
                await Task.Delay(waitTime);
            }
        }
    }

    private async Task ThrowBoldDeskException(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        BoldDeskErrorResponse? errorResponse = null;

        // Try to parse error response
        if (!string.IsNullOrWhiteSpace(errorContent))
        {
            try
            {
                errorResponse = JsonSerializer.Deserialize<BoldDeskErrorResponse>(errorContent, _jsonOptions);
            }
            catch
            {
                // If parsing fails, create a generic error response
                errorResponse = new BoldDeskErrorResponse
                {
                    Message = errorContent,
                    StatusCode = (int)response.StatusCode,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = errorContent,
                            ErrorType = "UnknownError"
                        }
                    }
                };
            }
        }

        // Throw specific exception types based on status code
        switch (response.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                throw new BoldDeskAuthenticationException(errorResponse ?? new BoldDeskErrorResponse 
                { 
                    Message = "Authentication failed. Please verify your API key.",
                    StatusCode = 401,
                    Errors = new List<BoldDeskError> 
                    { 
                        new BoldDeskError 
                        { 
                            Field = "User", 
                            ErrorMessage = "Unauthorized", 
                            ErrorType = BoldDeskErrorType.Unauthorized 
                        } 
                    }
                });

            case HttpStatusCode.Forbidden:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Access denied. Your API key may not have permission to access this resource.",
                    StatusCode = 403,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "UserID",
                            ErrorMessage = "Access Denied",
                            ErrorType = BoldDeskErrorType.AccessDenied
                        }
                    }
                });

            case HttpStatusCode.BadRequest:
                throw new BoldDeskValidationException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Validation failed",
                    StatusCode = 400,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = errorContent,
                            ErrorType = BoldDeskErrorType.InvalidValue
                        }
                    }
                });

            case HttpStatusCode.TooManyRequests:
                throw new BoldDeskRateLimitException(
                    errorResponse ?? new BoldDeskErrorResponse
                    {
                        Message = "Rate limit exceeded",
                        StatusCode = 429,
                        Errors = new List<BoldDeskError>
                        {
                            new BoldDeskError
                            {
                                Field = "",
                                ErrorMessage = "API calls quota exceeded",
                                ErrorType = BoldDeskErrorType.APICallQuotaExceeded
                            }
                        }
                    },
                    _lastRateLimitInfo);

            case HttpStatusCode.NotFound:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Resource not found",
                    StatusCode = 404,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = "Not found",
                            ErrorType = BoldDeskErrorType.NotFound
                        }
                    }
                });

            case HttpStatusCode.MethodNotAllowed:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Method not allowed",
                    StatusCode = 405,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = "Method not allowed",
                            ErrorType = BoldDeskErrorType.MethodNotAllowed
                        }
                    }
                });

            case HttpStatusCode.UnsupportedMediaType:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Unsupported media type",
                    StatusCode = 415,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = "Unsupported Media Type",
                            ErrorType = BoldDeskErrorType.UnsupportedMediaType
                        }
                    }
                });

            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                throw new BoldDeskApiException(errorResponse ?? new BoldDeskErrorResponse
                {
                    Message = "Unable to process your request. Please try again later",
                    StatusCode = (int)response.StatusCode,
                    Errors = new List<BoldDeskError>
                    {
                        new BoldDeskError
                        {
                            Field = "",
                            ErrorMessage = "Unable to process your request. Please try again later",
                            ErrorType = BoldDeskErrorType.UnknownError
                        }
                    }
                });

            default:
                throw new BoldDeskApiException(
                    errorResponse?.Message ?? $"API request failed with status {response.StatusCode}",
                    response.StatusCode,
                    errorResponse);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
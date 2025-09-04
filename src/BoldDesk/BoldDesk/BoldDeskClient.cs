using System.Net.Http.Headers;
using System.Text.Json;
using BoldDesk.Models;
using BoldDesk.Services;

namespace BoldDesk;

/// <summary>
/// Main client for interacting with the BoldDesk API
/// </summary>
public class BoldDeskClient : IBoldDeskClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    
    // Sub-services
    private readonly TicketService _ticketService;
    private readonly WorklogService _worklogService;
    private readonly BrandService _brandService;

    /// <summary>
    /// Initializes a new instance of the BoldDesk client
    /// </summary>
    /// <param name="domain">Your BoldDesk domain (e.g., yourdomain.bolddesk.com)</param>
    /// <param name="apiKey">Your BoldDesk API key</param>
    public BoldDeskClient(string domain, string apiKey)
    {
        _baseUrl = $"https://{domain}/api/v1.0";
        
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        // Initialize sub-services
        _ticketService = new TicketService(_httpClient, _baseUrl, _jsonOptions);
        _worklogService = new WorklogService(_httpClient, _baseUrl, _jsonOptions);
        _brandService = new BrandService(_httpClient, _baseUrl, _jsonOptions);
    }

    /// <summary>
    /// Gets the last rate limit information from API responses
    /// </summary>
    public RateLimitInfo? LastRateLimitInfo 
    { 
        get 
        {
            // Return the most recent rate limit info from any service
            var rateLimitInfos = new[]
            {
                _ticketService.GetLastRateLimitInfo(),
                _worklogService.GetLastRateLimitInfo(),
                _brandService.GetLastRateLimitInfo()
            };
            
            return rateLimitInfos
                .Where(r => r != null)
                .OrderByDescending(r => r!.Reset)
                .FirstOrDefault();
        }
    }

    /// <summary>
    /// Service for managing tickets
    /// </summary>
    public ITicketService Tickets => _ticketService;

    /// <summary>
    /// Service for managing worklogs
    /// </summary>
    public IWorklogService Worklogs => _worklogService;

    /// <summary>
    /// Service for managing brands
    /// </summary>
    public IBrandService Brands => _brandService;

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
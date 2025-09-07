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
    private readonly bool _ownsHttpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    
    // Sub-services
    private readonly TicketService _ticketService;
    private readonly WorklogService _worklogService;
    private readonly BrandService _brandService;
    private readonly AgentService _agentService;
    private readonly ContactGroupService _contactGroupService;
    private readonly ContactService _contactService;
    private readonly FieldService _fieldService;

    /// <summary>
    /// Initializes a new instance of the BoldDesk client with a new HttpClient (not recommended for production)
    /// </summary>
    /// <param name="domain">Your BoldDesk domain (e.g., yourdomain.bolddesk.com)</param>
    /// <param name="apiKey">Your BoldDesk API key</param>
    public BoldDeskClient(string domain, string apiKey) : this(new HttpClient(), domain, apiKey, true)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the BoldDesk client with a provided HttpClient (recommended for production)
    /// </summary>
    /// <param name="httpClient">HttpClient instance (should be managed by DI container or HttpClientFactory)</param>
    /// <param name="domain">Your BoldDesk domain (e.g., yourdomain.bolddesk.com)</param>
    /// <param name="apiKey">Your BoldDesk API key</param>
    public BoldDeskClient(HttpClient httpClient, string domain, string apiKey) : this(httpClient, domain, apiKey, false)
    {
    }
    
    /// <summary>
    /// Private constructor for internal initialization
    /// </summary>
    private BoldDeskClient(HttpClient httpClient, string domain, string apiKey, bool ownsHttpClient)
    {
        _httpClient = httpClient;
        _ownsHttpClient = ownsHttpClient;
        _baseUrl = $"https://{domain}/api/v1.0";
        
        // Configure HTTP timeout (allow override via env var)
        try
        {
            var envTimeout = Environment.GetEnvironmentVariable("BOLDDESK_HTTP_TIMEOUT_SECONDS");
            if (!string.IsNullOrWhiteSpace(envTimeout) && int.TryParse(envTimeout, out var seconds) && seconds > 0)
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(seconds);
            }
            else
            {
                // If using the default HttpClient timeout (~100s), increase to handle slower endpoints
                if (_httpClient.Timeout.TotalSeconds <= 100)
                {
                    _httpClient.Timeout = TimeSpan.FromMinutes(3);
                }
            }
        }
        catch
        {
            // Ignore timeout configuration errors
        }
        
        if (!_httpClient.DefaultRequestHeaders.Contains("x-api-key"))
        {
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }
        
        if (!_httpClient.DefaultRequestHeaders.Accept.Any(h => h.MediaType == "application/json"))
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        // Initialize sub-services
        _ticketService = new TicketService(_httpClient, _baseUrl, _jsonOptions);
        _worklogService = new WorklogService(_httpClient, _baseUrl, _jsonOptions);
        _brandService = new BrandService(_httpClient, _baseUrl, _jsonOptions);
        _agentService = new AgentService(_httpClient, _baseUrl, _jsonOptions);
        _contactGroupService = new ContactGroupService(_httpClient, _baseUrl, _jsonOptions);
        _contactService = new ContactService(_httpClient, _baseUrl, _jsonOptions);
        _fieldService = new FieldService(_httpClient, _baseUrl, _jsonOptions);
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
                _brandService.GetLastRateLimitInfo(),
                _agentService.GetLastRateLimitInfo(),
                _contactGroupService.GetLastRateLimitInfo(),
                _contactService.GetLastRateLimitInfo(),
                _fieldService.GetLastRateLimitInfo()
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

    /// <summary>
    /// Service for managing agents
    /// </summary>
    public IAgentService Agents => _agentService;

    /// <summary>
    /// Service for managing contact groups
    /// </summary>
    public IContactGroupService ContactGroups => _contactGroupService;

    /// <summary>
    /// Service for managing contacts
    /// </summary>
    public IContactService Contacts => _contactService;

    /// <summary>
    /// Service for managing fields and field options
    /// </summary>
    public IFieldService Fields => _fieldService;

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient?.Dispose();
        }
    }
}

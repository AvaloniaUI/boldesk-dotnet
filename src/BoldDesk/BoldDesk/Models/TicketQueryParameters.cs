using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class TicketQueryParameters
{
    /// <summary>
    /// Provides Q parameter for filtering by specified fields
    /// </summary>
    public string? Q { get; set; }
    
    /// <summary>
    /// Unique ID of the filter in ticket listing
    /// </summary>
    public string? FilterId { get; set; }
    
    /// <summary>
    /// Fields to be returned (e.g., "agentId", "brandId")
    /// </summary>
    public List<string>? Fields { get; set; }
    
    /// <summary>
    /// The brand IDs for which tickets will be returned (comma-separated in API)
    /// </summary>
    public List<int>? BrandIds { get; set; }
    
    /// <summary>
    /// Specifies the number of the page to be fetched (default: 1)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Number of records to be fetched in a specified page (default: 100, max: 100)
    /// </summary>
    public int PerPage { get; set; } = 100;
    
    /// <summary>
    /// Determines whether the total number of records to be returned or not
    /// </summary>
    public bool RequiresCounts { get; set; } = true;
    
    /// <summary>
    /// Sorting order of records (e.g., "ticketId desc")
    /// Values allowed: ticketId, title, createdon, lastUpdatedon, closedon, resolutionDue
    /// </summary>
    public string? OrderBy { get; set; }
    
    // Legacy properties for backward compatibility
    [JsonIgnore]
    [Obsolete("Use OrderBy instead")]
    public string? SortBy { get; set; }
}
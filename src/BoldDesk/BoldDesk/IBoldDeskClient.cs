using BoldDesk.Models;
using BoldDesk.Services;

namespace BoldDesk;

/// <summary>
/// Main interface for the BoldDesk API client
/// </summary>
public interface IBoldDeskClient : IDisposable
{
    /// <summary>
    /// Gets the last rate limit information from API responses
    /// </summary>
    RateLimitInfo? LastRateLimitInfo { get; }

    /// <summary>
    /// Service for managing tickets
    /// </summary>
    ITicketService Tickets { get; }

    /// <summary>
    /// Service for managing worklogs
    /// </summary>
    IWorklogService Worklogs { get; }

    /// <summary>
    /// Service for managing brands
    /// </summary>
    IBrandService Brands { get; }

    // Future services can be added here:
    // IAgentService Agents { get; }
    // IContactService Contacts { get; }
    // IFieldService Fields { get; }
    // IGroupService Groups { get; }
    // ICategoryService Categories { get; }
}
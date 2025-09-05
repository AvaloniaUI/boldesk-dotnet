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

    /// <summary>
    /// Service for managing agents
    /// </summary>
    IAgentService Agents { get; }

    /// <summary>
    /// Service for managing contact groups
    /// </summary>
    IContactGroupService ContactGroups { get; }

    /// <summary>
    /// Service for managing contacts
    /// </summary>
    IContactService Contacts { get; }

    /// <summary>
    /// Service for managing fields and field options
    /// </summary>
    IFieldService Fields { get; }

    // Future services can be added here:
    // IGroupService Groups { get; }
    // ICategoryService Categories { get; }
}
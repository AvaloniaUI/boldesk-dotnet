namespace BoldDesk.Models;

/// <summary>
/// Query parameters for listing agents
/// </summary>
public class AgentQueryParameters
{
    /// <summary>
    /// Specifies the number of the page to be fetched
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of the records to be fetched in a specified page (Max: 100)
    /// </summary>
    public int PerPage { get; set; } = 10;

    /// <summary>
    /// Determines whether the total number of records should be returned
    /// </summary>
    public bool RequiresCounts { get; set; } = true;

    /// <summary>
    /// Defines the status of the user
    /// </summary>
    public int? UserStatus { get; set; }

    /// <summary>
    /// Determines whether the agent is available or not
    /// </summary>
    public bool? IsAvailable { get; set; }

    /// <summary>
    /// Roles available for agents (Agent, Supervisor, Admin, Owner)
    /// </summary>
    public string? RoleId { get; set; }

    /// <summary>
    /// Determines whether the listed agents are verified or not
    /// </summary>
    public bool? IsVerifiedAgents { get; set; }

    /// <summary>
    /// Tags that are tied to the agent (comma-separated)
    /// </summary>
    public string? AgentTag { get; set; }

    /// <summary>
    /// Advanced query filter (e.g., "createdon:today", "ids:[1,2,3]")
    /// </summary>
    public string? Q { get; set; }

    /// <summary>
    /// Filters results by any string associated with agent's name or email
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// Sorting order (e.g., "name desc", "displayName", "userId", "emailId", "lastActivityOn", "createdon", "lastmodifiedon")
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Unique IDs of the brands (comma-separated)
    /// </summary>
    public string? BrandIds { get; set; }

    /// <summary>
    /// Ticket access scope ID
    /// </summary>
    public int? TicketAccessScopeId { get; set; }
}

/// <summary>
/// Query parameters for getting agents by group
/// </summary>
public class AgentGroupQueryParameters
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public bool RequiresCounts { get; set; } = true;
    public string? Filter { get; set; }
    public string? OrderBy { get; set; }
    public int? GroupId { get; set; }
    public int? RoleId { get; set; }
    public long? ShiftId { get; set; }
    public string? UserId { get; set; }
    public string? ExclusionIds { get; set; }
}
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Service for managing BoldDesk agents
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Lists agents in the organization
    /// </summary>
    Task<BoldDeskResponse<AgentDetail>> GetAgentsAsync(AgentQueryParameters? parameters = null);

    /// <summary>
    /// Gets all agents using pagination
    /// </summary>
    IAsyncEnumerable<AgentDetail> GetAllAgentsAsync(AgentQueryParameters? parameters = null, IProgress<string>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific agent by ID
    /// </summary>
    Task<AgentDetail> GetAgentAsync(long userId);

    /// <summary>
    /// Gets a specific agent by email
    /// </summary>
    Task<AgentDetail> GetAgentByEmailAsync(string email);

    /// <summary>
    /// Adds a new agent
    /// </summary>
    Task<AgentOperationResponse> AddAgentAsync(AddAgentRequest request, bool skipDependencyValidation = true);

    /// <summary>
    /// Updates an existing agent
    /// </summary>
    Task<AgentOperationResponse> UpdateAgentAsync(long userId, AddAgentRequest request);

    /// <summary>
    /// Updates specific agent fields
    /// </summary>
    Task<AgentOperationResponse> UpdateAgentFieldsAsync(long userId, UpdateAgentFieldsRequest request, bool skipDependencyValidation = false);

    /// <summary>
    /// Activates a deactivated agent
    /// </summary>
    Task<AgentOperationResponse> ActivateAgentAsync(long userId);

    /// <summary>
    /// Deactivates an active agent
    /// </summary>
    Task<AgentOperationResponse> DeactivateAgentAsync(long userId, DeactivateAgentRequest? request = null);

    /// <summary>
    /// Gets agents by group ID
    /// </summary>
    Task<BoldDeskResponse<AgentDetail>> GetAgentsByGroupAsync(AgentGroupQueryParameters? parameters = null);

    /// <summary>
    /// Gets the count of agents by status
    /// </summary>
    Task<List<AgentCountResponse>> GetAgentCountAsync();

    /// <summary>
    /// Updates agent availability status
    /// </summary>
    Task<AgentOperationResponse> UpdateAgentAvailabilityStatusAsync(long userId, int availabilityStatusId);
}
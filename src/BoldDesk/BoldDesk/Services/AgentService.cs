using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using BoldDesk.Exceptions;
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Implementation of BoldDesk agent operations
/// </summary>
public class AgentService : BaseService, IAgentService
{
    public AgentService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions) 
        : base(httpClient, baseUrl, jsonOptions)
    {
    }

    /// <summary>
    /// Lists agents in the organization
    /// </summary>
    public async Task<BoldDeskResponse<AgentDetail>> GetAgentsAsync(AgentQueryParameters? parameters = null)
    {
        parameters ??= new AgentQueryParameters();
        var url = BuildAgentsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<AgentDetail>>(url);
    }

    /// <summary>
    /// Gets all agents using pagination
    /// </summary>
    public async IAsyncEnumerable<AgentDetail> GetAllAgentsAsync(
        AgentQueryParameters? parameters = null, 
        IProgress<string>? progress = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new AgentQueryParameters();
        var currentPage = parameters.Page;
        var totalFetched = 0;
        var hasMorePages = true;

        while (hasMorePages && !cancellationToken.IsCancellationRequested)
        {
            await EnsureRateLimitCompliance();

            parameters.Page = currentPage;
            progress?.Report($"Fetching agent page {currentPage}...");
            
            var response = await GetAgentsAsync(parameters);
            
            if (response.Result.Count == 0)
            {
                hasMorePages = false;
                break;
            }

            foreach (var agent in response.Result)
            {
                totalFetched++;
                yield return agent;
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

            progress?.Report($"Fetched {totalFetched} agents so far...");
        }

        progress?.Report($"Completed. Total agents fetched: {totalFetched}");
    }

    /// <summary>
    /// Gets a specific agent by ID
    /// </summary>
    public async Task<AgentDetail> GetAgentAsync(long userId)
    {
        var url = $"{BaseUrl}/agents/{userId}";
        return await ExecuteRequestAsync<AgentDetail>(url);
    }

    /// <summary>
    /// Gets a specific agent by email
    /// </summary>
    public async Task<AgentDetail> GetAgentByEmailAsync(string email)
    {
        var url = $"{BaseUrl}/agents/{HttpUtility.UrlEncode(email)}";
        return await ExecuteRequestAsync<AgentDetail>(url);
    }

    /// <summary>
    /// Adds a new agent
    /// </summary>
    public async Task<AgentOperationResponse> AddAgentAsync(AddAgentRequest request, bool skipDependencyValidation = true)
    {
        var url = $"{BaseUrl}/agents?skipDependencyValidation={skipDependencyValidation.ToString().ToLower()}";
        
        try
        {
            var response = await HttpClient.PostAsJsonAsync(url, request, JsonOptions);
            
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AgentOperationResponse>(content, JsonOptions);
            
            return result ?? new AgentOperationResponse { Message = "Agent added successfully" };
        }
        catch (BoldDeskApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BoldDeskApiException($"Failed to add agent: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Updates an existing agent
    /// </summary>
    public async Task<AgentOperationResponse> UpdateAgentAsync(long userId, AddAgentRequest request)
    {
        var url = $"{BaseUrl}/agents/{userId}";
        
        try
        {
            var response = await HttpClient.PatchAsJsonAsync(url, request, JsonOptions);
            
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AgentOperationResponse>(content, JsonOptions);
            
            return result ?? new AgentOperationResponse { Message = "Agent updated successfully" };
        }
        catch (BoldDeskApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BoldDeskApiException($"Failed to update agent: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Updates specific agent fields
    /// </summary>
    public async Task<AgentOperationResponse> UpdateAgentFieldsAsync(long userId, UpdateAgentFieldsRequest request, bool skipDependencyValidation = false)
    {
        var url = $"{BaseUrl}/agents/{userId}/fields?skipDependencyValidation={skipDependencyValidation.ToString().ToLower()}";
        
        try
        {
            var response = await HttpClient.PatchAsJsonAsync(url, request, JsonOptions);
            
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AgentOperationResponse>(content, JsonOptions);
            
            return result ?? new AgentOperationResponse { Message = "Agent fields updated successfully" };
        }
        catch (BoldDeskApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BoldDeskApiException($"Failed to update agent fields: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Activates a deactivated agent
    /// </summary>
    public async Task<AgentOperationResponse> ActivateAgentAsync(long userId)
    {
        var url = $"{BaseUrl}/agents/{userId}/activate";
        
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            var response = await HttpClient.SendAsync(request);
            
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AgentOperationResponse>(content, JsonOptions);
            
            return result ?? new AgentOperationResponse { Message = "Agent activated successfully" };
        }
        catch (BoldDeskApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BoldDeskApiException($"Failed to activate agent: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deactivates an active agent
    /// </summary>
    public async Task<AgentOperationResponse> DeactivateAgentAsync(long userId, DeactivateAgentRequest? request = null)
    {
        var url = $"{BaseUrl}/agents/{userId}/deactivate";
        
        try
        {
            var response = request != null 
                ? await HttpClient.PatchAsJsonAsync(url, request, JsonOptions)
                : await HttpClient.PatchAsync(url, null);
            
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AgentOperationResponse>(content, JsonOptions);
            
            return result ?? new AgentOperationResponse { Message = "Agent deactivated successfully" };
        }
        catch (BoldDeskApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BoldDeskApiException($"Failed to deactivate agent: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets agents by group ID
    /// </summary>
    public async Task<BoldDeskResponse<AgentDetail>> GetAgentsByGroupAsync(AgentGroupQueryParameters? parameters = null)
    {
        parameters ??= new AgentGroupQueryParameters();
        var url = BuildAgentGroupUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<AgentDetail>>(url);
    }

    /// <summary>
    /// Gets the count of agents by status
    /// </summary>
    public async Task<List<AgentCountResponse>> GetAgentCountAsync()
    {
        var url = $"{BaseUrl}/agents/count";
        var response = await ExecuteRequestAsync<BoldDeskResponse<AgentCountResponse>>(url);
        return response.Result;
    }

    /// <summary>
    /// Updates agent availability status
    /// </summary>
    public async Task<AgentOperationResponse> UpdateAgentAvailabilityStatusAsync(long userId, int availabilityStatusId)
    {
        var url = $"{BaseUrl}/agents/{userId}/agent_availability/{availabilityStatusId}";
        
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            var response = await HttpClient.SendAsync(request);
            
            ParseRateLimitHeaders(response.Headers);
            
            if (!response.IsSuccessStatusCode)
            {
                await ThrowBoldDeskException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AgentOperationResponse>(content, JsonOptions);
            
            return result ?? new AgentOperationResponse { Message = "Availability status updated successfully" };
        }
        catch (BoldDeskApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BoldDeskApiException($"Failed to update availability status: {ex.Message}", ex);
        }
    }

    private string BuildAgentsUrl(AgentQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/agents");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (parameters.UserStatus.HasValue)
            query["UserStatus"] = parameters.UserStatus.Value.ToString();

        if (parameters.IsAvailable.HasValue)
            query["IsAvailable"] = parameters.IsAvailable.Value.ToString();

        if (!string.IsNullOrWhiteSpace(parameters.RoleId))
            query["RoleId"] = parameters.RoleId;

        if (parameters.IsVerifiedAgents.HasValue)
            query["IsVerifiedAgents"] = parameters.IsVerifiedAgents.Value.ToString();

        if (!string.IsNullOrWhiteSpace(parameters.AgentTag))
            query["AgentTag"] = parameters.AgentTag;

        if (!string.IsNullOrWhiteSpace(parameters.Q))
            query["Q"] = parameters.Q;

        if (!string.IsNullOrWhiteSpace(parameters.Filter))
            query["Filter"] = parameters.Filter;

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            query["OrderBy"] = parameters.OrderBy;

        if (!string.IsNullOrWhiteSpace(parameters.BrandIds))
            query["BrandIds"] = parameters.BrandIds;

        if (parameters.TicketAccessScopeId.HasValue)
            query["TicketAccessScopeId"] = parameters.TicketAccessScopeId.Value.ToString();

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildAgentGroupUrl(AgentGroupQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/agents/collections");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.Filter))
            query["Filter"] = parameters.Filter;

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            query["OrderBy"] = parameters.OrderBy;

        if (parameters.GroupId.HasValue)
            query["groupId"] = parameters.GroupId.Value.ToString();

        if (parameters.RoleId.HasValue)
            query["roleId"] = parameters.RoleId.Value.ToString();

        if (parameters.ShiftId.HasValue)
            query["shiftId"] = parameters.ShiftId.Value.ToString();

        if (!string.IsNullOrWhiteSpace(parameters.UserId))
            query["userId"] = parameters.UserId;

        if (!string.IsNullOrWhiteSpace(parameters.ExclusionIds))
            query["exclusionIds"] = parameters.ExclusionIds;

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}
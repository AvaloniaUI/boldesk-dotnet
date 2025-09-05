using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Request to add a new agent
/// </summary>
public class AddAgentRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("emailId")]
    public string EmailId { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("timeZoneId")]
    public int? TimeZoneId { get; set; }

    [JsonPropertyName("hasAllBrandAccess")]
    public bool? HasAllBrandAccess { get; set; }

    [JsonPropertyName("brandIds")]
    public string? BrandIds { get; set; }

    [JsonPropertyName("roleIds")]
    public string? RoleIds { get; set; }

    [JsonPropertyName("ticketAccessScopeId")]
    public int? TicketAccessScopeId { get; set; }

    [JsonPropertyName("isVerified")]
    public bool? IsVerified { get; set; }

    [JsonPropertyName("agentPhoneNo")]
    public string? AgentPhoneNo { get; set; }

    [JsonPropertyName("chatAccessScopeId")]
    public int? ChatAccessScopeId { get; set; }

    [JsonPropertyName("agentTag")]
    public string? AgentTag { get; set; }

    [JsonPropertyName("userCountryId")]
    public int? UserCountryId { get; set; }

    [JsonPropertyName("agentMobileNo")]
    public string? AgentMobileNo { get; set; }

    [JsonPropertyName("agentAddress")]
    public string? AgentAddress { get; set; }

    [JsonPropertyName("agentJobTitle")]
    public string? AgentJobTitle { get; set; }

    [JsonPropertyName("agentExternalReferenceId")]
    public string? AgentExternalReferenceId { get; set; }

    [JsonPropertyName("agentNotes")]
    public string? AgentNotes { get; set; }

    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }

    // Contact fields
    [JsonPropertyName("contactName")]
    public string? ContactName { get; set; }

    [JsonPropertyName("contactDisplayName")]
    public string? ContactDisplayName { get; set; }

    [JsonPropertyName("contactPhoneNo")]
    public string? ContactPhoneNo { get; set; }

    [JsonPropertyName("contactTag")]
    public string? ContactTag { get; set; }

    [JsonPropertyName("contactMobileNo")]
    public string? ContactMobileNo { get; set; }

    [JsonPropertyName("contactAddress")]
    public string? ContactAddress { get; set; }

    [JsonPropertyName("contactJobTitle")]
    public string? ContactJobTitle { get; set; }

    [JsonPropertyName("contactExternalReferenceId")]
    public string? ContactExternalReferenceId { get; set; }

    [JsonPropertyName("contactNotes")]
    public string? ContactNotes { get; set; }
}

/// <summary>
/// Request to update agent fields
/// </summary>
public class UpdateAgentFieldsRequest
{
    [JsonPropertyName("fields")]
    public Dictionary<string, object> Fields { get; set; } = new();

    [JsonPropertyName("integrationAppId")]
    public int? IntegrationAppId { get; set; }
}

/// <summary>
/// Request to deactivate an agent
/// </summary>
public class DeactivateAgentRequest
{
    [JsonPropertyName("newAgentId")]
    public long? NewAgentId { get; set; }

    [JsonPropertyName("newGroupId")]
    public int? NewGroupId { get; set; }

    [JsonPropertyName("reassignGrouporAgent")]
    public bool? ReassignGroupOrAgent { get; set; }

    [JsonPropertyName("removeAgentOnly")]
    public bool? RemoveAgentOnly { get; set; }

    [JsonPropertyName("integrationAppId")]
    public int? IntegrationAppId { get; set; }

    [JsonPropertyName("newChatAgentId")]
    public long? NewChatAgentId { get; set; }

    [JsonPropertyName("newChatGroupId")]
    public int? NewChatGroupId { get; set; }
}

/// <summary>
/// Response for agent operations
/// </summary>
public class AgentOperationResponse
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Agent count response
/// </summary>
public class AgentCountResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
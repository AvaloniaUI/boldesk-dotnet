using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Detailed agent information from the API
/// </summary>
public class AgentDetail
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("emailId")]
    public string EmailId { get; set; } = string.Empty;

    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; }

    [JsonPropertyName("roles")]
    public List<AgentRole> Roles { get; set; } = new();

    [JsonPropertyName("groups")]
    public List<AgentGroup>? Groups { get; set; }

    [JsonPropertyName("brands")]
    public List<AgentBrand>? Brands { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("isBlocked")]
    public bool IsBlocked { get; set; }

    [JsonPropertyName("isVerified")]
    public bool IsVerified { get; set; }

    [JsonPropertyName("lastActivityOn")]
    public DateTime? LastActivityOn { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("lastModifiedOn")]
    public DateTime LastModifiedOn { get; set; }

    [JsonPropertyName("shortCode")]
    public string? ShortCode { get; set; }

    [JsonPropertyName("colorCode")]
    public string? ColorCode { get; set; }

    [JsonPropertyName("hasAllBrandAccess")]
    public bool HasAllBrandAccess { get; set; }

    [JsonPropertyName("ticketAccessScope")]
    public string? TicketAccessScope { get; set; }

    [JsonPropertyName("ticketAccessScopeId")]
    public int? TicketAccessScopeId { get; set; }

    [JsonPropertyName("chatAccessScope")]
    public string? ChatAccessScope { get; set; }

    [JsonPropertyName("chatAccessScopeId")]
    public int? ChatAccessScopeId { get; set; }

    [JsonPropertyName("availabilityStatus")]
    public AvailabilityStatus? AvailabilityStatus { get; set; }

    [JsonPropertyName("timezone")]
    public Timezone? Timezone { get; set; }

    [JsonPropertyName("language")]
    public Language? Language { get; set; }

    [JsonPropertyName("isTfaEnabled")]
    public bool IsTfaEnabled { get; set; }

    [JsonPropertyName("ticketLimit")]
    public int TicketLimit { get; set; }

    [JsonPropertyName("profileImageUrl")]
    public string? ProfileImageUrl { get; set; }

    [JsonPropertyName("phoneNo")]
    public string? PhoneNo { get; set; }

    [JsonPropertyName("mobileNo")]
    public string? MobileNo { get; set; }

    [JsonPropertyName("jobTitle")]
    public string? JobTitle { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("externalReferenceId")]
    public string? ExternalReferenceId { get; set; }

    [JsonPropertyName("agentTag")]
    public List<IdNamePair>? AgentTag { get; set; }

    [JsonPropertyName("userCountryId")]
    [JsonConverter(typeof(NullableIntConverter))]
    public int? UserCountryId { get; set; }

    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }

    // Contact fields (when agent also has contact info)
    [JsonPropertyName("contactName")]
    public string? ContactName { get; set; }

    [JsonPropertyName("contactDisplayName")]
    public string? ContactDisplayName { get; set; }

    [JsonPropertyName("contactPhoneNo")]
    public string? ContactPhoneNo { get; set; }

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

    [JsonPropertyName("contactTag")]
    public List<IdNamePair>? ContactTag { get; set; }
}

public class AgentRole
{
    [JsonPropertyName("roleId")]
    public int RoleId { get; set; }

    [JsonPropertyName("roleName")]
    public string RoleName { get; set; } = string.Empty;
}

public class AgentGroup
{
    [JsonPropertyName("groupId")]
    public int GroupId { get; set; }

    [JsonPropertyName("groupName")]
    public string GroupName { get; set; } = string.Empty;
}

public class AgentBrand
{
    [JsonPropertyName("brandId")]
    public int BrandId { get; set; }

    [JsonPropertyName("brandName")]
    public string BrandName { get; set; } = string.Empty;

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }

    [JsonPropertyName("hasAccess")]
    public bool HasAccess { get; set; }
}

public class AvailabilityStatus
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("colorCode")]
    public string? ColorCode { get; set; }

    [JsonPropertyName("agentAvailabilityStatusCategory")]
    public AvailabilityStatusCategory? AgentAvailabilityStatusCategory { get; set; }
}

public class AvailabilityStatusCategory
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class Timezone
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class Language
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
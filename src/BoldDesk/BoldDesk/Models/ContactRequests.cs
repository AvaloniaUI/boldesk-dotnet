using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Request model for creating a new contact
/// </summary>
public class CreateContactRequest
{
    [JsonPropertyName("contactName")]
    public string? ContactName { get; set; }

    [JsonPropertyName("emailId")]
    public string EmailId { get; set; } = string.Empty;

    [JsonPropertyName("secondaryEmailId")]
    public string? SecondaryEmailId { get; set; }

    [JsonPropertyName("contactDisplayName")]
    public string ContactDisplayName { get; set; } = string.Empty;

    [JsonPropertyName("contactPhoneNo")]
    public string? ContactPhoneNo { get; set; }

    [JsonPropertyName("contactMobileNo")]
    public string? ContactMobileNo { get; set; }

    [JsonPropertyName("contactAddress")]
    public string? ContactAddress { get; set; }

    [JsonPropertyName("contactJobTitle")]
    public string? ContactJobTitle { get; set; }

    [JsonPropertyName("timeZoneId")]
    public int? TimeZoneId { get; set; }

    [JsonPropertyName("languageId")]
    public int? LanguageId { get; set; }

    [JsonPropertyName("contactNotes")]
    public string? ContactNotes { get; set; }

    [JsonPropertyName("contactExternalReferenceId")]
    public string? ContactExternalReferenceId { get; set; }

    [JsonPropertyName("contactGroup")]
    public List<ContactGroupAssignmentRequest>? ContactGroup { get; set; }

    [JsonPropertyName("contactTag")]
    public string? ContactTag { get; set; }

    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }

    [JsonPropertyName("isVerified")]
    public bool? IsVerified { get; set; }

    [JsonPropertyName("userCountryId")]
    public int? UserCountryId { get; set; }
}

/// <summary>
/// Request model for updating an existing contact
/// </summary>
public class UpdateContactRequest
{
    [JsonPropertyName("fields")]
    public Dictionary<string, object> Fields { get; set; } = new();
}

/// <summary>
/// Request model for deleting contacts
/// </summary>
public class DeleteContactRequest
{
    [JsonPropertyName("contactId")]
    public long[] ContactId { get; set; } = Array.Empty<long>();

    [JsonPropertyName("isMarkTicketAsSpam")]
    public bool? IsMarkTicketAsSpam { get; set; }

    [JsonPropertyName("integrationAppId")]
    public int? IntegrationAppId { get; set; }
}

/// <summary>
/// Request model for permanent delete contacts
/// </summary>
public class PermanentDeleteContactRequest
{
    [JsonPropertyName("contactIdList")]
    public long[] ContactIdList { get; set; } = Array.Empty<long>();

    [JsonPropertyName("isForced")]
    public bool? IsForced { get; set; }
}

/// <summary>
/// Request model for contact group assignment
/// </summary>
public class ContactGroupAssignmentRequest
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("accessScopeId")]
    public int AccessScopeId { get; set; }

    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Request model for adding multiple contact groups to a contact
/// </summary>
public class AddContactGroupsRequest
{
    [JsonPropertyName("contactGroupId")]
    public long ContactGroupId { get; set; }

    [JsonPropertyName("accessScopeId")]
    public int AccessScopeId { get; set; }
}

/// <summary>
/// Request model for removing multiple contact groups from a contact
/// </summary>
public class RemoveContactGroupsRequest
{
    [JsonPropertyName("contactGroupIds")]
    public long[] ContactGroupIds { get; set; } = Array.Empty<long>();
}

/// <summary>
/// Request model for converting contact to agent
/// </summary>
public class ConvertContactToAgentRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("timeZoneId")]
    public int? TimeZoneId { get; set; }

    [JsonPropertyName("hasAllBrandAccess")]
    public bool? HasAllBrandAccess { get; set; }

    [JsonPropertyName("brandIds")]
    public string? BrandIds { get; set; }

    [JsonPropertyName("roleIds")]
    public string RoleIds { get; set; } = string.Empty;

    [JsonPropertyName("ticketAccessScopeId")]
    public int? TicketAccessScopeId { get; set; }

    [JsonPropertyName("agentPhoneNo")]
    public string? AgentPhoneNo { get; set; }

    [JsonPropertyName("chatAccessScopeId")]
    public int? ChatAccessScopeId { get; set; }

    [JsonPropertyName("ticketGroup")]
    public List<AddAgentGroupObject>? TicketGroup { get; set; }

    [JsonPropertyName("chatGroup")]
    public List<AddAgentGroupObject>? ChatGroup { get; set; }

    [JsonPropertyName("contactTag")]
    public string? ContactTag { get; set; }

    [JsonPropertyName("userCountryId")]
    public int? UserCountryId { get; set; }

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

    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }
}

/// <summary>
/// Agent group object for contact to agent conversion
/// </summary>
public class AddAgentGroupObject
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
}

/// <summary>
/// Request model for merging contacts
/// </summary>
public class MergeContactRequest
{
    [JsonPropertyName("primaryContactId")]
    public long PrimaryContactId { get; set; }

    [JsonPropertyName("secondaryContactIdList")]
    public long[] SecondaryContactIdList { get; set; } = Array.Empty<long>();
}

/// <summary>
/// Request model for contact note operations
/// </summary>
public class ContactNoteRequest
{
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("attachments")]
    public string? Attachments { get; set; }
}
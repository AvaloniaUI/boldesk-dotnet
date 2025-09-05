using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Represents a contact group in BoldDesk
/// </summary>
public class ContactGroup
{
    [JsonPropertyName("contactGroupId")]
    public long ContactGroupId { get; set; }

    [JsonPropertyName("contactGroupName")]
    public string ContactGroupName { get; set; } = string.Empty;

    [JsonPropertyName("shortCode")]
    public string? ShortCode { get; set; }

    [JsonPropertyName("colorCode")]
    public string? ColorCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("externalReferenceId")]
    public string? ExternalReferenceId { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("lastModifiedOn")]
    public DateTime LastModifiedOn { get; set; }

    [JsonPropertyName("contactGroupCustomFields")]
    public Dictionary<string, object?>? ContactGroupCustomFields { get; set; }
}

/// <summary>
/// Detailed contact group information
/// </summary>
public class ContactGroupDetail : ContactGroup
{
    [JsonPropertyName("contactGroupDescription")]
    public string? ContactGroupDescription { get; set; }

    [JsonPropertyName("contactGroupNotes")]
    public string? ContactGroupNotes { get; set; }

    [JsonPropertyName("contactGroupAddress")]
    public string? ContactGroupAddress { get; set; }

    [JsonPropertyName("contactGroupTag")]
    public List<TagInfo>? ContactGroupTag { get; set; }

    [JsonPropertyName("contactGroupDomain")]
    public List<DomainInfo>? ContactGroupDomain { get; set; }

    [JsonPropertyName("contactGroupExternalReferenceId")]
    public string? ContactGroupExternalReferenceId { get; set; }

    [JsonPropertyName("customFields")]
    public Dictionary<string, object?>? CustomFields { get; set; }

    [JsonPropertyName("savedItemId")]
    public long? SavedItemId { get; set; }

    [JsonPropertyName("dataToken")]
    public string? DataToken { get; set; }
}

/// <summary>
/// Tag information
/// </summary>
public class TagInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Domain information
/// </summary>
public class DomainInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Contact group note
/// </summary>
public class ContactGroupNote
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("hasAttachment")]
    public bool HasAttachment { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("updatedOn")]
    public DateTime UpdatedOn { get; set; }

    [JsonPropertyName("createdBy")]
    public NoteCreator? CreatedBy { get; set; }

    [JsonPropertyName("isEdited")]
    public bool IsEdited { get; set; }

    [JsonPropertyName("attachments")]
    public List<object>? Attachments { get; set; }
}

/// <summary>
/// Note creator information
/// </summary>
public class NoteCreator
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("emailId")]
    public string EmailId { get; set; } = string.Empty;

    [JsonPropertyName("shortCode")]
    public string? ShortCode { get; set; }

    [JsonPropertyName("colorCode")]
    public string? ColorCode { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("isVerified")]
    public bool IsVerified { get; set; }

    [JsonPropertyName("profileImageUrl")]
    public string? ProfileImageUrl { get; set; }

    [JsonPropertyName("isAgent")]
    public bool IsAgent { get; set; }
}

/// <summary>
/// Contact group field information
/// </summary>
public class ContactGroupField
{
    [JsonPropertyName("fieldId")]
    public int FieldId { get; set; }

    [JsonPropertyName("apiName")]
    public string ApiName { get; set; } = string.Empty;

    [JsonPropertyName("labelForAgentPortal")]
    public string LabelForAgentPortal { get; set; } = string.Empty;

    [JsonPropertyName("labelForCustomerPortal")]
    public string LabelForCustomerPortal { get; set; } = string.Empty;

    [JsonPropertyName("isDefaultSystemField")]
    public bool IsDefaultSystemField { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("isVisibleInCustomerPortal")]
    public bool IsVisibleInCustomerPortal { get; set; }

    [JsonPropertyName("fieldType")]
    public string FieldType { get; set; } = string.Empty;

    [JsonPropertyName("fieldTypeId")]
    public int FieldTypeId { get; set; }

    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("fieldTypeIconCssClass")]
    public string? FieldTypeIconCssClass { get; set; }

    [JsonPropertyName("canEditConfiguration")]
    public bool CanEditConfiguration { get; set; }
}
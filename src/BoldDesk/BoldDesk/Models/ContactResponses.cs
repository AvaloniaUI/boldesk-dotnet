using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Response model for contact operations that return an ID and message
/// </summary>
public class ContactOperationResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Response model for contact delete operations
/// </summary>
public class ContactDeleteResponse
{
    [JsonPropertyName("result")]
    public ContactDeleteResult Result { get; set; } = new();

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Result part of contact delete response
/// </summary>
public class ContactDeleteResult
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }
}

/// <summary>
/// Response for operations that only return a message
/// </summary>
public class ContactMessageResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Response for adding contact groups to contact
/// </summary>
public class AddContactGroupResponse
{
    [JsonPropertyName("result")]
    public AddContactGroupResult Result { get; set; } = new();

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Result for adding contact groups to contact
/// </summary>
public class AddContactGroupResult
{
    [JsonPropertyName("contactGroupId")]
    public long ContactGroupId { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// Response for removing contact groups from contact
/// </summary>
public class RemoveContactGroupResponse
{
    [JsonPropertyName("result")]
    public List<RemoveContactGroupResult> Result { get; set; } = new();

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Result for removing contact groups from contact
/// </summary>
public class RemoveContactGroupResult
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// Response model for contact notes list
/// </summary>
public class ContactNotesResponse
{
    [JsonPropertyName("contactNoteObjects")]
    public List<ContactNote> ContactNoteObjects { get; set; } = new();

    [JsonPropertyName("totalListCount")]
    public int TotalListCount { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("isValidationError")]
    public bool IsValidationError { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errorList")]
    public object? ErrorList { get; set; }
}

/// <summary>
/// Contact note model
/// </summary>
public class ContactNote
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
    public ContactNoteUser? CreatedBy { get; set; }

    [JsonPropertyName("isEdited")]
    public bool IsEdited { get; set; }

    [JsonPropertyName("attachments")]
    public List<ContactNoteAttachment>? Attachments { get; set; }
}

/// <summary>
/// User who created/updated a contact note
/// </summary>
public class ContactNoteUser
{
    [JsonPropertyName("agentShiftId")]
    public int AgentShiftId { get; set; }

    [JsonPropertyName("agentShiftName")]
    public string? AgentShiftName { get; set; }

    [JsonPropertyName("ticketLimit")]
    public int TicketLimit { get; set; }

    [JsonPropertyName("chatLimit")]
    public int ChatLimit { get; set; }

    [JsonPropertyName("emailId")]
    public string EmailId { get; set; } = string.Empty;

    [JsonPropertyName("shortCode")]
    public string? ShortCode { get; set; }

    [JsonPropertyName("colorCode")]
    public string? ColorCode { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("isVerified")]
    public bool IsVerified { get; set; }

    [JsonPropertyName("profileImageUrl")]
    public string? ProfileImageUrl { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("isAgent")]
    public bool IsAgent { get; set; }
}

/// <summary>
/// Attachment in a contact note
/// </summary>
public class ContactNoteAttachment
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("extension")]
    public string Extension { get; set; } = string.Empty;

    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("fileUrl")]
    public string FileUrl { get; set; } = string.Empty;

    [JsonPropertyName("cloudStorageType")]
    public string? CloudStorageType { get; set; }

    [JsonPropertyName("isExternalFile")]
    public bool IsExternalFile { get; set; }

    [JsonPropertyName("updatedBy")]
    public ContactNoteUser? UpdatedBy { get; set; }
}

/// <summary>
/// Contact field model
/// </summary>
public class ContactField
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

    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("fieldTypeId")]
    public int FieldTypeId { get; set; }

    [JsonPropertyName("fieldTypeIconCssClass")]
    public string? FieldTypeIconCssClass { get; set; }

    [JsonPropertyName("canEditConfiguration")]
    public bool CanEditConfiguration { get; set; }

    [JsonPropertyName("isPrimaryField")]
    public bool IsPrimaryField { get; set; }

    [JsonPropertyName("isFixedField")]
    public bool IsFixedField { get; set; }
}
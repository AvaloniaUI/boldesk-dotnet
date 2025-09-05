using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Request to add a new contact group
/// </summary>
public class AddContactGroupRequest
{
    [JsonPropertyName("contactGroupName")]
    public string ContactGroupName { get; set; } = string.Empty;

    [JsonPropertyName("contactGroupDescription")]
    public string? ContactGroupDescription { get; set; }

    [JsonPropertyName("contactGroupNotes")]
    public string? ContactGroupNotes { get; set; }

    [JsonPropertyName("contactGroupExternalReferenceId")]
    public string? ContactGroupExternalReferenceId { get; set; }

    [JsonPropertyName("contactGroupAddress")]
    public string? ContactGroupAddress { get; set; }

    [JsonPropertyName("contactGroupTag")]
    public string? ContactGroupTag { get; set; }

    [JsonPropertyName("contactGroupDomain")]
    public string? ContactGroupDomain { get; set; }

    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }
}

/// <summary>
/// Request to update contact group fields
/// </summary>
public class UpdateContactGroupFieldsRequest
{
    [JsonPropertyName("fields")]
    public Dictionary<string, object> Fields { get; set; } = new();
}

/// <summary>
/// Request to delete contact groups
/// </summary>
public class DeleteContactGroupsRequest
{
    [JsonPropertyName("contactGroupIds")]
    public List<long> ContactGroupIds { get; set; } = new();
}

/// <summary>
/// Request to add contacts to a contact group
/// </summary>
public class AddContactToGroupRequest
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("accessScopeId")]
    public int AccessScopeId { get; set; }
}

/// <summary>
/// Request to add or update a contact group note
/// </summary>
public class ContactGroupNoteRequest
{
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("attachments")]
    public string? Attachments { get; set; }
}

/// <summary>
/// Response for contact group operations
/// </summary>
public class ContactGroupOperationResponse
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("isSuccess")]
    public bool? IsSuccess { get; set; }
}

/// <summary>
/// Response for delete operations
/// </summary>
public class ContactGroupDeleteResponse
{
    [JsonPropertyName("result")]
    public DeleteResult? Result { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Delete operation result
/// </summary>
public class DeleteResult
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }
}

/// <summary>
/// Response for adding contacts to group
/// </summary>
public class AddContactToGroupResponse
{
    [JsonPropertyName("result")]
    public AddContactResult? Result { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Result of adding a contact to a group
/// </summary>
public class AddContactResult
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// Response for contact group notes
/// </summary>
public class ContactGroupNotesResponse
{
    [JsonPropertyName("contactGroupNotesObject")]
    public List<ContactGroupNote> ContactGroupNotesObject { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}
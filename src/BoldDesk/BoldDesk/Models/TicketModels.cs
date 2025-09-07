using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Represents a ticket message/update
/// </summary>
public class TicketMessage
{
    [JsonPropertyName("messageId")]
    public int MessageId { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("fromUserId")]
    public int FromUserId { get; set; }

    [JsonPropertyName("fromUser")]
    public User? FromUser { get; set; }

    [JsonPropertyName("toUserIds")]
    public List<int>? ToUserIds { get; set; }

    [JsonPropertyName("ccUserIds")]
    public List<int>? CcUserIds { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("lastModifiedOn")]
    public DateTime? LastModifiedOn { get; set; }

    [JsonPropertyName("messageType")]
    public string? MessageType { get; set; }

    [JsonPropertyName("messageTypeId")]
    public int? MessageTypeId { get; set; }

    [JsonPropertyName("attachments")]
    public List<TicketAttachment>? Attachments { get; set; }

    [JsonPropertyName("messageTags")]
    public List<string>? MessageTags { get; set; }
}

/// <summary>
/// Query parameters for ticket messages
/// </summary>
public class TicketMessageQueryParameters
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 100;
    public bool RequiresCounts { get; set; } = true;
    public string? OrderBy { get; set; }
    public List<int>? MessageTypeIds { get; set; }
    public List<int>? MessageTagIds { get; set; }
    public bool? IsFirstUpdateRequired { get; set; }
    public int? AttachmentsCount { get; set; }
}

/// <summary>
/// Represents a ticket note
/// </summary>
public class TicketNote
{
    [JsonPropertyName("noteId")]
    public int NoteId { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("fromUserId")]
    public int FromUserId { get; set; }

    [JsonPropertyName("fromUser")]
    public User? FromUser { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("atMentionedUserIds")]
    public List<int>? AtMentionedUserIds { get; set; }
}

/// <summary>
/// Represents a ticket attachment
/// </summary>
public class TicketAttachment
{
    [JsonPropertyName("attachmentId")]
    public int AttachmentId { get; set; }

    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("createdBy")]
    public int CreatedBy { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// Represents a tag
/// </summary>
public class Tag
{
    [JsonPropertyName("tagId")]
    public int TagId { get; set; }

    [JsonPropertyName("tagName")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("color")]
    public string? Color { get; set; }
}

/// <summary>
/// Represents a ticket watcher
/// </summary>
public class TicketWatcher
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("user")]
    public User? User { get; set; }

    [JsonPropertyName("addedOn")]
    public DateTime AddedOn { get; set; }
}

/// <summary>
/// Represents a linked ticket
/// </summary>
public class LinkedTicket
{
    [JsonPropertyName("linkId")]
    public int LinkId { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("linkedTicketId")]
    public int LinkedTicketId { get; set; }

    [JsonPropertyName("linkType")]
    public string LinkType { get; set; } = string.Empty;

    [JsonPropertyName("linkedTicketData")]
    public Ticket? LinkedTicketData { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Represents ticket sharing details
/// </summary>
public class TicketShare
{
    [JsonPropertyName("shareId")]
    public int ShareId { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("accessScopeId")]
    public int AccessScopeId { get; set; }

    [JsonPropertyName("sharedWith")]
    public List<SharedWithEntity>? SharedWith { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Entity that a ticket is shared with
/// </summary>
public class SharedWithEntity
{
    [JsonPropertyName("isAgent")]
    public bool IsAgent { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>
/// Represents a ticket source
/// </summary>
public class TicketSource
{
    [JsonPropertyName("sourceId")]
    public int SourceId { get; set; }

    [JsonPropertyName("sourceName")]
    public string SourceName { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}

/// <summary>
/// Represents a ticket field
/// </summary>
public class TicketField
{
    [JsonPropertyName("fieldId")]
    public int FieldId { get; set; }

    [JsonPropertyName("fieldName")]
    public string FieldName { get; set; } = string.Empty;

    [JsonPropertyName("fieldType")]
    public string FieldType { get; set; } = string.Empty;

    [JsonPropertyName("isRequired")]
    public bool IsRequired { get; set; }

    [JsonPropertyName("isVisible")]
    public bool IsVisible { get; set; }

    [JsonPropertyName("options")]
    public List<TicketFieldOption>? Options { get; set; }
}

/// <summary>
/// Represents a ticket field option
/// </summary>
public class TicketFieldOption
{
    [JsonPropertyName("optionId")]
    public int OptionId { get; set; }

    [JsonPropertyName("optionValue")]
    public string OptionValue { get; set; } = string.Empty;
}

/// <summary>
/// Represents a ticket form
/// </summary>
public class TicketForm
{
    [JsonPropertyName("formId")]
    public int FormId { get; set; }

    [JsonPropertyName("formName")]
    public string FormName { get; set; } = string.Empty;

    [JsonPropertyName("fields")]
    public List<TicketField>? Fields { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Represents ticket history
/// </summary>
public class TicketHistory
{
    [JsonPropertyName("historyId")]
    public int HistoryId { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("field")]
    public string? Field { get; set; }

    [JsonPropertyName("oldValue")]
    public string? OldValue { get; set; }

    [JsonPropertyName("newValue")]
    public string? NewValue { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("user")]
    public User? User { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Query parameters for ticket history
/// </summary>
public class TicketHistoryQueryParameters
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 100;
    public bool RequiresCounts { get; set; } = true;
    public string? OrderBy { get; set; }
    public DateTime? UpdatedFromDate { get; set; }
    public DateTime? UpdatedToDate { get; set; }
}

/// <summary>
/// Represents a user in ticket context
/// </summary>
public class User
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("profileImageUrl")]
    public string? ProfileImageUrl { get; set; }

    [JsonPropertyName("isAgent")]
    public bool IsAgent { get; set; }
}

/// <summary>
/// Represents ticket metrics
/// </summary>
public class TicketMetrics
{
    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("firstResponseTime")]
    public TimeSpan? FirstResponseTime { get; set; }

    [JsonPropertyName("resolutionTime")]
    public TimeSpan? ResolutionTime { get; set; }

    [JsonPropertyName("responseCount")]
    public int ResponseCount { get; set; }

    [JsonPropertyName("reopenCount")]
    public int ReopenCount { get; set; }

    [JsonPropertyName("agentInteractions")]
    public int AgentInteractions { get; set; }

    [JsonPropertyName("customerInteractions")]
    public int CustomerInteractions { get; set; }
}

/// <summary>
/// Represents a ticket web link
/// </summary>
public class TicketWebLink
{
    [JsonPropertyName("linkId")]
    public int LinkId { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("createdBy")]
    public int CreatedBy { get; set; }
}

/// <summary>
/// Represents a ticket article link
/// </summary>
public class TicketArticleLink
{
    [JsonPropertyName("linkId")]
    public int LinkId { get; set; }

    [JsonPropertyName("articleId")]
    public int ArticleId { get; set; }

    [JsonPropertyName("articleTitle")]
    public string? ArticleTitle { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Represents a related contact
/// </summary>
public class RelatedContact
{
    [JsonPropertyName("linkId")]
    public int LinkId { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("contact")]
    public Contact? Contact { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Represents email delivery log
/// </summary>
public class EmailDeliveryLog
{
    [JsonPropertyName("logId")]
    public int LogId { get; set; }

    [JsonPropertyName("messageId")]
    public int MessageId { get; set; }

    [JsonPropertyName("recipient")]
    public string Recipient { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("sentOn")]
    public DateTime? SentOn { get; set; }
}

/// <summary>
/// Represents permanent delete log
/// </summary>
public class PermanentDeleteLog
{
    [JsonPropertyName("logId")]
    public int LogId { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("deletedBy")]
    public int DeletedBy { get; set; }

    [JsonPropertyName("deletedByUser")]
    public User? DeletedByUser { get; set; }

    [JsonPropertyName("deletedOn")]
    public DateTime DeletedOn { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// Represents a suspended email
/// </summary>
public class SuspendedEmail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    [JsonPropertyName("subject")]
    public string Subject { get; set; } = string.Empty;

    [JsonPropertyName("receivedOn")]
    public DateTime ReceivedOn { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Represents ticket update
/// </summary>
public class TicketUpdate
{
    [JsonPropertyName("updateId")]
    public int UpdateId { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }

    [JsonPropertyName("createdBy")]
    public int CreatedBy { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }
}

/// <summary>
/// Represents public messages statistics
/// </summary>
public class PublicMessagesStats
{
    [JsonPropertyName("minMessageId")]
    public int MinMessageId { get; set; }

    [JsonPropertyName("maxMessageId")]
    public int MaxMessageId { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("dateRange")]
    public DateRange? DateRange { get; set; }
}

/// <summary>
/// Represents a date range
/// </summary>
public class DateRange
{
    [JsonPropertyName("from")]
    public DateTime From { get; set; }

    [JsonPropertyName("to")]
    public DateTime To { get; set; }
}
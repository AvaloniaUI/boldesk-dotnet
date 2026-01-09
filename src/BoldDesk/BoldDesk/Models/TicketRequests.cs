using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Request model for creating a new ticket
/// </summary>
public class CreateTicketRequest
{
    [JsonPropertyName("subject")]
    public required string Title { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("requesterId")]
    public int? RequestedById { get; set; }

    [JsonPropertyName("requestedForId")]
    public int? RequestedForId { get; set; }

    [JsonPropertyName("ccUserIds")]
    public List<int>? CcUserIds { get; set; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("subCategoryId")]
    public int? SubCategoryId { get; set; }

    [JsonPropertyName("priorityId")]
    public int? PriorityId { get; set; }

    [JsonPropertyName("statusId")]
    public int? StatusId { get; set; }

    [JsonPropertyName("agentId")]
    public int? AgentId { get; set; }

    [JsonPropertyName("groupId")]
    public int? GroupId { get; set; }

    [JsonPropertyName("sourceId")]
    public int? SourceId { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }

    [JsonPropertyName("brandId")]
    public int? BrandId { get; set; }

    [JsonPropertyName("typeId")]
    public int? TypeId { get; set; }

    [JsonPropertyName("isSpam")]
    public bool? IsSpam { get; set; }

    [JsonPropertyName("productId")]
    public int? ProductId { get; set; }

    [JsonPropertyName("skipEmailNotification")]
    public bool? SkipEmailNotification { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }

    [JsonPropertyName("externalReferenceId")]
    public string? ExternalReferenceId { get; set; }

    [JsonPropertyName("ticketPortalValue")]
    public string? TicketPortalValue { get; set; }
}

/// <summary>
/// Request model for updating an existing ticket
/// </summary>
public class UpdateTicketRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("subCategoryId")]
    public int? SubCategoryId { get; set; }

    [JsonPropertyName("priorityId")]
    public int? PriorityId { get; set; }

    [JsonPropertyName("statusId")]
    public int? StatusId { get; set; }

    [JsonPropertyName("agentId")]
    public int? AgentId { get; set; }

    [JsonPropertyName("groupId")]
    public int? GroupId { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }

    [JsonPropertyName("typeId")]
    public int? TypeId { get; set; }

    [JsonPropertyName("productId")]
    public int? ProductId { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }

    [JsonPropertyName("externalReferenceId")]
    public string? ExternalReferenceId { get; set; }

    [JsonPropertyName("skipEmailNotification")]
    public bool? SkipEmailNotification { get; set; }
}

/// <summary>
/// Request model for replying to a ticket
/// </summary>
public class ReplyTicketRequest
{
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("fromUserId")]
    public int? FromUserId { get; set; }

    [JsonPropertyName("toUserIds")]
    public List<int>? ToUserIds { get; set; }

    [JsonPropertyName("cc")]
    public List<int>? CcUserIds { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool? IsPrivate { get; set; }

    [JsonPropertyName("skipEmailNotification")]
    public bool? SkipEmailNotification { get; set; }

    [JsonPropertyName("atMentionedUserIds")]
    public List<int>? AtMentionedUserIds { get; set; }

    [JsonPropertyName("replyOnBehalfOfRequester")]
    public bool? ReplyOnBehalfOfRequester { get; set; }

    [JsonPropertyName("updatedByuserIdorEmailId")]
    public string? UpdatedByUserIdOrEmailId { get; set; }

    /// <summary>
    /// Comma-separated attachment tokens obtained from the upload attachment API
    /// </summary>
    [JsonPropertyName("attachments")]
    public string? Attachments { get; set; }
}

/// <summary>
/// Request model for adding a note to a ticket
/// </summary>
public class AddTicketNoteRequest
{
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("fromUserId")]
    public int? FromUserId { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool? IsPrivate { get; set; } = true;

    [JsonPropertyName("atMentionedUserIds")]
    public List<int>? AtMentionedUserIds { get; set; }
}

/// <summary>
/// Request model for merging tickets
/// </summary>
public class MergeTicketsRequest
{
    [JsonPropertyName("primaryTicketId")]
    public required int PrimaryTicketId { get; set; }

    [JsonPropertyName("secondaryTicketIds")]
    public required List<int> SecondaryTicketIds { get; set; }

    [JsonPropertyName("secondaryTicketNote")]
    public string? SecondaryTicketNote { get; set; }

    [JsonPropertyName("isSecondaryTicketNotePrivate")]
    public bool? IsSecondaryTicketNotePrivate { get; set; }

    [JsonPropertyName("primaryTicketNote")]
    public string? PrimaryTicketNote { get; set; }

    [JsonPropertyName("isPrimaryTicketNotePrivate")]
    public bool? IsPrimaryTicketNotePrivate { get; set; }

    [JsonPropertyName("includeSecondaryTicketCCs")]
    public bool? IncludeSecondaryTicketCCs { get; set; }

    [JsonPropertyName("includeSecondaryTicketRequester")]
    public bool? IncludeSecondaryTicketRequester { get; set; }

    [JsonPropertyName("isLinkTicketsAsRelatedTickets")]
    public bool? IsLinkTicketsAsRelatedTickets { get; set; }

    [JsonPropertyName("isCloseSecondaryTickets")]
    public bool? IsCloseSecondaryTickets { get; set; }

    [JsonPropertyName("isMergeSecondaryTicketsFirstUpdate")]
    public bool? IsMergeSecondaryTicketsFirstUpdate { get; set; }

    [JsonPropertyName("skipEmailNotification")]
    public bool? SkipEmailNotification { get; set; }
}

/// <summary>
/// Request model for splitting tickets
/// </summary>
public class SplitTicketRequest
{
    [JsonPropertyName("messageIds")]
    public required List<int> MessageIds { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("agentId")]
    public int? AgentId { get; set; }

    [JsonPropertyName("groupId")]
    public int? GroupId { get; set; }

    [JsonPropertyName("statusId")]
    public int? StatusId { get; set; }

    [JsonPropertyName("priorityId")]
    public int? PriorityId { get; set; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("subCategoryId")]
    public int? SubCategoryId { get; set; }

    [JsonPropertyName("customFields")]
    public Dictionary<string, object>? CustomFields { get; set; }

    [JsonPropertyName("isLinkAsRelatedTicket")]
    public bool? IsLinkAsRelatedTicket { get; set; }
}

/// <summary>
/// Request model for sharing tickets
/// </summary>
public class ShareTicketRequest
{
    [JsonPropertyName("accessScopeId")]
    public required int AccessScopeId { get; set; }

    [JsonPropertyName("agentGroupList")]
    public List<AgentGroupShare>? AgentGroupList { get; set; }

    [JsonPropertyName("skipEmailNotification")]
    public bool? SkipEmailNotification { get; set; }
}

/// <summary>
/// Agent or group to share with
/// </summary>
public class AgentGroupShare
{
    [JsonPropertyName("isAgent")]
    public bool IsAgent { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }
}

/// <summary>
/// Request model for adding tags to a ticket
/// </summary>
public class AddTagsRequest
{
    [JsonPropertyName("tags")]
    public required List<string> Tags { get; set; }
}

/// <summary>
/// Request model for removing tags from a ticket
/// </summary>
public class RemoveTagsRequest
{
    [JsonPropertyName("tags")]
    public required List<string> Tags { get; set; }
}

/// <summary>
/// Request model for adding watchers to a ticket
/// </summary>
public class AddWatchersRequest
{
    [JsonPropertyName("userIds")]
    public required List<int> UserIds { get; set; }
}

/// <summary>
/// Request model for removing watchers from a ticket
/// </summary>
public class RemoveWatchersRequest
{
    [JsonPropertyName("userIds")]
    public required List<int> UserIds { get; set; }
}

/// <summary>
/// Request model for adding a web link to a ticket
/// </summary>
public class AddWebLinkRequest
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// Request model for linking tickets
/// </summary>
public class LinkTicketsRequest
{
    [JsonPropertyName("linkType")]
    public required string LinkType { get; set; } // "related", "child", etc.

    [JsonPropertyName("linkedTicketIds")]
    public required List<int> LinkedTicketIds { get; set; }
}

/// <summary>
/// Request for bulk ticket operations
/// </summary>
public class BulkTicketIdsRequest
{
    [JsonPropertyName("ticketIdList")]
    public required List<int> TicketIdList { get; set; }
}

/// <summary>
/// Request model for updating message tags
/// </summary>
public class UpdateMessageTagRequest
{
    [JsonPropertyName("tagName")]
    public required string TagName { get; set; }
}

/// <summary>
/// Request model for adding related contacts
/// </summary>
public class AddRelatedContactRequest
{
    [JsonPropertyName("userIds")]
    public required List<int> UserIds { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for editing a message
/// </summary>
public class EditMessageRequest
{
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("atMentionedUserIds")]
    public List<int>? AtMentionedUserIds { get; set; }
}

/// <summary>
/// Request model for converting link type
/// </summary>
public class ConvertLinkTypeRequest
{
    [JsonPropertyName("newLinkType")]
    public required string NewLinkType { get; set; } // "related", "child", etc.
}

/// <summary>
/// Request model for adding an article link
/// </summary>
public class AddArticleLinkRequest
{
    [JsonPropertyName("articleId")]
    public required int ArticleId { get; set; }
}

/// <summary>
/// Request model for recovering suspended email
/// </summary>
public class RecoverSuspendedEmailRequest
{
    [JsonPropertyName("suspendedEmailId")]
    public required int SuspendedEmailId { get; set; }

    [JsonPropertyName("ticketId")]
    public int? TicketId { get; set; }

    [JsonPropertyName("createNewTicket")]
    public bool CreateNewTicket { get; set; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("priorityId")]
    public int? PriorityId { get; set; }

    [JsonPropertyName("agentId")]
    public int? AgentId { get; set; }
}

/// <summary>
/// Request for searching tickets for linking
/// </summary>
public class SearchTicketForLinkingRequest
{
    [JsonPropertyName("searchText")]
    public required string SearchText { get; set; }

    [JsonPropertyName("excludeTicketId")]
    public int? ExcludeTicketId { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; } = 10;
}

/// <summary>
/// Query parameters for ticket updates
/// </summary>
public class TicketUpdatesQueryParameters
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 100;
    public bool RequiresCounts { get; set; } = true;
    public string? OrderBy { get; set; }
    public bool? IsPublic { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

/// <summary>
/// Query parameters for ticket metrics
/// </summary>
public class TicketMetricsQueryParameters
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 100;
    public bool RequiresCounts { get; set; } = true;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<int>? TicketIds { get; set; }
}

/// <summary>
/// Query parameters for email delivery logs
/// </summary>
public class EmailDeliveryLogQueryParameters
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 100;
    public bool RequiresCounts { get; set; } = true;
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class Ticket
{
    private int _ticketId;
    private bool _ticketIdExplicitlySet;

    // Primary mapping used by list APIs - this is the public-facing ticket ID
    [JsonPropertyName("ticketId")]
    public int TicketId
    {
        get => _ticketId;
        set
        {
            _ticketId = value;
            _ticketIdExplicitlySet = true;
        }
    }

    // Some endpoints may return "id" instead of "ticketId" for single ticket.
    // Only use this as a fallback if ticketId wasn't explicitly set.
    [JsonPropertyName("id")]
    public int? Id
    {
        get => _ticketId;
        set
        {
            // Only set if ticketId wasn't already provided - ticketId takes precedence
            if (value.HasValue && !_ticketIdExplicitlySet)
                _ticketId = value.Value;
        }
    }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("ticketStatusCategoryId")]
    public int TicketStatusCategoryId { get; set; }

    [JsonPropertyName("agent")]
    public Agent? Agent { get; set; }

    [JsonPropertyName("group")]
    public Group? Group { get; set; }

    [JsonPropertyName("category")]
    public Category? Category { get; set; }

    [JsonPropertyName("status")]
    public Status? Status { get; set; }

    [JsonPropertyName("priority")]
    public Priority? Priority { get; set; }

    [JsonPropertyName("resolutionDue")]
    public DateTime? ResolutionDue { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("lastUpdatedOn")]
    public DateTime? LastUpdatedOn { get; set; }

    [JsonPropertyName("brand")]
    public object? Brand { get; set; }

    [JsonPropertyName("brandId")]
    public int? BrandId { get; set; }

    [JsonPropertyName("mode")]
    public object? Mode { get; set; }

    // Map type.name to Mode if provided (some APIs use "type")
    [JsonPropertyName("type")]
    public IdName? Type { get; set; }

    [JsonPropertyName("isVisibleToCustomer")]
    public bool IsVisibleToCustomer { get; set; }

    [JsonPropertyName("updatesCount")]
    public int? UpdatesCount { get; set; }

    // Note: API uses "attachmentCount" not "attachmentsCount"
    [JsonPropertyName("attachmentCount")]
    public int AttachmentsCount { get; set; }

    [JsonPropertyName("sourceId")]
    public int? SourceId { get; set; }

    [JsonPropertyName("source")]
    public object? Source { get; set; }

    [JsonPropertyName("lastRepliedOn")]
    public DateTime? LastRepliedOn { get; set; }

    private RequestedBy? _requestedBy;
    [JsonPropertyName("requestedBy")]
    public RequestedBy? RequestedBy { get => _requestedBy; set => _requestedBy = value; }

    // Alternative shape uses "requester"
    [JsonPropertyName("requester")]
    public RequestedBy? Requester { get => _requestedBy; set => _requestedBy = value; }

    [JsonPropertyName("isSpamOrDeleted")]
    public bool? IsSpamOrDeleted { get; set; }

    // Additional fields from actual API response
    [JsonPropertyName("statusSortOrder")]
    public int? StatusSortOrder { get; set; }

    [JsonPropertyName("prioritySortOrder")]
    public int? PrioritySortOrder { get; set; }

    [JsonPropertyName("ticketLastRepliedByUserTypeId")]
    public int? TicketLastRepliedByUserTypeId { get; set; }

    [JsonPropertyName("isBrandActive")]
    public bool? IsBrandActive { get; set; }

    [JsonPropertyName("isCustomerPortalActive")]
    public bool? IsCustomerPortalActive { get; set; }

    [JsonPropertyName("lockedBy")]
    public object? LockedBy { get; set; }

    [JsonPropertyName("isTicketLocked")]
    public bool? IsTicketLocked { get; set; }

    [JsonPropertyName("contactGroup")]
    public IdName? ContactGroup { get; set; }

    [JsonPropertyName("ticketStateId")]
    public int? TicketStateId { get; set; }

    [JsonPropertyName("totalTimeLogged")]
    public int? TotalTimeLogged { get; set; }

    [JsonPropertyName("billableTimeLogged")]
    public int? BillableTimeLogged { get; set; }

    [JsonPropertyName("createdBy")]
    public IdName? CreatedBy { get; set; }

    [JsonPropertyName("lastModifiedBy")]
    public IdName? LastModifiedBy { get; set; }

    [JsonPropertyName("ticketApprovalPendingCount")]
    public int? TicketApprovalPendingCount { get; set; }

    [JsonPropertyName("ticketApprovalRequestCount")]
    public int? TicketApprovalRequestCount { get; set; }

    [JsonPropertyName("ticketApprovalApprovedCount")]
    public int? TicketApprovalApprovedCount { get; set; }

    [JsonPropertyName("ticketApprovalRejectedCount")]
    public int? TicketApprovalRejectedCount { get; set; }

    [JsonPropertyName("hoursSinceRequesterResponded")]
    public DateTime? HoursSinceRequesterResponded { get; set; }

    [JsonPropertyName("hoursSinceAgentResponded")]
    public DateTime? HoursSinceAgentResponded { get; set; }

    [JsonPropertyName("isTicketBrandChanged")]
    public bool? IsTicketBrandChanged { get; set; }

    [JsonPropertyName("ticketFormId")]
    public IdName? TicketFormId { get; set; }

    [JsonPropertyName("emailReceivedAt")]
    public string? EmailReceivedAt { get; set; }

    [JsonPropertyName("requesterEmail")]
    public string? RequesterEmail { get; set; }

    [JsonPropertyName("approvalRequestStatus")]
    public string? ApprovalRequestStatus { get; set; }

    [JsonPropertyName("hasAnyPendingApprovalRequests")]
    public bool? HasAnyPendingApprovalRequests { get; set; }

    [JsonPropertyName("tag")]
    public List<Tag>? Tags { get; set; }

    [JsonPropertyName("satisfactionSurveyCategory")]
    public IdName? SatisfactionSurveyCategory { get; set; }

    [JsonPropertyName("satisfactionSurveyRatingPoint")]
    public int? SatisfactionSurveyRatingPoint { get; set; }

    [JsonPropertyName("closedOn")]
    public DateTime? ClosedOn { get; set; }

    [JsonPropertyName("lastStatusChangedOn")]
    public DateTime? LastStatusChangedOn { get; set; }

    [JsonPropertyName("responseDue")]
    public DateTime? ResponseDue { get; set; }

    [JsonPropertyName("isSlaTimerRunning")]
    public bool? IsSlaTimerRunning { get; set; }

    [JsonPropertyName("slaBreachedCount")]
    public int SlaBreachedCount { get; set; }

    [JsonPropertyName("slaAchievedCount")]
    public int? SlaAchievedCount { get; set; }

    [JsonPropertyName("isResolutionOverdue")]
    public bool? IsResolutionOverdue { get; set; }

    [JsonPropertyName("isResponseOverdue")]
    public bool? IsResponseOverdue { get; set; }

    [JsonPropertyName("lastRepliedBy")]
    public object? LastRepliedBy { get; set; }  // Can be string or object

    [JsonPropertyName("lastRepliedByAgent")]
    public object? LastRepliedByAgent { get; set; }  // Can be string or object

    [JsonPropertyName("brandOptionId")]
    public int? BrandOptionId { get; set; }

    // Custom fields that appear in the actual API response
    [JsonPropertyName("cf_operating_system")]
    public string? OperatingSystem { get; set; }

    [JsonPropertyName("cf_avaloniaxpf_version")]
    public string? AvaloniaXpfVersion { get; set; }

    [JsonPropertyName("cf_accelerate_support_tier")]
    public string? AccelerateSupportTier { get; set; }

    [JsonPropertyName("customFields")]
    public object? CustomFields { get; set; }

    // Maps simple { id, name } objects
    public class IdName
    {
        [JsonPropertyName("id")] public int? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
    }

    // Tag model for tags array
    public class Tag
    {
        [JsonPropertyName("id")] public int? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
    }
}

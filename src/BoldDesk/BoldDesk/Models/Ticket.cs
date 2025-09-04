using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class Ticket
{
    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

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
    public string? Brand { get; set; }

    [JsonPropertyName("brandId")]
    public int? BrandId { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("isVisibleToCustomer")]
    public bool IsVisibleToCustomer { get; set; }

    [JsonPropertyName("updatesCount")]
    public int? UpdatesCount { get; set; }

    // Note: API uses "attachmentCount" not "attachmentsCount"
    [JsonPropertyName("attachmentCount")]
    public int AttachmentsCount { get; set; }

    [JsonPropertyName("source")]
    public object? Source { get; set; }  // Can be string or object

    [JsonPropertyName("sourceId")]
    public int? SourceId { get; set; }

    [JsonPropertyName("lastRepliedOn")]
    public DateTime? LastRepliedOn { get; set; }

    [JsonPropertyName("requestedBy")]
    public RequestedBy? RequestedBy { get; set; }

    [JsonPropertyName("isSpamOrDeleted")]
    public bool? IsSpamOrDeleted { get; set; }

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
    public string? CfOperatingSystem { get; set; }

    [JsonPropertyName("cf_avaloniaxpf_version")]
    public string? CfAvaloniaXpfVersion { get; set; }

    [JsonPropertyName("customFields")]
    public object? CustomFields { get; set; }
}
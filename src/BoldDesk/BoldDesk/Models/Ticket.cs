using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class Ticket
{
    private int _ticketId;

    // Primary mapping used by list APIs
    [JsonPropertyName("ticketId")]
    public int TicketId
    {
        get => _ticketId;
        set => _ticketId = value;
    }

    // Some endpoints may return "id" instead of "ticketId" for single ticket
    [JsonPropertyName("id")]
    public int? Id
    {
        get => _ticketId;
        set
        {
            if (value.HasValue)
                _ticketId = value.Value;
        }
    }

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

    private Brand? _brand;
    [JsonPropertyName("brand")]
    public Brand? Brand
    {
        get => _brand;
        set
        {
            _brand = value;
            if (value != null)
                BrandId = value.BrandId;
        }
    }

    [JsonPropertyName("brandId")]
    public int? BrandId { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    // Map type.name to Mode if provided (some APIs use "type")
    [JsonPropertyName("type")]
    public IdName? Type
    {
        get => null;
        set
        {
            if (value?.Name != null)
                Mode = value.Name;
        }
    }

    [JsonPropertyName("isVisibleToCustomer")]
    public bool IsVisibleToCustomer { get; set; }

    [JsonPropertyName("updatesCount")]
    public int? UpdatesCount { get; set; }

    // Note: API uses "attachmentCount" not "attachmentsCount"
    [JsonPropertyName("attachmentCount")]
    public int AttachmentsCount { get; set; }

    [JsonPropertyName("sourceId")]
    public int? SourceId { get; set; }

    private IdName? _source;
    // Some responses provide { id, name } for source
    [JsonPropertyName("source")]
    public IdName? Source
    {
        get => _source;
        set
        {
            _source = value;
            if (value?.Id != null)
                SourceId = value.Id;
        }
    }

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

    // Maps simple { id, name } objects
    public class IdName
    {
        [JsonPropertyName("id")] public int? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
    }
}

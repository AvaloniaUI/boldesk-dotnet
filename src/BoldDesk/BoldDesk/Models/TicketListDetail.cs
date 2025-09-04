using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class TicketListDetail
{
    [JsonPropertyName("slaAchievedCount")]
    public int SlaAchievedCount { get; set; }

    [JsonPropertyName("ticketStatusCategoryId")]
    public int TicketStatusCategoryId { get; set; }

    [JsonPropertyName("statusSortOrder")]
    public int StatusSortOrder { get; set; }

    [JsonPropertyName("ticketLastRepliedByUserTypeId")]
    public int TicketLastRepliedByUserTypeId { get; set; }

    [JsonPropertyName("prioritySortOrder")]
    public int PrioritySortOrder { get; set; }

    [JsonPropertyName("satisfactionSurveyCategory")]
    public int SatisfactionSurveyCategory { get; set; }

    [JsonPropertyName("isBrandActive")]
    public bool IsBrandActive { get; set; }

    [JsonPropertyName("isCustomerPortalActive")]
    public bool IsCustomerPortalActive { get; set; }
}
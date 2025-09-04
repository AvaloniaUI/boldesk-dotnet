using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class Worklog
{
    [JsonPropertyName("worklogId")]
    public int WorklogId { get; set; }

    [JsonPropertyName("ticketId")]
    public int TicketId { get; set; }

    [JsonPropertyName("timeSpent")]
    public int TimeSpent { get; set; } // Time in minutes

    [JsonPropertyName("isBillable")]
    public bool IsBillable { get; set; }

    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("worklogDate")]
    public DateTime WorklogDate { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("createdBy")]
    public WorklogUser? CreatedBy { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("lastModifiedOn")]
    public DateTime LastModifiedOn { get; set; }
}
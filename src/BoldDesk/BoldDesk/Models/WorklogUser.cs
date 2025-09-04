using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// User model for worklogs (similar to RequestedBy but slightly different structure)
/// </summary>
public class WorklogUser
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("emailId")]
    public string EmailId { get; set; } = string.Empty;

    [JsonPropertyName("shortCode")]
    public string ShortCode { get; set; } = string.Empty;

    [JsonPropertyName("colorCode")]
    public string ColorCode { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("isVerified")]
    public bool IsVerified { get; set; }

    [JsonPropertyName("profileImageUrl")]
    public string? ProfileImageUrl { get; set; }

    [JsonPropertyName("isAgent")]
    public bool IsAgent { get; set; }

    [JsonPropertyName("agentShiftId")]
    public int AgentShiftId { get; set; }

    [JsonPropertyName("agentShiftName")]
    public string AgentShiftName { get; set; } = string.Empty;

    [JsonPropertyName("ticketLimit")]
    public int TicketLimit { get; set; }

    [JsonPropertyName("chatLimit")]
    public int ChatLimit { get; set; }
}
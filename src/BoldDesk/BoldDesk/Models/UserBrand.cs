using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class UserBrand
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
    
    [JsonPropertyName("logoLink")]
    public string LogoLink { get; set; } = string.Empty;
    
    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }
    
    [JsonPropertyName("fieldOptionId")]
    public int FieldOptionId { get; set; }
    
    [JsonPropertyName("isDeactivated")]
    public bool IsDeactivated { get; set; }
    
    [JsonPropertyName("isCustomerPortalActive")]
    public bool IsCustomerPortalActive { get; set; }
    
    [JsonPropertyName("defaultTicketFormId")]
    public int? DefaultTicketFormId { get; set; }
    
    [JsonPropertyName("isKbEnabled")]
    public bool IsKbEnabled { get; set; }
}
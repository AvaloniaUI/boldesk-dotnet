using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class Status
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("textColor")]
    public string TextColor { get; set; } = string.Empty;

    [JsonPropertyName("backgroundColor")]
    public string BackgroundColor { get; set; } = string.Empty;
}
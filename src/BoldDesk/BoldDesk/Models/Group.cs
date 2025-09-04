using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class Group
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class BoldDeskErrorResponse
{
    [JsonPropertyName("errors")]
    public List<BoldDeskError> Errors { get; set; } = new();
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }
}
using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class BoldDeskError
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;
    
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;
    
    [JsonPropertyName("errorType")]
    public string ErrorType { get; set; } = string.Empty;
}
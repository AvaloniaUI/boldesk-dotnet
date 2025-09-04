using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class BoldDeskResponse<T>
{
    [JsonPropertyName("result")]
    public List<T> Result { get; set; } = new();

    [JsonPropertyName("count")]
    public int Count { get; set; }
}

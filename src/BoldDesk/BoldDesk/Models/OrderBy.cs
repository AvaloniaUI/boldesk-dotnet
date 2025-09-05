using System.Text.Json.Serialization;

namespace BoldDesk.Models;

/// <summary>
/// Specifies the sort order for API queries
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderBy
{
    /// <summary>
    /// Sort in ascending order
    /// </summary>
    [JsonPropertyName("asc")]
    Ascending,

    /// <summary>
    /// Sort in descending order  
    /// </summary>
    [JsonPropertyName("desc")]
    Descending
}
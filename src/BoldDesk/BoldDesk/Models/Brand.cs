using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class Brand
{
    [JsonPropertyName("brandId")]
    public int BrandId { get; set; }
    
    [JsonPropertyName("brandName")]
    public string BrandName { get; set; } = string.Empty;
    
    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }
    
    [JsonPropertyName("isDisabled")]
    public bool IsDisabled { get; set; }
}
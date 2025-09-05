using System.Text.Json.Serialization;

namespace BoldDesk.Models;

public class FieldOption
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("isReadOnly")]
    public bool IsReadOnly { get; set; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }

    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("parentOptionId")]
    public List<int>? ParentOptionId { get; set; }

    [JsonPropertyName("isPrivate")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("canDelete")]
    public bool CanDelete { get; set; }

    [JsonPropertyName("isSystemDefault")]
    public bool IsSystemDefault { get; set; }
}

public class FieldOptionsResponse
{
    [JsonPropertyName("result")]
    public List<FieldOption> Result { get; set; } = new();
}

public class FieldOptionQueryParameters
{
    public string? Filter { get; set; }
    public int? ParentOptionId { get; set; }
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 10;
    public bool RequiresCounts { get; set; }
    public string? OrderBy { get; set; }
    public string? ExclusionIds { get; set; }
    public bool IncludeReadOnlyAlso { get; set; }
}

public class AddFieldOptionsRequest
{
    [JsonPropertyName("fieldOptions")]
    public List<string> FieldOptions { get; set; } = new();
}

public class FieldPositionChangeParameters
{
    public int ToPosition { get; set; }
    public bool IsSortByAlphabeticalOrder { get; set; }
    public bool IsMoveToTopPosition { get; set; }
    public bool IsMoveToBottomPosition { get; set; }
}

public class FieldApiResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
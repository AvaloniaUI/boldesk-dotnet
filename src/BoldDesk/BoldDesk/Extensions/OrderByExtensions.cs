using BoldDesk.Models;

namespace BoldDesk.Extensions;

/// <summary>
/// Extension methods for SortOrder enum
/// </summary>
public static class OrderByExtensions
{
    /// <summary>
    /// Converts SortOrder enum to API string value
    /// </summary>
    public static string ToApiString(this OrderBy orderBy)
    {
        return orderBy switch
        {
            OrderBy.Ascending => "asc",
            OrderBy.Descending => "desc",
            _ => "desc" // Default to descending
        };
    }

    /// <summary>
    /// Converts string value to SortOrder enum
    /// </summary>
    public static OrderBy FromApiString(string value)
    {
        return value?.ToLowerInvariant() switch
        {
            "asc" => OrderBy.Ascending,
            "desc" => OrderBy.Descending,
            _ => OrderBy.Descending // Default to descending
        };
    }
}
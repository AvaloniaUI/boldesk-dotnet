using System.Text;

namespace BoldDesk.QueryBuilder;

/// <summary>
/// Base class for building Q parameter queries for BoldDesk API
/// </summary>
public abstract class QueryBuilder
{
    protected readonly List<string> Conditions = new();

    /// <summary>
    /// Builds the final Q parameter string
    /// </summary>
    public string Build()
    {
        if (Conditions.Count == 0)
            return string.Empty;

        if (Conditions.Count == 1)
            return Conditions[0];

        // Join multiple conditions with AND
        return string.Join(" AND ", Conditions);
    }

    /// <summary>
    /// Builds the Q parameter as an array of strings for APIs that expect string arrays
    /// </summary>
    public string[] BuildArray()
    {
        if (Conditions.Count == 0)
            return Array.Empty<string>();
        
        return Conditions.ToArray();
    }
    
    /// <summary>
    /// Converts the query builder to a Q parameter string
    /// </summary>
    public override string ToString() => Build();

    /// <summary>
    /// Adds a raw condition to the query
    /// </summary>
    protected void AddCondition(string condition)
    {
        if (!string.IsNullOrWhiteSpace(condition))
        {
            Conditions.Add(condition);
        }
    }

    /// <summary>
    /// Formats an array of IDs for the query
    /// </summary>
    protected static string FormatIdArray(params int[] ids)
    {
        return $"[{string.Join(",", ids)}]";
    }

    /// <summary>
    /// Formats an array of IDs for the query
    /// </summary>
    protected static string FormatIdArray(IEnumerable<int> ids)
    {
        return $"[{string.Join(",", ids)}]";
    }

    /// <summary>
    /// Formats a date range for the query
    /// </summary>
    protected static string FormatDateRange(DateTime from, DateTime to)
    {
        return $"{{\"from\":\"{from:yyyy-MM-ddTHH:mm:ss.fffZ}\",\"to\":\"{to:yyyy-MM-ddTHH:mm:ss.fffZ}\"}}";
    }

    /// <summary>
    /// Gets a predefined time period string
    /// </summary>
    protected static class TimePeriod
    {
        public const string Within4Hours = "within4hours";
        public const string Within12Hours = "within12hours";
        public const string Within24Hours = "within24hours";
        public const string Today = "today";
        public const string Yesterday = "yesterday";
        public const string ThisWeek = "thisweek";
        public const string Last7Days = "last7days";
        public const string ThisMonth = "thismonth";
        public const string Last30Days = "last30days";
    }
}
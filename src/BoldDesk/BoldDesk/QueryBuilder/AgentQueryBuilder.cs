using BoldDesk.Models;

namespace BoldDesk.QueryBuilder;

/// <summary>
/// Fluent query builder for agent Q parameters
/// </summary>
public class AgentQueryBuilder : QueryBuilder
{
    /// <summary>
    /// Filters by agent creation date
    /// </summary>
    public AgentQueryBuilder WithCreatedOn(string period)
    {
        AddCondition($"createdon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by agent creation date with custom range
    /// </summary>
    public AgentQueryBuilder WithCreatedOn(DateTime from, DateTime to)
    {
        AddCondition($"createdon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by agents created today
    /// </summary>
    public AgentQueryBuilder CreatedToday()
    {
        return WithCreatedOn(TimePeriod.Today);
    }

    /// <summary>
    /// Filters by agents created this week
    /// </summary>
    public AgentQueryBuilder CreatedThisWeek()
    {
        return WithCreatedOn(TimePeriod.ThisWeek);
    }

    /// <summary>
    /// Filters by agents created in last 7 days
    /// </summary>
    public AgentQueryBuilder CreatedLast7Days()
    {
        return WithCreatedOn(TimePeriod.Last7Days);
    }

    /// <summary>
    /// Filters by agents created this month
    /// </summary>
    public AgentQueryBuilder CreatedThisMonth()
    {
        return WithCreatedOn(TimePeriod.ThisMonth);
    }

    /// <summary>
    /// Filters by last modified date
    /// </summary>
    public AgentQueryBuilder WithLastModifiedOn(string period)
    {
        AddCondition($"lastmodifiedon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by last modified date with custom range
    /// </summary>
    public AgentQueryBuilder WithLastModifiedOn(DateTime from, DateTime to)
    {
        AddCondition($"lastmodifiedon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by agent IDs
    /// </summary>
    public AgentQueryBuilder WithIds(params int[] ids)
    {
        if (ids?.Length > 0)
        {
            AddCondition($"ids:{FormatIdArray(ids)}");
        }
        return this;
    }

    /// <summary>
    /// Applies the query to AgentQueryParameters
    /// </summary>
    public AgentQueryParameters ApplyTo(AgentQueryParameters? parameters = null)
    {
        parameters ??= new AgentQueryParameters();
        parameters.Q = Build();
        return parameters;
    }

    /// <summary>
    /// Creates a new AgentQueryParameters with the query applied
    /// </summary>
    public AgentQueryParameters ToParameters()
    {
        return ApplyTo(null);
    }
}
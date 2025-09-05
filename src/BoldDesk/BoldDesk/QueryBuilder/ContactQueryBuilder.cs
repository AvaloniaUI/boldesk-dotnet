using BoldDesk.Models;

namespace BoldDesk.QueryBuilder;

/// <summary>
/// Fluent query builder for contact Q parameters
/// </summary>
public class ContactQueryBuilder : QueryBuilder
{
    /// <summary>
    /// Filters by contact creation date
    /// </summary>
    public ContactQueryBuilder WithCreatedOn(string period)
    {
        AddCondition($"createdon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by contact creation date with custom range
    /// </summary>
    public ContactQueryBuilder WithCreatedOn(DateTime from, DateTime to)
    {
        AddCondition($"createdon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by contacts created today
    /// </summary>
    public ContactQueryBuilder CreatedToday()
    {
        return WithCreatedOn(TimePeriod.Today);
    }

    /// <summary>
    /// Filters by contacts created this week
    /// </summary>
    public ContactQueryBuilder CreatedThisWeek()
    {
        return WithCreatedOn(TimePeriod.ThisWeek);
    }

    /// <summary>
    /// Filters by contacts created in last 7 days
    /// </summary>
    public ContactQueryBuilder CreatedLast7Days()
    {
        return WithCreatedOn(TimePeriod.Last7Days);
    }

    /// <summary>
    /// Filters by contacts created this month
    /// </summary>
    public ContactQueryBuilder CreatedThisMonth()
    {
        return WithCreatedOn(TimePeriod.ThisMonth);
    }

    /// <summary>
    /// Filters by contacts created in last 30 days
    /// </summary>
    public ContactQueryBuilder CreatedLast30Days()
    {
        return WithCreatedOn(TimePeriod.Last30Days);
    }

    /// <summary>
    /// Filters by contact last modified date
    /// </summary>
    public ContactQueryBuilder WithLastModifiedOn(string period)
    {
        AddCondition($"lastmodifiedon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by contact last modified date with custom range
    /// </summary>
    public ContactQueryBuilder WithLastModifiedOn(DateTime from, DateTime to)
    {
        AddCondition($"lastmodifiedon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by contacts modified today
    /// </summary>
    public ContactQueryBuilder ModifiedToday()
    {
        return WithLastModifiedOn(TimePeriod.Today);
    }

    /// <summary>
    /// Filters by contacts modified this week
    /// </summary>
    public ContactQueryBuilder ModifiedThisWeek()
    {
        return WithLastModifiedOn(TimePeriod.ThisWeek);
    }

    /// <summary>
    /// Filters by contacts modified in last 7 days
    /// </summary>
    public ContactQueryBuilder ModifiedLast7Days()
    {
        return WithLastModifiedOn(TimePeriod.Last7Days);
    }

    /// <summary>
    /// Filters by contacts modified this month
    /// </summary>
    public ContactQueryBuilder ModifiedThisMonth()
    {
        return WithLastModifiedOn(TimePeriod.ThisMonth);
    }

    /// <summary>
    /// Filters by contacts modified in last 30 days
    /// </summary>
    public ContactQueryBuilder ModifiedLast30Days()
    {
        return WithLastModifiedOn(TimePeriod.Last30Days);
    }

    /// <summary>
    /// Filters by specific contact IDs
    /// </summary>
    public ContactQueryBuilder WithIds(params long[] ids)
    {
        if (ids.Length > 0)
        {
            AddCondition($"ids:[{string.Join(",", ids)}]");
        }
        return this;
    }

    /// <summary>
    /// Filters by specific contact IDs
    /// </summary>
    public ContactQueryBuilder WithIds(IEnumerable<long> ids)
    {
        return WithIds(ids.ToArray());
    }

    /// <summary>
    /// Filters by a single contact ID
    /// </summary>
    public ContactQueryBuilder WithId(long id)
    {
        return WithIds(id);
    }

    /// <summary>
    /// Adds a custom condition to the query
    /// </summary>
    public ContactQueryBuilder WithCustomCondition(string condition)
    {
        AddCondition(condition);
        return this;
    }

    /// <summary>
    /// Combines this builder with another using AND logic
    /// </summary>
    public ContactQueryBuilder And(ContactQueryBuilder other)
    {
        foreach (var condition in other.Conditions)
        {
            AddCondition(condition);
        }
        return this;
    }

    /// <summary>
    /// Creates a ContactQueryParameters object with the built query
    /// </summary>
    public ContactQueryParameters ToParameters()
    {
        return new ContactQueryParameters
        {
            Q = BuildArray()
        };
    }

    /// <summary>
    /// Creates a ContactQueryParameters object with the built query and additional parameters
    /// </summary>
    public ContactQueryParameters ToParameters(int page = 1, int perPage = 50, string? filter = null, 
        string? view = null, long? contactGroupId = null, string? orderBy = null, bool requiresCounts = true)
    {
        return new ContactQueryParameters
        {
            Q = BuildArray(),
            Page = page,
            PerPage = perPage,
            Filter = filter,
            View = view,
            ContactGroupId = contactGroupId,
            OrderBy = orderBy,
            RequiresCounts = requiresCounts
        };
    }
}
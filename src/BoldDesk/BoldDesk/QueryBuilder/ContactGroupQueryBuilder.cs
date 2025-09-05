namespace BoldDesk.QueryBuilder;

/// <summary>
/// Fluent query builder for ContactGroup Q parameter
/// </summary>
public class ContactGroupQueryBuilder : QueryBuilder
{
    /// <summary>
    /// Filter by created date range
    /// </summary>
    public ContactGroupQueryBuilder WithCreatedOn(DateTime from, DateTime to)
    {
        AddCondition($"createdon:{FormatDateRange(from, to)}");
        return this;
    }
    
    /// <summary>
    /// Filter by created date (greater than)
    /// </summary>
    public ContactGroupQueryBuilder WithCreatedOnAfter(DateTime date)
    {
        AddCondition($"createdon:>{date:yyyy-MM-ddTHH:mm:ss.fffZ}");
        return this;
    }
    
    /// <summary>
    /// Filter by created date (less than)
    /// </summary>
    public ContactGroupQueryBuilder WithCreatedOnBefore(DateTime date)
    {
        AddCondition($"createdon:<{date:yyyy-MM-ddTHH:mm:ss.fffZ}");
        return this;
    }
    
    /// <summary>
    /// Filter by last modified date range
    /// </summary>
    public ContactGroupQueryBuilder WithLastModifiedOn(DateTime from, DateTime to)
    {
        AddCondition($"lastmodifiedon:{FormatDateRange(from, to)}");
        return this;
    }
    
    /// <summary>
    /// Filter by last modified date (greater than)
    /// </summary>
    public ContactGroupQueryBuilder WithLastModifiedOnAfter(DateTime date)
    {
        AddCondition($"lastmodifiedon:>{date:yyyy-MM-ddTHH:mm:ss.fffZ}");
        return this;
    }
    
    /// <summary>
    /// Filter by last modified date (less than)
    /// </summary>
    public ContactGroupQueryBuilder WithLastModifiedOnBefore(DateTime date)
    {
        AddCondition($"lastmodifiedon:<{date:yyyy-MM-ddTHH:mm:ss.fffZ}");
        return this;
    }
    
    /// <summary>
    /// Filter by contact group IDs
    /// </summary>
    public ContactGroupQueryBuilder WithIds(params long[] ids)
    {
        if (ids.Length > 0)
        {
            AddCondition($"ids:[{string.Join(",", ids)}]");
        }
        return this;
    }
    
    /// <summary>
    /// Filter by a single contact group ID
    /// </summary>
    public ContactGroupQueryBuilder WithId(long id)
    {
        return WithIds(id);
    }
}
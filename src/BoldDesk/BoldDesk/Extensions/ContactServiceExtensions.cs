using BoldDesk.QueryBuilder;
using BoldDesk.Services;

namespace BoldDesk.Extensions;

/// <summary>
/// Extension methods for ContactService to enable fluent query building
/// </summary>
public static class ContactServiceExtensions
{
    /// <summary>
    /// Creates a new ContactQueryBuilder for building contact queries
    /// </summary>
    public static ContactQueryBuilder NewQuery(this IContactService service)
    {
        return new ContactQueryBuilder();
    }

    /// <summary>
    /// Starts a query for contacts created today
    /// </summary>
    public static ContactQueryBuilder CreatedToday(this IContactService service)
    {
        return new ContactQueryBuilder().CreatedToday();
    }

    /// <summary>
    /// Starts a query for contacts created this week
    /// </summary>
    public static ContactQueryBuilder CreatedThisWeek(this IContactService service)
    {
        return new ContactQueryBuilder().CreatedThisWeek();
    }

    /// <summary>
    /// Starts a query for contacts created in the last 7 days
    /// </summary>
    public static ContactQueryBuilder CreatedLast7Days(this IContactService service)
    {
        return new ContactQueryBuilder().CreatedLast7Days();
    }

    /// <summary>
    /// Starts a query for contacts created this month
    /// </summary>
    public static ContactQueryBuilder CreatedThisMonth(this IContactService service)
    {
        return new ContactQueryBuilder().CreatedThisMonth();
    }

    /// <summary>
    /// Starts a query for contacts created in the last 30 days
    /// </summary>
    public static ContactQueryBuilder CreatedLast30Days(this IContactService service)
    {
        return new ContactQueryBuilder().CreatedLast30Days();
    }

    /// <summary>
    /// Starts a query for contacts modified today
    /// </summary>
    public static ContactQueryBuilder ModifiedToday(this IContactService service)
    {
        return new ContactQueryBuilder().ModifiedToday();
    }

    /// <summary>
    /// Starts a query for contacts modified this week
    /// </summary>
    public static ContactQueryBuilder ModifiedThisWeek(this IContactService service)
    {
        return new ContactQueryBuilder().ModifiedThisWeek();
    }

    /// <summary>
    /// Starts a query for contacts modified in the last 7 days
    /// </summary>
    public static ContactQueryBuilder ModifiedLast7Days(this IContactService service)
    {
        return new ContactQueryBuilder().ModifiedLast7Days();
    }

    /// <summary>
    /// Starts a query for contacts modified this month
    /// </summary>
    public static ContactQueryBuilder ModifiedThisMonth(this IContactService service)
    {
        return new ContactQueryBuilder().ModifiedThisMonth();
    }

    /// <summary>
    /// Starts a query for contacts modified in the last 30 days
    /// </summary>
    public static ContactQueryBuilder ModifiedLast30Days(this IContactService service)
    {
        return new ContactQueryBuilder().ModifiedLast30Days();
    }

    /// <summary>
    /// Starts a query for specific contact IDs
    /// </summary>
    public static ContactQueryBuilder WithIds(this IContactService service, params long[] ids)
    {
        return new ContactQueryBuilder().WithIds(ids);
    }

    /// <summary>
    /// Starts a query for a specific contact ID
    /// </summary>
    public static ContactQueryBuilder WithId(this IContactService service, long id)
    {
        return new ContactQueryBuilder().WithId(id);
    }
}
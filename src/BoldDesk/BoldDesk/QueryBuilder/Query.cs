namespace BoldDesk.QueryBuilder;

/// <summary>
/// Entry point for building BoldDesk Q parameter queries
/// </summary>
public static class Query
{
    /// <summary>
    /// Creates a new ticket query builder
    /// </summary>
    public static TicketQueryBuilder ForTickets() => new TicketQueryBuilder();

    /// <summary>
    /// Creates a new agent query builder
    /// </summary>
    public static AgentQueryBuilder ForAgents() => new AgentQueryBuilder();

    /// <summary>
    /// Creates a new activity/worklog query builder
    /// </summary>
    public static ActivityQueryBuilder ForActivities() => new ActivityQueryBuilder();

    /// <summary>
    /// Creates a new contact group query builder
    /// </summary>
    public static ContactGroupQueryBuilder ForContactGroups() => new ContactGroupQueryBuilder();

    /// <summary>
    /// Helper class for common time periods
    /// </summary>
    public static class TimePeriods
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
        public const string Overdue = "overdue";
        public const string Tomorrow = "tomorrow";
        public const string Next30Minutes = "next30minutes";
        public const string NextHour = "nexthour";
        public const string Next2Hours = "next2hours";
        public const string Next4Hours = "next4hours";
        public const string Next8Hours = "next8hours";
        public const string Next24Hours = "next24hours";
    }
}
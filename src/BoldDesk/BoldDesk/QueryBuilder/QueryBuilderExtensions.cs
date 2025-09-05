using BoldDesk.Models;

namespace BoldDesk.QueryBuilder;

/// <summary>
/// Extension methods for fluent query building
/// </summary>
public static class QueryBuilderExtensions
{
    /// <summary>
    /// Build a ticket query using a fluent builder
    /// </summary>
    public static TicketQueryParameters WithQuery(
        this TicketQueryParameters parameters, 
        Action<TicketQueryBuilder> builder)
    {
        var queryBuilder = new TicketQueryBuilder();
        builder(queryBuilder);
        parameters.Q = queryBuilder.Build();
        return parameters;
    }
    
    /// <summary>
    /// Build an agent query using a fluent builder
    /// </summary>
    public static AgentQueryParameters WithQuery(
        this AgentQueryParameters parameters,
        Action<AgentQueryBuilder> builder)
    {
        var queryBuilder = new AgentQueryBuilder();
        builder(queryBuilder);
        parameters.Q = queryBuilder.Build();
        return parameters;
    }
    
    /// <summary>
    /// Build a worklog query using a fluent builder
    /// </summary>
    public static WorklogQueryParameters WithQuery(
        this WorklogQueryParameters parameters,
        Action<ActivityQueryBuilder> builder)
    {
        var queryBuilder = new ActivityQueryBuilder();
        builder(queryBuilder);
        // WorklogQueryParameters doesn't have a Q property in the current implementation
        // This will need to be added if query building is needed for worklogs
        return parameters;
    }
    
    /// <summary>
    /// Build a contact group query using a fluent builder
    /// </summary>
    public static ContactGroupQueryParameters WithQuery(
        this ContactGroupQueryParameters parameters,
        Action<ContactGroupQueryBuilder> builder)
    {
        var queryBuilder = new ContactGroupQueryBuilder();
        builder(queryBuilder);
        parameters.Q = queryBuilder.BuildArray();
        return parameters;
    }
}
using BoldDesk.Models;

namespace BoldDesk.QueryBuilder;

/// <summary>
/// Fluent query builder for activity/worklog Q parameters
/// </summary>
public class ActivityQueryBuilder : QueryBuilder
{
    /// <summary>
    /// Filters by updated date
    /// </summary>
    public ActivityQueryBuilder WithUpdatedOn(string period)
    {
        AddCondition($"updatedon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by updated date with custom range
    /// </summary>
    public ActivityQueryBuilder WithUpdatedOn(DateTime from, DateTime to)
    {
        AddCondition($"updatedon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by activity agent IDs
    /// </summary>
    public ActivityQueryBuilder WithActivityAgent(params int[] agentIds)
    {
        if (agentIds?.Length > 0)
        {
            AddCondition($"activityagent:{FormatIdArray(agentIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by activity status IDs
    /// </summary>
    public ActivityQueryBuilder WithActivityStatus(params int[] statusIds)
    {
        if (statusIds?.Length > 0)
        {
            AddCondition($"activitystatus:{FormatIdArray(statusIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by activity status category IDs
    /// </summary>
    public ActivityQueryBuilder WithActivityStatusCategory(params int[] categoryIds)
    {
        if (categoryIds?.Length > 0)
        {
            AddCondition($"activitystatuscategory:{FormatIdArray(categoryIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by activity priority IDs
    /// </summary>
    public ActivityQueryBuilder WithActivityPriority(params int[] priorityIds)
    {
        if (priorityIds?.Length > 0)
        {
            AddCondition($"activitypriority:{FormatIdArray(priorityIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by activity due date
    /// </summary>
    public ActivityQueryBuilder WithActivityDueDate(string period)
    {
        AddCondition($"activityduedate:{period}");
        return this;
    }

    /// <summary>
    /// Filters by activity due date with custom range
    /// </summary>
    public ActivityQueryBuilder WithActivityDueDate(DateTime from, DateTime to)
    {
        AddCondition($"activityduedate:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by overdue activities
    /// </summary>
    public ActivityQueryBuilder WithOverdueActivities()
    {
        AddCondition("activityduedate:overdue");
        return this;
    }

    /// <summary>
    /// Filters by activities due today
    /// </summary>
    public ActivityQueryBuilder WithActivitiesDueToday()
    {
        AddCondition("activityduedate:today");
        return this;
    }

    /// <summary>
    /// Filters by activities due tomorrow
    /// </summary>
    public ActivityQueryBuilder WithActivitiesDueTomorrow()
    {
        AddCondition("activityduedate:tomorrow");
        return this;
    }

    /// <summary>
    /// Filters by activity start time
    /// </summary>
    public ActivityQueryBuilder WithActivityStartTime(string period)
    {
        AddCondition($"activitystarttime:{period}");
        return this;
    }

    /// <summary>
    /// Filters by activity start time with custom range
    /// </summary>
    public ActivityQueryBuilder WithActivityStartTime(DateTime from, DateTime to)
    {
        AddCondition($"activitystarttime:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by activity end time
    /// </summary>
    public ActivityQueryBuilder WithActivityEndTime(string period)
    {
        AddCondition($"activityendtime:{period}");
        return this;
    }

    /// <summary>
    /// Filters by activity end time with custom range
    /// </summary>
    public ActivityQueryBuilder WithActivityEndTime(DateTime from, DateTime to)
    {
        AddCondition($"activityendtime:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by linked ticket IDs
    /// </summary>
    public ActivityQueryBuilder WithLinkedTicket(params int[] ticketIds)
    {
        if (ticketIds?.Length > 0)
        {
            AddCondition($"activitylinkedticket:{FormatIdArray(ticketIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by linked user IDs
    /// </summary>
    public ActivityQueryBuilder WithLinkedUser(params int[] userIds)
    {
        if (userIds?.Length > 0)
        {
            AddCondition($"activitylinkeduser:{FormatIdArray(userIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by activity created by user IDs
    /// </summary>
    public ActivityQueryBuilder WithCreatedBy(params int[] userIds)
    {
        if (userIds?.Length > 0)
        {
            AddCondition($"activitycreatedby:{FormatIdArray(userIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by activity creation date
    /// </summary>
    public ActivityQueryBuilder WithCreatedOn(string period)
    {
        AddCondition($"activitycreatedon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by activity creation date with custom range
    /// </summary>
    public ActivityQueryBuilder WithCreatedOn(DateTime from, DateTime to)
    {
        AddCondition($"activitycreatedon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by activity last modified date
    /// </summary>
    public ActivityQueryBuilder WithLastModifiedOn(string period)
    {
        AddCondition($"activitylastmodifiedon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by activity last modified date with custom range
    /// </summary>
    public ActivityQueryBuilder WithLastModifiedOn(DateTime from, DateTime to)
    {
        AddCondition($"activitylastmodifiedon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by approver IDs
    /// </summary>
    public ActivityQueryBuilder WithApprovers(params int[] approverIds)
    {
        if (approverIds?.Length > 0)
        {
            AddCondition($"approvers:{FormatIdArray(approverIds)}");
        }
        return this;
    }

    /// <summary>
    /// Applies the query to WorklogQueryParameters
    /// </summary>
    public WorklogQueryParameters ApplyTo(WorklogQueryParameters? parameters = null)
    {
        parameters ??= new WorklogQueryParameters();
        // Note: WorklogQueryParameters doesn't have a Q property yet
        // We'll need to add it or use a different approach
        return parameters;
    }
}
using BoldDesk.Models;

namespace BoldDesk.QueryBuilder;

/// <summary>
/// Fluent query builder for ticket Q parameters
/// </summary>
public class TicketQueryBuilder : QueryBuilder
{
    /// <summary>
    /// Filters by ticket creation date
    /// </summary>
    public TicketQueryBuilder WithCreatedOn(string period)
    {
        AddCondition($"createdon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by ticket creation date with custom range
    /// </summary>
    public TicketQueryBuilder WithCreatedOn(DateTime from, DateTime to)
    {
        AddCondition($"createdon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by tickets created today
    /// </summary>
    public TicketQueryBuilder CreatedToday()
    {
        return WithCreatedOn(TimePeriod.Today);
    }

    /// <summary>
    /// Filters by tickets created this week
    /// </summary>
    public TicketQueryBuilder CreatedThisWeek()
    {
        return WithCreatedOn(TimePeriod.ThisWeek);
    }

    /// <summary>
    /// Filters by tickets created in last 7 days
    /// </summary>
    public TicketQueryBuilder CreatedLast7Days()
    {
        return WithCreatedOn(TimePeriod.Last7Days);
    }

    /// <summary>
    /// Filters by tickets created this month
    /// </summary>
    public TicketQueryBuilder CreatedThisMonth()
    {
        return WithCreatedOn(TimePeriod.ThisMonth);
    }

    /// <summary>
    /// Filters by last modified date
    /// </summary>
    public TicketQueryBuilder WithLastModifiedOn(string period)
    {
        AddCondition($"lastmodifiedon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by last modified date with custom range
    /// </summary>
    public TicketQueryBuilder WithLastModifiedOn(DateTime from, DateTime to)
    {
        AddCondition($"lastmodifiedon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by ticket IDs
    /// </summary>
    public TicketQueryBuilder WithIds(params int[] ids)
    {
        if (ids?.Length > 0)
        {
            AddCondition($"ids:{FormatIdArray(ids)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by brand IDs
    /// </summary>
    public TicketQueryBuilder WithBrands(params int[] brandIds)
    {
        if (brandIds?.Length > 0)
        {
            AddCondition($"brands:{FormatIdArray(brandIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by agent IDs
    /// </summary>
    public TicketQueryBuilder WithAgents(params int[] agentIds)
    {
        if (agentIds?.Length > 0)
        {
            AddCondition($"agents:{FormatIdArray(agentIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by group IDs
    /// </summary>
    public TicketQueryBuilder WithGroups(params int[] groupIds)
    {
        if (groupIds?.Length > 0)
        {
            AddCondition($"groups:{FormatIdArray(groupIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by status IDs
    /// </summary>
    public TicketQueryBuilder WithStatus(params int[] statusIds)
    {
        if (statusIds?.Length > 0)
        {
            AddCondition($"status:{FormatIdArray(statusIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by priority IDs
    /// </summary>
    public TicketQueryBuilder WithPriority(params int[] priorityIds)
    {
        if (priorityIds?.Length > 0)
        {
            AddCondition($"priority:{FormatIdArray(priorityIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by category IDs
    /// </summary>
    public TicketQueryBuilder WithCategories(params int[] categoryIds)
    {
        if (categoryIds?.Length > 0)
        {
            AddCondition($"categories:{FormatIdArray(categoryIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by requester IDs
    /// </summary>
    public TicketQueryBuilder WithRequester(params int[] requesterIds)
    {
        if (requesterIds?.Length > 0)
        {
            AddCondition($"requester:{FormatIdArray(requesterIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by created by user IDs
    /// </summary>
    public TicketQueryBuilder WithCreatedBy(params int[] userIds)
    {
        if (userIds?.Length > 0)
        {
            AddCondition($"createdby:{FormatIdArray(userIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by closed on date
    /// </summary>
    public TicketQueryBuilder WithClosedOn(string period)
    {
        AddCondition($"closedon:{period}");
        return this;
    }

    /// <summary>
    /// Filters by closed on date with custom range
    /// </summary>
    public TicketQueryBuilder WithClosedOn(DateTime from, DateTime to)
    {
        AddCondition($"closedon:{FormatDateRange(from, to)}");
        return this;
    }

    /// <summary>
    /// Filters by response due
    /// </summary>
    public TicketQueryBuilder WithResponseDue(string period)
    {
        AddCondition($"responsedue:{period}");
        return this;
    }

    /// <summary>
    /// Filters by overdue response
    /// </summary>
    public TicketQueryBuilder WithResponseOverdue()
    {
        AddCondition("responsedue:overdue");
        return this;
    }

    /// <summary>
    /// Filters by response due today
    /// </summary>
    public TicketQueryBuilder WithResponseDueToday()
    {
        AddCondition("responsedue:today");
        return this;
    }

    /// <summary>
    /// Filters by response due tomorrow
    /// </summary>
    public TicketQueryBuilder WithResponseDueTomorrow()
    {
        AddCondition("responsedue:tomorrow");
        return this;
    }

    /// <summary>
    /// Filters by resolution due
    /// </summary>
    public TicketQueryBuilder WithResolutionDue(string period)
    {
        AddCondition($"resolutiondue:{period}");
        return this;
    }

    /// <summary>
    /// Filters by overdue resolution
    /// </summary>
    public TicketQueryBuilder WithResolutionOverdue()
    {
        AddCondition("resolutiondue:overdue");
        return this;
    }

    /// <summary>
    /// Filters by resolution due today
    /// </summary>
    public TicketQueryBuilder WithResolutionDueToday()
    {
        AddCondition("resolutiondue:today");
        return this;
    }

    /// <summary>
    /// Filters by source IDs
    /// </summary>
    public TicketQueryBuilder WithSource(params int[] sourceIds)
    {
        if (sourceIds?.Length > 0)
        {
            AddCondition($"source:{FormatIdArray(sourceIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by tag IDs
    /// </summary>
    public TicketQueryBuilder WithTags(params int[] tagIds)
    {
        if (tagIds?.Length > 0)
        {
            AddCondition($"tags:{FormatIdArray(tagIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by type IDs
    /// </summary>
    public TicketQueryBuilder WithType(params int[] typeIds)
    {
        if (typeIds?.Length > 0)
        {
            AddCondition($"type:{FormatIdArray(typeIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by contact group IDs
    /// </summary>
    public TicketQueryBuilder WithContactGroups(params int[] contactGroupIds)
    {
        if (contactGroupIds?.Length > 0)
        {
            AddCondition($"contactgroups:{FormatIdArray(contactGroupIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by specific ticket IDs (different from ids)
    /// </summary>
    public TicketQueryBuilder WithTicketIds(params int[] ticketIds)
    {
        if (ticketIds?.Length > 0)
        {
            AddCondition($"ticketids:{FormatIdArray(ticketIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by rating points
    /// </summary>
    public TicketQueryBuilder WithRatingPoints(params int[] points)
    {
        if (points?.Length > 0)
        {
            AddCondition($"ratingpoints:{FormatIdArray(points)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by satisfaction categories
    /// </summary>
    public TicketQueryBuilder WithSatisfactionCategories(params int[] categoryIds)
    {
        if (categoryIds?.Length > 0)
        {
            AddCondition($"satisfactioncategories:{FormatIdArray(categoryIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by updated by email
    /// </summary>
    public TicketQueryBuilder WithUpdatedByEmail(string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            AddCondition($"updatedbyemail:{email}");
        }
        return this;
    }

    /// <summary>
    /// Filters by application name
    /// </summary>
    public TicketQueryBuilder WithAppName(string appName)
    {
        if (!string.IsNullOrWhiteSpace(appName))
        {
            AddCondition($"appname:\"{appName}\"");
        }
        return this;
    }

    /// <summary>
    /// Includes unlinked items
    /// </summary>
    public TicketQueryBuilder IncludeUnlinkedItems(bool include = true)
    {
        if (include)
        {
            AddCondition($"includeunlinkeditems:{include.ToString().ToLower()}");
        }
        return this;
    }

    /// <summary>
    /// Filters tickets that have comments
    /// </summary>
    public TicketQueryBuilder HasComment(bool hasComment = true)
    {
        AddCondition($"hascomment:{hasComment.ToString().ToLower()}");
        return this;
    }

    /// <summary>
    /// Filters by status category (open, closed, etc.)
    /// </summary>
    public TicketQueryBuilder WithStatusCategory(params int[] categoryIds)
    {
        if (categoryIds?.Length > 0)
        {
            AddCondition($"statuscategory:{FormatIdArray(categoryIds)}");
        }
        return this;
    }

    /// <summary>
    /// Filters by external reference IDs
    /// </summary>
    public TicketQueryBuilder WithExternalReferenceIds(params string[] referenceIds)
    {
        if (referenceIds?.Length > 0)
        {
            var formattedIds = string.Join(",", referenceIds.Select(id => $"\"{id}\""));
            AddCondition($"externalreferenceids:[{formattedIds}]");
        }
        return this;
    }

    /// <summary>
    /// Filters by requester email
    /// </summary>
    public TicketQueryBuilder WithRequesterEmail(string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            AddCondition($"requesteremail:\"{email}\"");
        }
        return this;
    }

    /// <summary>
    /// Filters by requester phone
    /// </summary>
    public TicketQueryBuilder WithRequesterPhone(string phone)
    {
        if (!string.IsNullOrWhiteSpace(phone))
        {
            AddCondition($"requesterphone:\"{phone}\"");
        }
        return this;
    }

    /// <summary>
    /// Filters by agent email
    /// </summary>
    public TicketQueryBuilder WithAgentEmail(string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            AddCondition($"agentemail:\"{email}\"");
        }
        return this;
    }

    /// <summary>
    /// Filters by subject containing text
    /// </summary>
    public TicketQueryBuilder WithSubject(string subject)
    {
        if (!string.IsNullOrWhiteSpace(subject))
        {
            AddCondition($"subject:\"{subject}\"");
        }
        return this;
    }

    /// <summary>
    /// Applies the query to TicketQueryParameters
    /// </summary>
    public TicketQueryParameters ApplyTo(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        parameters.Q = Build();
        return parameters;
    }

    /// <summary>
    /// Creates a new TicketQueryParameters with the query applied
    /// </summary>
    public TicketQueryParameters ToParameters()
    {
        return ApplyTo(null);
    }

    /// <summary>
    /// Sets the OrderBy field in the parameters
    /// </summary>
    public TicketQueryParameters WithOrderBy(string orderBy)
    {
        var parameters = ToParameters();
        parameters.OrderBy = orderBy;
        return parameters;
    }

    /// <summary>
    /// Sets page number
    /// </summary>
    public TicketQueryParameters WithPage(int page)
    {
        var parameters = ToParameters();
        parameters.Page = page;
        return parameters;
    }

    /// <summary>
    /// Sets per page count
    /// </summary>
    public TicketQueryParameters WithPerPage(int perPage)
    {
        var parameters = ToParameters();
        parameters.PerPage = perPage;
        return parameters;
    }
}
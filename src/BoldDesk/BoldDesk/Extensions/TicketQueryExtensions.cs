using BoldDesk.Models;

namespace BoldDesk;

/// <summary>
/// Extension methods for query building
/// </summary>
public static class TicketQueryExtensions
{
    public static TicketQueryParameters WithDateRange(this TicketQueryParameters parameters, string field, DateTime from, DateTime to)
    {
        var dateFilter = $"{field}:{{\"from\":\"{from:yyyy-MM-ddTHH:mm:ss.fffZ}\",\"to\":\"{to:yyyy-MM-ddTHH:mm:ss.fffZ}\"}}";
        
        if (string.IsNullOrWhiteSpace(parameters.Q))
        {
            parameters.Q = dateFilter;
        }
        else
        {
            parameters.Q += $" AND {dateFilter}";
        }
        
        return parameters;
    }

    public static TicketQueryParameters WithStatus(this TicketQueryParameters parameters, params int[] statusIds)
    {
        if (statusIds?.Length > 0)
        {
            var statusFilter = $"status:[{string.Join(",", statusIds)}]";
            
            if (string.IsNullOrWhiteSpace(parameters.Q))
            {
                parameters.Q = statusFilter;
            }
            else
            {
                parameters.Q += $" AND {statusFilter}";
            }
        }
        
        return parameters;
    }

    public static TicketQueryParameters WithPriority(this TicketQueryParameters parameters, params int[] priorityIds)
    {
        if (priorityIds?.Length > 0)
        {
            var priorityFilter = $"priority:[{string.Join(",", priorityIds)}]";
            
            if (string.IsNullOrWhiteSpace(parameters.Q))
            {
                parameters.Q = priorityFilter;
            }
            else
            {
                parameters.Q += $" AND {priorityFilter}";
            }
        }
        
        return parameters;
    }

    public static TicketQueryParameters WithAgent(this TicketQueryParameters parameters, params int[] agentIds)
    {
        if (agentIds?.Length > 0)
        {
            var agentFilter = $"agents:[{string.Join(",", agentIds)}]";
            
            if (string.IsNullOrWhiteSpace(parameters.Q))
            {
                parameters.Q = agentFilter;
            }
            else
            {
                parameters.Q += $" AND {agentFilter}";
            }
        }
        
        return parameters;
    }

    public static TicketQueryParameters WithCreatedToday(this TicketQueryParameters parameters)
    {
        var todayFilter = "createdon:today";
        
        if (string.IsNullOrWhiteSpace(parameters.Q))
        {
            parameters.Q = todayFilter;
        }
        else
        {
            parameters.Q += $" AND {todayFilter}";
        }
        
        return parameters;
    }

    public static TicketQueryParameters WithCreatedThisWeek(this TicketQueryParameters parameters)
    {
        var weekFilter = "createdon:thisweek";
        
        if (string.IsNullOrWhiteSpace(parameters.Q))
        {
            parameters.Q = weekFilter;
        }
        else
        {
            parameters.Q += $" AND {weekFilter}";
        }
        
        return parameters;
    }
}
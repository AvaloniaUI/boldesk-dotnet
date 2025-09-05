namespace BoldDesk.Models;

public class TicketQueryParameters
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 100; // Max allowed
    public bool RequiresCounts { get; set; } = true;
    public string? Q { get; set; } // Advanced query filter
    public string? SortBy { get; set; }
    public OrderBy? OrderBy { get; set; } = Models.OrderBy.Descending;
}
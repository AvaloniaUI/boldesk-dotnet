namespace BoldDesk.Models;

public class WorklogQueryParameters
{
    public int Page { get; set; } = 1;
    public int PerPage { get; set; } = 100; // Max allowed
    public bool RequiresCounts { get; set; } = true;
    public string? OrderBy { get; set; }
    public DateTime? LastCreatedDateFrom { get; set; }
    public DateTime? LastCreatedDateTo { get; set; }
    public DateTime? LastUpdatedDateFrom { get; set; }
    public DateTime? LastUpdatedDateTo { get; set; }
    public bool IncludeDeletedWorklogs { get; set; } = false;
}
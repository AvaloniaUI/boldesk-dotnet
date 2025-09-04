namespace BoldDesk.Models;

public class RateLimitInfo
{
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public DateTime Reset { get; set; }
}
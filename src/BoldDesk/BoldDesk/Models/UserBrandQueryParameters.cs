namespace BoldDesk.Models;

public class UserBrandQueryParameters
{
    /// <summary>
    /// Any string associated with the brand name
    /// </summary>
    public string? Filter { get; set; }
    
    /// <summary>
    /// If the value is true, deactivated brands will also be included
    /// Default: false
    /// </summary>
    public bool NeedToIncludeDeactivatedBrands { get; set; } = false;
}
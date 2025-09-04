using BoldDesk.Models;

namespace BoldDesk.Extensions;

/// <summary>
/// Extension methods for working with brands
/// </summary>
public static class BrandExtensions
{
    /// <summary>
    /// Gets tickets for a specific brand
    /// </summary>
    public static async Task<BoldDeskResponse<Ticket>> GetTicketsByBrandAsync(
        this IBoldDeskService service, 
        int brandId,
        TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        
        // Add brand filter to Q parameter
        var brandFilter = $"brands:[{brandId}]";
        
        if (string.IsNullOrWhiteSpace(parameters.Q))
        {
            parameters.Q = brandFilter;
        }
        else
        {
            // Append to existing Q parameter
            parameters.Q = $"{parameters.Q} AND {brandFilter}";
        }
        
        return await service.GetTicketsAsync(parameters);
    }
    
    /// <summary>
    /// Filters tickets by brand
    /// </summary>
    public static IEnumerable<Ticket> FilterByBrand(this IEnumerable<Ticket> tickets, int brandId)
    {
        return tickets.Where(t => t.BrandId == brandId);
    }
    
    /// <summary>
    /// Filters tickets by brand name
    /// </summary>
    public static IEnumerable<Ticket> FilterByBrand(this IEnumerable<Ticket> tickets, string brandName)
    {
        return tickets.Where(t => t.Brand?.Equals(brandName, StringComparison.OrdinalIgnoreCase) == true);
    }
    
    /// <summary>
    /// Groups tickets by brand
    /// </summary>
    public static IEnumerable<IGrouping<int?, Ticket>> GroupByBrand(this IEnumerable<Ticket> tickets)
    {
        return tickets.GroupBy(t => t.BrandId);
    }
    
    /// <summary>
    /// Gets active brands (published and not disabled)
    /// </summary>
    public static IEnumerable<Brand> GetActiveBrands(this IEnumerable<Brand> brands)
    {
        return brands.Where(b => b.IsPublished && !b.IsDisabled);
    }
    
    /// <summary>
    /// Gets active user brands (not deactivated with active customer portal)
    /// </summary>
    public static IEnumerable<UserBrand> GetActiveUserBrands(this IEnumerable<UserBrand> userBrands)
    {
        return userBrands.Where(ub => !ub.IsDeactivated && ub.IsCustomerPortalActive);
    }
    
    /// <summary>
    /// Finds a brand by ID
    /// </summary>
    public static Brand? FindBrandById(this IEnumerable<Brand> brands, int brandId)
    {
        return brands.FirstOrDefault(b => b.BrandId == brandId);
    }
    
    /// <summary>
    /// Finds a brand by name (case-insensitive)
    /// </summary>
    public static Brand? FindBrandByName(this IEnumerable<Brand> brands, string brandName)
    {
        return brands.FirstOrDefault(b => b.BrandName.Equals(brandName, StringComparison.OrdinalIgnoreCase));
    }
}
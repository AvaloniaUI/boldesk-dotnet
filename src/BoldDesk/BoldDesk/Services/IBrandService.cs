using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Service for managing BoldDesk brands
/// </summary>
public interface IBrandService
{
    /// <summary>
    /// Fetches a list of brands for your organization
    /// </summary>
    Task<BoldDeskResponse<Brand>> GetBrandsAsync();

    /// <summary>
    /// Fetches a list of brands based on user preferences
    /// </summary>
    Task<BoldDeskResponse<UserBrand>> GetUserBrandsAsync(UserBrandQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets all brands using pagination (if needed in future)
    /// </summary>
    IAsyncEnumerable<Brand> GetAllBrandsAsync(CancellationToken cancellationToken = default);
}
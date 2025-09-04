using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Web;
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Implementation of BoldDesk brand operations
/// </summary>
public class BrandService : BaseService, IBrandService
{
    public BrandService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions) 
        : base(httpClient, baseUrl, jsonOptions)
    {
    }

    /// <summary>
    /// Fetches a list of brands for your organization
    /// </summary>
    public async Task<BoldDeskResponse<Brand>> GetBrandsAsync()
    {
        var url = $"{BaseUrl}/brands";
        return await ExecuteRequestAsync<BoldDeskResponse<Brand>>(url);
    }

    /// <summary>
    /// Fetches a list of brands based on user preferences
    /// </summary>
    public async Task<BoldDeskResponse<UserBrand>> GetUserBrandsAsync(UserBrandQueryParameters? parameters = null)
    {
        parameters ??= new UserBrandQueryParameters();
        var url = BuildUserBrandsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<UserBrand>>(url);
    }

    /// <summary>
    /// Gets all brands using pagination
    /// </summary>
    public async IAsyncEnumerable<Brand> GetAllBrandsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await GetBrandsAsync();
        
        foreach (var brand in response.Result)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;
                
            yield return brand;
        }
    }

    private string BuildUserBrandsUrl(UserBrandQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/user_brands");
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrWhiteSpace(parameters.Filter))
        {
            query["filter"] = parameters.Filter;
        }

        if (parameters.NeedToIncludeDeactivatedBrands)
        {
            query["needToIncludeDeactivatedBrands"] = "true";
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}
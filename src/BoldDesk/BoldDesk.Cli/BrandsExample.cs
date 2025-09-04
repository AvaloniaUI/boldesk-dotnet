using BoldDesk;
using BoldDesk.Exceptions;
using BoldDesk.Models;

namespace BoldDesk.Examples;

/// <summary>
/// Example demonstrating how to use the Brands API endpoints
/// </summary>
public class BrandsExample
{
    public static async Task RunExampleAsync(string domain, string apiKey)
    {
        using var service = new BoldDeskService(domain, apiKey);
        
        try
        {
            // Example 1: Get all brands
            Console.WriteLine("Fetching all brands...");
            var brandsResponse = await service.GetBrandsAsync();
            
            Console.WriteLine($"Found {brandsResponse.Result.Count} brands:");
            foreach (var brand in brandsResponse.Result)
            {
                Console.WriteLine($"  - Brand #{brand.BrandId}: {brand.BrandName}");
                Console.WriteLine($"    Published: {brand.IsPublished}, Disabled: {brand.IsDisabled}");
            }
            
            // Example 2: Get user brands with filter
            Console.WriteLine("\nFetching user brands...");
            var userBrandParams = new UserBrandQueryParameters
            {
                Filter = "Technology",
                NeedToIncludeDeactivatedBrands = true
            };
            
            var userBrandsResponse = await service.GetUserBrandsAsync(userBrandParams);
            
            Console.WriteLine($"Found {userBrandsResponse.Result.Count} user brands:");
            foreach (var userBrand in userBrandsResponse.Result)
            {
                Console.WriteLine($"  - {userBrand.Value}: {userBrand.Text}");
                Console.WriteLine($"    Default: {userBrand.IsDefault}, Deactivated: {userBrand.IsDeactivated}");
                Console.WriteLine($"    Customer Portal Active: {userBrand.IsCustomerPortalActive}");
                Console.WriteLine($"    KB Enabled: {userBrand.IsKbEnabled}");
                
                if (!string.IsNullOrEmpty(userBrand.LogoLink))
                {
                    Console.WriteLine($"    Logo: {userBrand.LogoLink}");
                }
            }
            
            // Example 3: Get user brands without filter (all active brands)
            Console.WriteLine("\nFetching all active user brands...");
            var allUserBrandsResponse = await service.GetUserBrandsAsync();
            
            Console.WriteLine($"Found {allUserBrandsResponse.Result.Count} active user brands");
            
            // Check rate limit information
            if (service.LastRateLimitInfo != null)
            {
                Console.WriteLine($"\nRate Limit Status:");
                Console.WriteLine($"  Limit: {service.LastRateLimitInfo.Limit} calls/min");
                Console.WriteLine($"  Remaining: {service.LastRateLimitInfo.Remaining} calls");
                Console.WriteLine($"  Reset: {service.LastRateLimitInfo.Reset:yyyy-MM-dd HH:mm:ss UTC}");
            }
        }
        catch (BoldDeskAuthenticationException ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
            Console.WriteLine("Please check your API key.");
        }
        catch (BoldDeskValidationException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
            if (ex.HasFieldErrors())
            {
                foreach (var field in ex.FieldErrors)
                {
                    Console.WriteLine($"  {field.Key}: {string.Join(", ", field.Value)}");
                }
            }
        }
        catch (BoldDeskRateLimitException ex)
        {
            Console.WriteLine($"Rate limit exceeded: {ex.Message}");
            if (ex.GetWaitTime() is TimeSpan waitTime)
            {
                Console.WriteLine($"Please wait {waitTime.TotalSeconds:F1} seconds before retrying.");
            }
        }
        catch (BoldDeskApiException ex)
        {
            Console.WriteLine($"API Error: {ex.Message}");
            Console.WriteLine($"Status Code: {ex.StatusCode}");
            
            if (ex.Errors.Any())
            {
                Console.WriteLine("Error details:");
                foreach (var error in ex.Errors)
                {
                    Console.WriteLine($"  Field: {error.Field}, Type: {error.ErrorType}");
                    Console.WriteLine($"  Message: {error.ErrorMessage}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
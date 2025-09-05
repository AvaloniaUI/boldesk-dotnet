using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using BoldDesk.Models;

namespace BoldDesk.Services;

public class FieldService : BaseService, IFieldService
{
    public FieldService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions) 
        : base(httpClient, baseUrl, jsonOptions)
    {
    }

    public async Task<FieldOptionsResponse> ListFieldOptionsAsync(string apiName, FieldOptionQueryParameters? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(apiName))
            throw new ArgumentException("API name cannot be null or empty.", nameof(apiName));

        var queryParams = new List<string>();
        
        if (parameters != null)
        {
            if (!string.IsNullOrEmpty(parameters.Filter))
                queryParams.Add($"Filter={HttpUtility.UrlEncode(parameters.Filter)}");
            
            if (parameters.ParentOptionId.HasValue)
                queryParams.Add($"parentOptionId={parameters.ParentOptionId}");
            
            queryParams.Add($"Page={parameters.Page}");
            queryParams.Add($"PerPage={parameters.PerPage}");
            
            if (parameters.RequiresCounts)
                queryParams.Add("RequiresCounts=true");
            
            if (!string.IsNullOrEmpty(parameters.OrderBy))
                queryParams.Add($"OrderBy={HttpUtility.UrlEncode(parameters.OrderBy)}");
            
            if (!string.IsNullOrEmpty(parameters.ExclusionIds))
                queryParams.Add($"exclusionIds={HttpUtility.UrlEncode(parameters.ExclusionIds)}");
            
            if (parameters.IncludeReadOnlyAlso)
                queryParams.Add("includeReadOnlyAlso=true");
        }

        var queryString = queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : "";
        var url = $"{BaseUrl}/fields/collection/{apiName}/options{queryString}";
        
        return await ExecuteRequestAsync<FieldOptionsResponse>(url);
    }

    public async Task<FieldApiResponse> AddFieldOptionsAsync(string apiName, List<string> fieldOptions)
    {
        if (string.IsNullOrWhiteSpace(apiName))
            throw new ArgumentException("API name cannot be null or empty.", nameof(apiName));
        
        if (fieldOptions == null || fieldOptions.Count == 0)
            throw new ArgumentException("Field options cannot be null or empty.", nameof(fieldOptions));

        var url = $"{BaseUrl}/fields/{apiName}/option_values";
        var request = new AddFieldOptionsRequest { FieldOptions = fieldOptions };
        
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<FieldApiResponse>(responseContent, JsonOptions);
        
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize API response");
        
        return result;
    }

    public async Task<FieldApiResponse> RemoveFieldOptionAsync(long fieldOptionId)
    {
        if (fieldOptionId <= 0)
            throw new ArgumentException("Field option ID must be greater than 0.", nameof(fieldOptionId));

        var url = $"{BaseUrl}/fields/option_values/{fieldOptionId}";
        
        var response = await HttpClient.DeleteAsync(url);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<FieldApiResponse>(content, JsonOptions);
        
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize API response");
        
        return result;
    }

    public async Task<FieldApiResponse> SetFieldOptionReadOnlyAsync(long fieldOptionId, bool isReadOnly)
    {
        if (fieldOptionId <= 0)
            throw new ArgumentException("Field option ID must be greater than 0.", nameof(fieldOptionId));

        var url = $"{BaseUrl}/fields/option_values/{fieldOptionId}/readonly/{isReadOnly.ToString().ToLower()}";
        
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
        var response = await HttpClient.SendAsync(request);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<FieldApiResponse>(content, JsonOptions);
        
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize API response");
        
        return result;
    }

    public async Task<FieldApiResponse> ChangeFieldOptionPositionAsync(int fieldId, long fieldOptionId, FieldPositionChangeParameters parameters)
    {
        if (fieldId <= 0)
            throw new ArgumentException("Field ID must be greater than 0.", nameof(fieldId));
        
        if (fieldOptionId <= 0)
            throw new ArgumentException("Field option ID must be greater than 0.", nameof(fieldOptionId));
        
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        var queryParams = new List<string>
        {
            $"toPosition={parameters.ToPosition}",
            $"isSortByAlphabeticalOrder={parameters.IsSortByAlphabeticalOrder.ToString().ToLower()}",
            $"isMoveToTopPosition={parameters.IsMoveToTopPosition.ToString().ToLower()}",
            $"isMoveToBottomPosition={parameters.IsMoveToBottomPosition.ToString().ToLower()}"
        };

        var queryString = $"?{string.Join("&", queryParams)}";
        var url = $"{BaseUrl}/fields/{fieldId}/option_values/{fieldOptionId}/position_change{queryString}";
        
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new StringContent("", Encoding.UTF8)
        };
        request.Headers.Add("content-length", "0");
        
        var response = await HttpClient.SendAsync(request);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<FieldApiResponse>(content, JsonOptions);
        
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize API response");
        
        return result;
    }

    public async Task<FieldApiResponse> SetDefaultFieldOptionAsync(int fieldId, long fieldOptionId)
    {
        if (fieldId <= 0)
            throw new ArgumentException("Field ID must be greater than 0.", nameof(fieldId));
        
        if (fieldOptionId <= 0)
            throw new ArgumentException("Field option ID must be greater than 0.", nameof(fieldOptionId));

        var url = $"{BaseUrl}/fields/{fieldId}/option_values/{fieldOptionId}/set_default";
        
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new StringContent("", Encoding.UTF8)
        };
        request.Headers.Add("content-length", "0");
        
        var response = await HttpClient.SendAsync(request);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<FieldApiResponse>(content, JsonOptions);
        
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize API response");
        
        return result;
    }

    public async Task<FieldApiResponse> RemoveDefaultFieldOptionAsync(int fieldId, long fieldOptionId)
    {
        if (fieldId <= 0)
            throw new ArgumentException("Field ID must be greater than 0.", nameof(fieldId));
        
        if (fieldOptionId <= 0)
            throw new ArgumentException("Field option ID must be greater than 0.", nameof(fieldOptionId));

        var url = $"{BaseUrl}/fields/{fieldId}/option_values/{fieldOptionId}/remove_default";
        
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new StringContent("", Encoding.UTF8)
        };
        request.Headers.Add("content-length", "0");
        
        var response = await HttpClient.SendAsync(request);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<FieldApiResponse>(content, JsonOptions);
        
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize API response");
        
        return result;
    }
}
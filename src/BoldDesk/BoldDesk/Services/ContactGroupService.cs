using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Service implementation for managing contact groups in BoldDesk
/// </summary>
public class ContactGroupService : BaseService, IContactGroupService
{
    public ContactGroupService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions) : base(httpClient, baseUrl, jsonOptions)
    {
    }
    
    public async Task<BoldDeskResponse<ContactGroup>> ListContactGroupsAsync(ContactGroupQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildContactGroupsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<ContactGroup>>(url);
    }
    
    public async IAsyncEnumerable<ContactGroup> ListAllContactGroupsAsync(ContactGroupQueryParameters? parameters = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new ContactGroupQueryParameters();
        parameters.Page = 1;
        parameters.PerPage = 100;
        
        while (true)
        {
            var response = await ListContactGroupsAsync(parameters, cancellationToken);
            
            if (response.Result == null || !response.Result.Any())
                yield break;
            
            foreach (var group in response.Result)
            {
                yield return group;
            }
            
            if (response.Result.Count < parameters.PerPage)
                yield break;
            
            parameters.Page++;
        }
    }
    
    public async Task<ContactGroupDetail> GetContactGroupAsync(long contactGroupId, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contact_groups/{contactGroupId}";
        return await ExecuteRequestAsync<ContactGroupDetail>(url);
    }
    
    public async Task<ContactGroupDetail> GetContactGroupByNameAsync(string contactGroupName, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contact_groups/{Uri.EscapeDataString(contactGroupName)}";
        return await ExecuteRequestAsync<ContactGroupDetail>(url);
    }
    
    public async Task<ContactGroupOperationResponse> AddContactGroupAsync(AddContactGroupRequest request, bool skipDependencyValidation = false, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var url = $"{BaseUrl}/contact_groups";
        if (skipDependencyValidation)
        {
            url += "?skipDependencyValidation=true";
        }
        
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ContactGroupOperationResponse>(responseContent, JsonOptions) 
            ?? new ContactGroupOperationResponse();
    }
    
    public async Task<ContactGroupOperationResponse> UpdateContactGroupAsync(long contactGroupId, UpdateContactGroupFieldsRequest request, bool skipDependencyValidation = false, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var url = $"{BaseUrl}/contact_groups/{contactGroupId}";
        if (skipDependencyValidation)
        {
            url += "?skipDependencyValidation=true";
        }
        
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ContactGroupOperationResponse>(responseContent, JsonOptions) 
            ?? new ContactGroupOperationResponse();
    }
    
    public async Task<ContactGroupDeleteResponse> DeleteContactGroupsAsync(DeleteContactGroupsRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/contact_groups")
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ContactGroupDeleteResponse>(responseContent, JsonOptions) 
            ?? new ContactGroupDeleteResponse();
    }
    
    public async Task<BoldDeskResponse<Contact>> GetContactsByGroupAsync(long contactGroupId, ContactGroupContactsQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildContactsByGroupUrl(contactGroupId, parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<Contact>>(url);
    }
    
    public async IAsyncEnumerable<Contact> GetAllContactsByGroupAsync(
        long contactGroupId,
        ContactGroupContactsQueryParameters? parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new ContactGroupContactsQueryParameters();
        parameters.Page = 1;
        parameters.PerPage = 100;
        
        while (true)
        {
            var response = await GetContactsByGroupAsync(contactGroupId, parameters, cancellationToken);
            
            if (response.Result == null || !response.Result.Any())
                yield break;
            
            foreach (var contact in response.Result)
            {
                yield return contact;
            }
            
            if (response.Result.Count < parameters.PerPage)
                yield break;
            
            parameters.Page++;
        }
    }
    
    public async Task<AddContactToGroupResponse> AddContactsToGroupAsync(long contactGroupId, List<AddContactToGroupRequest> contacts, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var url = $"{BaseUrl}/contact_groups/{contactGroupId}/contacts";
        
        var json = JsonSerializer.Serialize(contacts, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<AddContactToGroupResponse>(responseContent, JsonOptions) 
            ?? new AddContactToGroupResponse();
    }
    
    public async Task<ContactGroupOperationResponse> RemoveContactFromGroupAsync(long contactGroupId, long userId, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var url = $"{BaseUrl}/contact_groups/{contactGroupId}/contacts/{userId}";
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url);
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ContactGroupOperationResponse>(responseContent, JsonOptions) 
            ?? new ContactGroupOperationResponse();
    }
    
    public async Task<ContactGroupOperationResponse> ChangeTicketAccessScopeAsync(long contactGroupId, long contactId, bool viewAllTickets, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var url = $"{BaseUrl}/contact_groups/{contactGroupId}/change_ticket_access_scope/{contactId}?viewalltickets={viewAllTickets.ToString().ToLower()}";
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Patch, url);
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ContactGroupOperationResponse>(responseContent, JsonOptions) 
            ?? new ContactGroupOperationResponse();
    }
    
    public async Task<BoldDeskResponse<DomainInfo>> GetContactGroupDomainsAsync(ContactGroupDomainsQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildDomainsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<DomainInfo>>(url);
    }
    
    public async IAsyncEnumerable<DomainInfo> GetAllContactGroupDomainsAsync(
        ContactGroupDomainsQueryParameters? parameters = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new ContactGroupDomainsQueryParameters();
        parameters.Page = 1;
        parameters.PerPage = 100;
        
        while (true)
        {
            var response = await GetContactGroupDomainsAsync(parameters, cancellationToken);
            
            if (response.Result == null || !response.Result.Any())
                yield break;
            
            foreach (var domain in response.Result)
            {
                yield return domain;
            }
            
            if (response.Result.Count < parameters.PerPage)
                yield break;
            
            parameters.Page++;
        }
    }
    
    public async Task<List<ContactGroupField>> GetContactGroupFieldsAsync(ContactGroupFieldsQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildFieldsUrl(parameters);
        var response = await ExecuteRequestAsync<ContactGroupFieldsResponse>(url);
        return response.Result ?? new List<ContactGroupField>();
    }
    
    public async Task<object> GetContactGroupFieldAsync(
        int fieldId,
        CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contact_group_fields/{fieldId}";
        return await ExecuteRequestAsync<object>(url);
    }
    
    public async Task<ContactGroupNotesResponse> GetContactGroupNotesAsync(long contactGroupId, ContactGroupNotesQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildNotesUrl(contactGroupId, parameters);
        return await ExecuteRequestAsync<ContactGroupNotesResponse>(url);
    }
    
    public async Task<ContactGroupOperationResponse> AddContactGroupNoteAsync(long contactGroupId, ContactGroupNoteRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var url = $"{BaseUrl}/contact_groups/{contactGroupId}/notes";
        
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ContactGroupOperationResponse>(responseContent, JsonOptions) 
            ?? new ContactGroupOperationResponse();
    }
    
    public async Task<ContactGroupOperationResponse> UpdateContactGroupNoteAsync(long noteId, ContactGroupNoteRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var url = $"{BaseUrl}/contact_groups/notes/{noteId}";
        
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ContactGroupOperationResponse>(responseContent, JsonOptions) 
            ?? new ContactGroupOperationResponse();
    }
    
    public async Task<ContactGroupOperationResponse> DeleteContactGroupNoteAsync(long noteId, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        
        var url = $"{BaseUrl}/contact_groups/notes/{noteId}";
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url);
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ContactGroupOperationResponse>(responseContent, JsonOptions) 
            ?? new ContactGroupOperationResponse();
    }
    
    private string BuildContactGroupsUrl(ContactGroupQueryParameters? parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/contact_groups");
        var query = HttpUtility.ParseQueryString(string.Empty);
        
        if (parameters != null)
        {
            if (parameters.Page.HasValue)
                query["page"] = parameters.Page.ToString();
            if (parameters.PerPage.HasValue)
                query["perPage"] = parameters.PerPage.ToString();
            if (parameters.RequiresCounts.HasValue)
                query["requiresCounts"] = parameters.RequiresCounts.ToString()!.ToLower();
            if (!string.IsNullOrWhiteSpace(parameters.Filter))
                query["filter"] = parameters.Filter;
            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
                query["orderBy"] = parameters.OrderBy;
            if (parameters.Q != null && parameters.Q.Length > 0)
            {
                foreach (var q in parameters.Q)
                {
                    query.Add("Q", q);
                }
            }
        }
        
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
    
    private string BuildContactsByGroupUrl(long contactGroupId, ContactGroupContactsQueryParameters? parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/contact_groups/{contactGroupId}/contacts");
        var query = HttpUtility.ParseQueryString(string.Empty);
        
        if (parameters != null)
        {
            if (parameters.Page.HasValue)
                query["page"] = parameters.Page.ToString();
            if (parameters.PerPage.HasValue)
                query["perPage"] = parameters.PerPage.ToString();
            if (parameters.RequiresCounts.HasValue)
                query["requiresCounts"] = parameters.RequiresCounts.ToString()!.ToLower();
            if (!string.IsNullOrWhiteSpace(parameters.Filter))
                query["filter"] = parameters.Filter;
            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
                query["orderBy"] = parameters.OrderBy;
        }
        
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
    
    private string BuildDomainsUrl(ContactGroupDomainsQueryParameters? parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/contact_groups/domains");
        var query = HttpUtility.ParseQueryString(string.Empty);
        
        if (parameters != null)
        {
            if (parameters.Page.HasValue)
                query["page"] = parameters.Page.ToString();
            if (parameters.PerPage.HasValue)
                query["perPage"] = parameters.PerPage.ToString();
            if (parameters.RequiresCounts.HasValue)
                query["requiresCounts"] = parameters.RequiresCounts.ToString()!.ToLower();
            if (!string.IsNullOrWhiteSpace(parameters.Filter))
                query["filter"] = parameters.Filter;
            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
                query["orderBy"] = parameters.OrderBy;
            if (parameters.DomainIds != null && parameters.DomainIds.Length > 0)
            {
                foreach (var domainId in parameters.DomainIds)
                {
                    query.Add("domainIds", domainId.ToString());
                }
            }
        }
        
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
    
    private string BuildFieldsUrl(ContactGroupFieldsQueryParameters? parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/contact_group_fields");
        var query = HttpUtility.ParseQueryString(string.Empty);
        
        if (parameters != null)
        {
            if (parameters.Page.HasValue)
                query["page"] = parameters.Page.ToString();
            if (parameters.PerPage.HasValue)
                query["perPage"] = parameters.PerPage.ToString();
            if (parameters.RequiresCounts.HasValue)
                query["requiresCounts"] = parameters.RequiresCounts.ToString()!.ToLower();
            if (!string.IsNullOrWhiteSpace(parameters.Filter))
                query["filter"] = parameters.Filter;
            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
                query["orderBy"] = parameters.OrderBy;
        }
        
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
    
    private string BuildNotesUrl(long contactGroupId, ContactGroupNotesQueryParameters? parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/contact_groups/{contactGroupId}/notes");
        var query = HttpUtility.ParseQueryString(string.Empty);
        
        if (parameters != null)
        {
            if (parameters.Page.HasValue)
                query["page"] = parameters.Page.ToString();
            if (parameters.PerPage.HasValue)
                query["perPage"] = parameters.PerPage.ToString();
            if (parameters.RequiresCounts.HasValue)
                query["requiresCounts"] = parameters.RequiresCounts.ToString()!.ToLower();
            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
                query["orderBy"] = parameters.OrderBy;
        }
        
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
    
    private class ContactGroupFieldsResponse
    {
        [JsonPropertyName("result")]
        public List<ContactGroupField>? Result { get; set; }
    }
}
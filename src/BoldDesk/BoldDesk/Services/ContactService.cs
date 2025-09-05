using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web;
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Service implementation for managing contacts in BoldDesk
/// </summary>
public class ContactService : BaseService, IContactService
{
    public ContactService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions) 
        : base(httpClient, baseUrl, jsonOptions)
    {
    }

    public async Task<BoldDeskResponse<Contact>> ListContactsAsync(ContactQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildContactsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<Contact>>(url);
    }

    public async IAsyncEnumerable<Contact> ListAllContactsAsync(ContactQueryParameters? parameters = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new ContactQueryParameters();
        parameters.Page = 1;
        parameters.PerPage = 100;

        while (true)
        {
            var response = await ListContactsAsync(parameters, cancellationToken);

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

    public async Task<Contact> GetContactAsync(long userId, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{userId}";
        return await ExecuteRequestAsync<Contact>(url);
    }

    public async Task<Contact> GetContactByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{Uri.EscapeDataString(email)}";
        return await ExecuteRequestAsync<Contact>(url);
    }

    public async Task<ContactOperationResponse> CreateContactAsync(CreateContactRequest request, bool skipDependencyValidation = false, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts";
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
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactOperationResponse();
        }
        
        return JsonSerializer.Deserialize<ContactOperationResponse>(responseContent, JsonOptions) ?? new ContactOperationResponse();
    }

    public async Task<ContactMessageResponse> UpdateContactAsync(long contactId, UpdateContactRequest request, bool skipDependencyValidation = false, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{contactId}";
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
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactMessageResponse();
        }
        
        return JsonSerializer.Deserialize<ContactMessageResponse>(responseContent, JsonOptions) ?? new ContactMessageResponse();
    }

    public async Task<ContactDeleteResponse> DeleteContactsAsync(DeleteContactRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts";

        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactDeleteResponse();
        }
        
        return JsonSerializer.Deserialize<ContactDeleteResponse>(responseContent, JsonOptions) ?? new ContactDeleteResponse();
    }

    public async Task<ContactDeleteResponse> PermanentDeleteContactsAsync(PermanentDeleteContactRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/permanent_delete";

        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactDeleteResponse();
        }
        
        return JsonSerializer.Deserialize<ContactDeleteResponse>(responseContent, JsonOptions) ?? new ContactDeleteResponse();
    }

    public async Task<ContactMessageResponse> BlockContactAsync(long contactId, bool markTicketAsSpam = false, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{contactId}/block";
        if (markTicketAsSpam)
        {
            url += "?markTicketAsSpam=true";
        }
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Patch, url);
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactMessageResponse();
        }
        
        return JsonSerializer.Deserialize<ContactMessageResponse>(responseContent, JsonOptions) ?? new ContactMessageResponse();
    }

    public async Task<ContactMessageResponse> UnblockContactAsync(long contactId, bool removeTicketFromSpam = false, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{contactId}/unblock";
        if (removeTicketFromSpam)
        {
            url += "?removeTicketFromSpam=true";
        }
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Patch, url);
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactMessageResponse();
        }
        
        return JsonSerializer.Deserialize<ContactMessageResponse>(responseContent, JsonOptions) ?? new ContactMessageResponse();
    }

    public async Task<AddContactGroupResponse> AddContactGroupsAsync(long contactId, List<AddContactGroupsRequest> request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{contactId}/contactgroups";

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
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new AddContactGroupResponse();
        }
        
        return JsonSerializer.Deserialize<AddContactGroupResponse>(responseContent, JsonOptions) ?? new AddContactGroupResponse();
    }

    public async Task<RemoveContactGroupResponse> RemoveContactGroupsAsync(long contactId, RemoveContactGroupsRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{contactId}/contactgroups";

        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = content
        };
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new RemoveContactGroupResponse();
        }
        
        return JsonSerializer.Deserialize<RemoveContactGroupResponse>(responseContent, JsonOptions) ?? new RemoveContactGroupResponse();
    }

    public async Task<BoldDeskResponse<ContactGroupInfo>> GetContactGroupsByContactIdAsync(long contactId, ContactGroupQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildContactGroupsByContactIdUrl(contactId, parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<ContactGroupInfo>>(url);
    }

    public async Task<ContactMessageResponse> ChangePrimaryContactGroupAsync(long contactId, long contactGroupId, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{contactId}/change_primary_contact_group/{contactGroupId}";
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Patch, url);
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactMessageResponse();
        }
        
        return JsonSerializer.Deserialize<ContactMessageResponse>(responseContent, JsonOptions) ?? new ContactMessageResponse();
    }

    public async Task<ContactMessageResponse> ConvertContactToAgentAsync(long userId, ConvertContactToAgentRequest request, bool skipDependencyValidation = true, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/convert_to_agent/{userId}";
        if (!skipDependencyValidation)
        {
            url += "?skipDependencyValidation=false";
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
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactMessageResponse();
        }
        
        return JsonSerializer.Deserialize<ContactMessageResponse>(responseContent, JsonOptions) ?? new ContactMessageResponse();
    }

    public async Task<ContactNotesResponse> ListContactNotesAsync(long contactId, ContactQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildContactNotesUrl(contactId, parameters);
        return await ExecuteRequestAsync<ContactNotesResponse>(url);
    }

    public async Task<ContactOperationResponse> AddContactNoteAsync(long contactId, ContactNoteRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/{contactId}/notes";

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
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactOperationResponse();
        }
        
        return JsonSerializer.Deserialize<ContactOperationResponse>(responseContent, JsonOptions) ?? new ContactOperationResponse();
    }

    public async Task<ContactMessageResponse> UpdateContactNoteAsync(long noteId, ContactNoteRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/notes/{noteId}";

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
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactMessageResponse();
        }
        
        return JsonSerializer.Deserialize<ContactMessageResponse>(responseContent, JsonOptions) ?? new ContactMessageResponse();
    }

    public async Task<ContactMessageResponse> DeleteContactNoteAsync(long noteId, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/notes/{noteId}";
        
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url);
        
        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        ParseRateLimitHeaders(response.Headers);
        
        if (!response.IsSuccessStatusCode)
        {
            await ThrowBoldDeskException(response);
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactMessageResponse();
        }
        
        return JsonSerializer.Deserialize<ContactMessageResponse>(responseContent, JsonOptions) ?? new ContactMessageResponse();
    }

    public async Task<ContactDeleteResponse> MergeContactsAsync(MergeContactRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contacts/merge";

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
        
        var responseContent = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return new ContactDeleteResponse();
        }
        
        return JsonSerializer.Deserialize<ContactDeleteResponse>(responseContent, JsonOptions) ?? new ContactDeleteResponse();
    }

    public async Task<BoldDeskResponse<ContactField>> ListContactFieldsAsync(ContactQueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = BuildContactFieldsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<ContactField>>(url);
    }

    public async Task<ContactField> GetContactFieldAsync(int contactFieldId, CancellationToken cancellationToken = default)
    {
        await EnsureRateLimitCompliance();
        var url = $"{BaseUrl}/contact_fields/{contactFieldId}";
        return await ExecuteRequestAsync<ContactField>(url);
    }

    private string BuildContactsUrl(ContactQueryParameters? parameters)
    {
        parameters ??= new ContactQueryParameters();
        var uriBuilder = new UriBuilder($"{BaseUrl}/contacts");
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (parameters.Q != null && parameters.Q.Length > 0)
        {
            for (int i = 0; i < parameters.Q.Length; i++)
            {
                query.Add("Q", parameters.Q[i]);
            }
        }

        if (!string.IsNullOrWhiteSpace(parameters.Filter))
        {
            query["Filter"] = parameters.Filter;
        }

        query["Page"] = parameters.Page.ToString();
        query["PerPage"] = parameters.PerPage.ToString();
        query["RequiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["OrderBy"] = parameters.OrderBy;
        }

        if (!string.IsNullOrWhiteSpace(parameters.View))
        {
            query["view"] = parameters.View;
        }

        if (parameters.ContactGroupId.HasValue)
        {
            query["contactGroupId"] = parameters.ContactGroupId.Value.ToString();
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildContactGroupsByContactIdUrl(long contactId, ContactGroupQueryParameters? parameters)
    {
        parameters ??= new ContactGroupQueryParameters();
        var uriBuilder = new UriBuilder($"{BaseUrl}/contacts/{contactId}/contact_groups");
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrWhiteSpace(parameters.Filter))
        {
            query["Filter"] = parameters.Filter;
        }

        query["Page"] = parameters.Page.ToString();
        query["PerPage"] = parameters.PerPage.ToString();
        query["RequiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["OrderBy"] = parameters.OrderBy;
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildContactNotesUrl(long contactId, ContactQueryParameters? parameters)
    {
        parameters ??= new ContactQueryParameters();
        var uriBuilder = new UriBuilder($"{BaseUrl}/contacts/{contactId}/notes");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["Page"] = parameters.Page.ToString();
        query["PerPage"] = parameters.PerPage.ToString();
        query["RequiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["OrderBy"] = parameters.OrderBy;
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildContactFieldsUrl(ContactQueryParameters? parameters)
    {
        parameters ??= new ContactQueryParameters();
        var uriBuilder = new UriBuilder($"{BaseUrl}/contact_fields");
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrWhiteSpace(parameters.Filter))
        {
            query["Filter"] = parameters.Filter;
        }

        query["Page"] = parameters.Page.ToString();
        query["PerPage"] = parameters.PerPage.ToString();
        query["RequiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["OrderBy"] = parameters.OrderBy;
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}
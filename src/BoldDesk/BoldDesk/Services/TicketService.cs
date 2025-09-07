using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web;
using BoldDesk.Extensions;
using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Implementation of BoldDesk ticket operations
/// </summary>
public class TicketService : BaseService, ITicketService
{
    public TicketService(HttpClient httpClient, string baseUrl, JsonSerializerOptions jsonOptions) 
        : base(httpClient, baseUrl, jsonOptions)
    {
    }

    /// <summary>
    /// Fetches a single page of tickets from the BoldDesk API
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> GetTicketsAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        var url = BuildTicketsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<Ticket>>(url);
    }

    /// <summary>
    /// Fetches all tickets using pagination, respecting rate limits
    /// </summary>
    public async IAsyncEnumerable<Ticket> GetAllTicketsAsync(
        TicketQueryParameters? parameters = null, 
        IProgress<string>? progress = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        parameters ??= new TicketQueryParameters();
        var currentPage = parameters.Page;
        var totalFetched = 0;
        var hasMorePages = true;

        while (hasMorePages && !cancellationToken.IsCancellationRequested)
        {
            await EnsureRateLimitCompliance();

            parameters.Page = currentPage;
            
            progress?.Report($"Fetching page {currentPage}...");
            
            var response = await GetTicketsAsync(parameters);
            
            if (response.Result.Count == 0)
            {
                hasMorePages = false;
                break;
            }

            foreach (var ticket in response.Result)
            {
                totalFetched++;
                yield return ticket;
            }

            // Check if we have more pages
            if (response.Count > 0 && totalFetched >= response.Count)
            {
                hasMorePages = false;
            }
            else if (response.Result.Count < parameters.PerPage)
            {
                hasMorePages = false;
            }
            else
            {
                currentPage++;
            }

            progress?.Report($"Fetched {totalFetched} tickets so far...");
        }

        progress?.Report($"Completed. Total tickets fetched: {totalFetched}");
    }

    /// <summary>
    /// Gets a count of tickets matching the query without fetching all data
    /// </summary>
    public async Task<int> GetTicketCountAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        
        // Set up parameters to get count with minimal data
        var countParams = new TicketQueryParameters
        {
            Page = 1,
            PerPage = 1,
            RequiresCounts = true,
            Q = parameters.Q,
            FilterId = parameters.FilterId,
            OrderBy = parameters.OrderBy
        };

        var response = await GetTicketsAsync(countParams);
        return response.Count;
    }

    /// <summary>
    /// Retrieves a single ticket by its ID
    /// </summary>
    public async Task<Ticket> GetTicketAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}";
        return await ExecuteRequestAsync<Ticket>(url);
    }

    /// <summary>
    /// Creates a new ticket
    /// </summary>
    public async Task<Ticket> CreateTicketAsync(CreateTicketRequest request)
    {
        var url = $"{BaseUrl}/tickets";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<Ticket>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Updates an existing ticket
    /// </summary>
    public async Task<Ticket> UpdateTicketAsync(int ticketId, UpdateTicketRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PutAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<Ticket>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Deletes a ticket (moves to trash)
    /// </summary>
    public async Task<bool> DeleteTicketAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Permanently deletes a ticket
    /// </summary>
    public async Task<bool> DeleteTicketPermanentlyAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/permanently";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Deletes multiple tickets
    /// </summary>
    public async Task<bool> DeleteTicketsAsync(List<int> ticketIds)
    {
        var url = $"{BaseUrl}/tickets/bulk/delete";
        var request = new BulkTicketIdsRequest { TicketIdList = ticketIds };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Permanently deletes multiple tickets
    /// </summary>
    public async Task<bool> DeleteTicketsPermanentlyAsync(List<int> ticketIds)
    {
        var url = $"{BaseUrl}/tickets/bulk/delete/permanently";
        var request = new BulkTicketIdsRequest { TicketIdList = ticketIds };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Restores a deleted ticket
    /// </summary>
    public async Task<bool> RestoreTicketAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/restore";
        var response = await HttpClient.PostAsync(url, null);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Restores multiple deleted tickets
    /// </summary>
    public async Task<bool> RestoreTicketsAsync(List<int> ticketIds)
    {
        var url = $"{BaseUrl}/tickets/bulk/restore";
        var request = new BulkTicketIdsRequest { TicketIdList = ticketIds };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Marks a ticket as spam
    /// </summary>
    public async Task<bool> MarkAsSpamAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/spam";
        var response = await HttpClient.PostAsync(url, null);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Removes a ticket from spam
    /// </summary>
    public async Task<bool> RemoveFromSpamAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/spam";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Archives a ticket
    /// </summary>
    public async Task<bool> ArchiveTicketAsync(int ticketId)
    {
        return await ArchiveTicketsAsync(new List<int> { ticketId });
    }

    /// <summary>
    /// Archives multiple tickets
    /// </summary>
    public async Task<bool> ArchiveTicketsAsync(List<int> ticketIds)
    {
        var url = $"{BaseUrl}/tickets/archive";
        var request = new BulkTicketIdsRequest { TicketIdList = ticketIds };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Locks a ticket
    /// </summary>
    public async Task<bool> LockTicketAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/lock";
        var response = await HttpClient.PostAsync(url, null);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Unlocks a ticket
    /// </summary>
    public async Task<bool> UnlockTicketAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/unlock";
        var response = await HttpClient.PostAsync(url, null);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Gets the date range from existing tickets
    /// </summary>
    public async Task<(DateTime? oldestDate, DateTime? newestDate)> GetTicketDateRangeAsync()
    {
        try
        {
            // Get oldest ticket
            var oldestParams = new TicketQueryParameters
            {
                Page = 1,
                PerPage = 1,
                OrderBy = "createdon asc"
            };

            // Get newest ticket
            var newestParams = new TicketQueryParameters
            {
                Page = 1,
                PerPage = 1,
                OrderBy = "createdon desc"
            };

            var oldestTask = GetTicketsAsync(oldestParams);
            var newestTask = GetTicketsAsync(newestParams);

            await Task.WhenAll(oldestTask, newestTask);

            var oldestResponse = await oldestTask;
            var newestResponse = await newestTask;

            var oldestDate = oldestResponse.Result.FirstOrDefault()?.CreatedOn;
            var newestDate = newestResponse.Result.FirstOrDefault()?.CreatedOn;

            return (oldestDate, newestDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not determine ticket date range: {ex.Message}");
            return (null, null);
        }
    }

    // Reply and Update Operations

    /// <summary>
    /// Replies to a ticket
    /// </summary>
    public async Task<Ticket> ReplyTicketAsync(int ticketId, ReplyTicketRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/reply";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<Ticket>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Gets ticket updates/messages
    /// </summary>
    public async Task<BoldDeskResponse<TicketMessage>> GetTicketMessagesAsync(int ticketId, TicketMessageQueryParameters? parameters = null)
    {
        parameters ??= new TicketMessageQueryParameters();
        var url = BuildTicketMessagesUrl(ticketId, parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<TicketMessage>>(url);
    }

    /// <summary>
    /// Gets a specific message
    /// </summary>
    public async Task<TicketMessage> GetTicketMessageAsync(int ticketId, int messageId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/messages/{messageId}";
        return await ExecuteRequestAsync<TicketMessage>(url);
    }

    /// <summary>
    /// Edits a ticket message
    /// </summary>
    public async Task<bool> EditMessageAsync(int ticketId, int messageId, EditMessageRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/messages/{messageId}";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PutAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Deletes a ticket message
    /// </summary>
    public async Task<bool> DeleteMessageAsync(int ticketId, int messageId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/messages/{messageId}";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Note Operations

    /// <summary>
    /// Gets ticket notes
    /// </summary>
    public async Task<BoldDeskResponse<TicketNote>> GetTicketNotesAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/notes";
        return await ExecuteRequestAsync<BoldDeskResponse<TicketNote>>(url);
    }

    /// <summary>
    /// Adds a note to a ticket
    /// </summary>
    public async Task<TicketNote> AddTicketNoteAsync(int ticketId, AddTicketNoteRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/notes";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<TicketNote>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    // Attachment Operations

    /// <summary>
    /// Gets ticket attachments
    /// </summary>
    public async Task<BoldDeskResponse<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/attachments";
        return await ExecuteRequestAsync<BoldDeskResponse<TicketAttachment>>(url);
    }

    /// <summary>
    /// Adds an attachment to a ticket
    /// </summary>
    public async Task<TicketAttachment> AddAttachmentAsync(int ticketId, Stream fileStream, string fileName, string? contentType = null)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/attachments";
        
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        }
        content.Add(streamContent, "uploadFiles", fileName);
        
        var response = await HttpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<TicketAttachment>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Deletes a ticket attachment
    /// </summary>
    public async Task<bool> DeleteAttachmentAsync(int ticketId, int attachmentId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/attachments/{attachmentId}";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Tag Operations

    /// <summary>
    /// Gets ticket tags
    /// </summary>
    public async Task<BoldDeskResponse<Tag>> GetTicketTagsAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/tags";
        return await ExecuteRequestAsync<BoldDeskResponse<Tag>>(url);
    }

    /// <summary>
    /// Adds tags to a ticket
    /// </summary>
    public async Task<bool> AddTagsAsync(int ticketId, List<string> tags)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/tags";
        var request = new AddTagsRequest { Tags = tags };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Removes tags from a ticket
    /// </summary>
    public async Task<bool> RemoveTagsAsync(int ticketId, List<string> tags)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/tags";
        var request = new RemoveTagsRequest { Tags = tags };
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(request, JsonOptions),
                Encoding.UTF8,
                "application/json")
        };
        
        var response = await HttpClient.SendAsync(httpRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Watcher Operations

    /// <summary>
    /// Gets ticket watchers
    /// </summary>
    public async Task<BoldDeskResponse<TicketWatcher>> GetWatchersAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/watchers";
        return await ExecuteRequestAsync<BoldDeskResponse<TicketWatcher>>(url);
    }

    /// <summary>
    /// Adds watchers to a ticket
    /// </summary>
    public async Task<bool> AddWatchersAsync(int ticketId, List<int> userIds)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/watchers";
        var request = new AddWatchersRequest { UserIds = userIds };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Removes watchers from a ticket
    /// </summary>
    public async Task<bool> RemoveWatchersAsync(int ticketId, List<int> userIds)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/watchers";
        var request = new RemoveWatchersRequest { UserIds = userIds };
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(request, JsonOptions),
                Encoding.UTF8,
                "application/json")
        };
        
        var response = await HttpClient.SendAsync(httpRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Relationship Operations

    /// <summary>
    /// Merges tickets
    /// </summary>
    public async Task<bool> MergeTicketsAsync(MergeTicketsRequest request)
    {
        var url = $"{BaseUrl}/tickets/merge";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Splits a ticket into a new ticket
    /// </summary>
    public async Task<Ticket> SplitTicketAsync(int ticketId, SplitTicketRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/split";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<Ticket>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Links tickets together
    /// </summary>
    public async Task<bool> LinkTicketsAsync(int ticketId, LinkTicketsRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/links";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Gets linked tickets
    /// </summary>
    public async Task<BoldDeskResponse<LinkedTicket>> GetLinkedTicketsAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/links";
        return await ExecuteRequestAsync<BoldDeskResponse<LinkedTicket>>(url);
    }

    /// <summary>
    /// Removes a ticket link
    /// </summary>
    public async Task<bool> RemoveTicketLinkAsync(int ticketId, int linkId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/links/{linkId}";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Sharing Operations

    /// <summary>
    /// Shares a ticket
    /// </summary>
    public async Task<bool> ShareTicketAsync(int ticketId, ShareTicketRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/share";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Gets ticket sharing details
    /// </summary>
    public async Task<TicketShare> GetTicketShareAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/share";
        return await ExecuteRequestAsync<TicketShare>(url);
    }

    /// <summary>
    /// Removes ticket sharing
    /// </summary>
    public async Task<bool> RemoveTicketShareAsync(int ticketId, int shareId, bool isAgent)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/share";
        var request = new { isAgent, id = shareId };
        var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(request, JsonOptions),
                Encoding.UTF8,
                "application/json")
        };
        
        var response = await HttpClient.SendAsync(httpRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Metadata Operations

    /// <summary>
    /// Gets ticket priorities
    /// </summary>
    public async Task<BoldDeskResponse<Priority>> GetPrioritiesAsync()
    {
        var url = $"{BaseUrl}/tickets/priorities";
        return await ExecuteRequestAsync<BoldDeskResponse<Priority>>(url);
    }

    /// <summary>
    /// Gets ticket statuses
    /// </summary>
    public async Task<BoldDeskResponse<Status>> GetStatusesAsync()
    {
        var url = $"{BaseUrl}/tickets/statuses";
        return await ExecuteRequestAsync<BoldDeskResponse<Status>>(url);
    }

    /// <summary>
    /// Gets ticket sources
    /// </summary>
    public async Task<BoldDeskResponse<TicketSource>> GetSourcesAsync()
    {
        var url = $"{BaseUrl}/tickets/sources";
        return await ExecuteRequestAsync<BoldDeskResponse<TicketSource>>(url);
    }

    /// <summary>
    /// Gets ticket fields
    /// </summary>
    public async Task<BoldDeskResponse<TicketField>> GetFieldsAsync()
    {
        var url = $"{BaseUrl}/tickets/fields";
        return await ExecuteRequestAsync<BoldDeskResponse<TicketField>>(url);
    }

    /// <summary>
    /// Gets ticket forms
    /// </summary>
    public async Task<BoldDeskResponse<TicketForm>> GetFormsAsync()
    {
        var url = $"{BaseUrl}/tickets/forms";
        return await ExecuteRequestAsync<BoldDeskResponse<TicketForm>>(url);
    }

    /// <summary>
    /// Gets a specific ticket form
    /// </summary>
    public async Task<TicketForm> GetFormAsync(int formId)
    {
        var url = $"{BaseUrl}/tickets/forms/{formId}";
        return await ExecuteRequestAsync<TicketForm>(url);
    }

    // History Operations

    /// <summary>
    /// Gets ticket history
    /// </summary>
    public async Task<BoldDeskResponse<TicketHistory>> GetTicketHistoryAsync(int ticketId, TicketHistoryQueryParameters? parameters = null)
    {
        parameters ??= new TicketHistoryQueryParameters();
        var url = BuildTicketHistoryUrl(ticketId, parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<TicketHistory>>(url);
    }

    /// <summary>
    /// Gets filtered ticket histories
    /// </summary>
    public async Task<BoldDeskResponse<TicketHistory>> GetTicketHistoriesAsync(TicketHistoryQueryParameters? parameters = null)
    {
        parameters ??= new TicketHistoryQueryParameters();
        var url = BuildTicketHistoriesUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<TicketHistory>>(url);
    }

    // Special Lists

    /// <summary>
    /// Gets deleted tickets
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> GetDeletedTicketsAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        var url = BuildTicketsUrl(parameters, "deleted");
        return await ExecuteRequestAsync<BoldDeskResponse<Ticket>>(url);
    }

    /// <summary>
    /// Gets spam tickets
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> GetSpamTicketsAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        var url = BuildTicketsUrl(parameters, "spam");
        return await ExecuteRequestAsync<BoldDeskResponse<Ticket>>(url);
    }

    /// <summary>
    /// Gets archived tickets
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> GetArchivedTicketsAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        var url = BuildTicketsUrl(parameters, "archived");
        return await ExecuteRequestAsync<BoldDeskResponse<Ticket>>(url);
    }

    // Additional Operations

    /// <summary>
    /// Downloads a ticket attachment
    /// </summary>
    public async Task<Stream> DownloadAttachmentAsync(int ticketId, int attachmentId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/attachments/{attachmentId}/download";
        var response = await HttpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return await response.Content.ReadAsStreamAsync();
    }

    /// <summary>
    /// Converts link type between tickets
    /// </summary>
    public async Task<bool> ConvertLinkTypeAsync(int ticketId, int linkId, string newLinkType)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/links/{linkId}/convert";
        var request = new ConvertLinkTypeRequest { NewLinkType = newLinkType };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PutAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Gets count of linked tickets
    /// </summary>
    public async Task<int> GetLinkedTicketsCountAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/links/count";
        var response = await HttpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, int>>(content, JsonOptions);
        return result?["count"] ?? 0;
    }

    /// <summary>
    /// Gets public messages for a ticket
    /// </summary>
    public async Task<BoldDeskResponse<TicketMessage>> GetPublicMessagesAsync(int ticketId, TicketMessageQueryParameters? parameters = null)
    {
        parameters ??= new TicketMessageQueryParameters();
        var url = BuildPublicMessagesUrl(ticketId, parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<TicketMessage>>(url);
    }

    /// <summary>
    /// Updates message tag
    /// </summary>
    public async Task<bool> UpdateMessageTagAsync(int ticketId, int messageId, string tagName)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/messages/{messageId}/tag";
        var request = new UpdateMessageTagRequest { TagName = tagName };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PutAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Article Links

    /// <summary>
    /// Gets article links for a ticket
    /// </summary>
    public async Task<BoldDeskResponse<TicketArticleLink>> GetArticleLinksAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/articlelinks";
        return await ExecuteRequestAsync<BoldDeskResponse<TicketArticleLink>>(url);
    }

    /// <summary>
    /// Adds an article link to a ticket
    /// </summary>
    public async Task<TicketArticleLink> AddArticleLinkAsync(int ticketId, int articleId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/articlelinks";
        var request = new AddArticleLinkRequest { ArticleId = articleId };
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<TicketArticleLink>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Removes an article link from a ticket
    /// </summary>
    public async Task<bool> RemoveArticleLinkAsync(int ticketId, int linkId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/articlelinks/{linkId}";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Related Contacts

    /// <summary>
    /// Gets related contacts for a ticket
    /// </summary>
    public async Task<BoldDeskResponse<RelatedContact>> GetRelatedContactsAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/relatedcontacts";
        return await ExecuteRequestAsync<BoldDeskResponse<RelatedContact>>(url);
    }

    /// <summary>
    /// Adds related contacts to a ticket
    /// </summary>
    public async Task<bool> AddRelatedContactsAsync(int ticketId, AddRelatedContactRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/relatedcontacts";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Removes a related contact from a ticket
    /// </summary>
    public async Task<bool> RemoveRelatedContactAsync(int ticketId, int contactId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/relatedcontacts/{contactId}";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Logs and Recovery

    /// <summary>
    /// Gets email delivery logs for a ticket
    /// </summary>
    public async Task<BoldDeskResponse<EmailDeliveryLog>> GetEmailDeliveryLogsAsync(int ticketId, EmailDeliveryLogQueryParameters? parameters = null)
    {
        parameters ??= new EmailDeliveryLogQueryParameters();
        var url = BuildEmailDeliveryLogsUrl(ticketId, parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<EmailDeliveryLog>>(url);
    }

    /// <summary>
    /// Gets permanent delete logs
    /// </summary>
    public async Task<BoldDeskResponse<PermanentDeleteLog>> GetPermanentDeleteLogsAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        var url = BuildPermanentDeleteLogsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<PermanentDeleteLog>>(url);
    }

    /// <summary>
    /// Recovers a suspended email as a ticket
    /// </summary>
    public async Task<Ticket> RecoverSuspendedEmailAsync(RecoverSuspendedEmailRequest request)
    {
        var url = $"{BaseUrl}/tickets/recover/suspendedemail";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<Ticket>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    // Updates and Metrics

    /// <summary>
    /// Gets ticket updates list
    /// </summary>
    public async Task<BoldDeskResponse<TicketUpdate>> GetTicketUpdatesAsync(int ticketId, TicketUpdatesQueryParameters? parameters = null)
    {
        parameters ??= new TicketUpdatesQueryParameters();
        var url = BuildTicketUpdatesUrl(ticketId, parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<TicketUpdate>>(url);
    }

    /// <summary>
    /// Gets all ticket updates across tickets
    /// </summary>
    public async Task<BoldDeskResponse<TicketUpdate>> GetAllTicketUpdatesAsync(TicketUpdatesQueryParameters? parameters = null)
    {
        parameters ??= new TicketUpdatesQueryParameters();
        var url = BuildAllTicketUpdatesUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<TicketUpdate>>(url);
    }

    /// <summary>
    /// Gets ticket metrics list
    /// </summary>
    public async Task<BoldDeskResponse<TicketMetrics>> GetTicketMetricsListAsync(TicketMetricsQueryParameters? parameters = null)
    {
        parameters ??= new TicketMetricsQueryParameters();
        var url = BuildTicketMetricsUrl(parameters);
        return await ExecuteRequestAsync<BoldDeskResponse<TicketMetrics>>(url);
    }

    /// <summary>
    /// Gets metrics for a specific ticket
    /// </summary>
    public async Task<TicketMetrics> GetTicketMetricsAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/metrics";
        return await ExecuteRequestAsync<TicketMetrics>(url);
    }

    // Web Links

    /// <summary>
    /// Gets web links for a ticket
    /// </summary>
    public async Task<BoldDeskResponse<TicketWebLink>> GetWebLinksAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/weblinks";
        return await ExecuteRequestAsync<BoldDeskResponse<TicketWebLink>>(url);
    }

    /// <summary>
    /// Adds a web link to a ticket
    /// </summary>
    public async Task<TicketWebLink> AddWebLinkAsync(int ticketId, AddWebLinkRequest request)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/weblinks";
        var content = new StringContent(
            JsonSerializer.Serialize(request, JsonOptions),
            Encoding.UTF8,
            "application/json");
        
        var response = await HttpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return JsonSerializer.Deserialize<TicketWebLink>(responseContent, JsonOptions) 
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Removes a web link from a ticket
    /// </summary>
    public async Task<bool> RemoveWebLinkAsync(int ticketId, int linkId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/weblinks/{linkId}";
        var response = await HttpClient.DeleteAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
        }
        
        ParseRateLimitHeaders(response.Headers);
        return response.IsSuccessStatusCode;
    }

    // Search and Stats

    /// <summary>
    /// Searches tickets for linking
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> SearchTicketsForLinkingAsync(SearchTicketForLinkingRequest request)
    {
        var url = BuildSearchForLinkingUrl(request);
        return await ExecuteRequestAsync<BoldDeskResponse<Ticket>>(url);
    }

    /// <summary>
    /// Gets public messages min max statistics
    /// </summary>
    public async Task<PublicMessagesStats> GetPublicMessagesStatsAsync(int ticketId)
    {
        var url = $"{BaseUrl}/tickets/{ticketId}/publicmessages/stats";
        return await ExecuteRequestAsync<PublicMessagesStats>(url);
    }

    // Filtered Special Lists

    /// <summary>
    /// Gets deleted tickets with advanced filters
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> GetDeletedTicketsFilteredAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        var url = BuildTicketsUrl(parameters, "filters/deleted");
        return await ExecuteRequestAsync<BoldDeskResponse<Ticket>>(url);
    }

    /// <summary>
    /// Gets spam tickets with advanced filters
    /// </summary>
    public async Task<BoldDeskResponse<Ticket>> GetSpamTicketsFilteredAsync(TicketQueryParameters? parameters = null)
    {
        parameters ??= new TicketQueryParameters();
        var url = BuildTicketsUrl(parameters, "filters/spam");
        return await ExecuteRequestAsync<BoldDeskResponse<Ticket>>(url);
    }

    // Helper methods

    private string BuildTicketsUrl(TicketQueryParameters parameters, string? suffix = null)
    {
        var baseUrl = suffix != null ? $"{BaseUrl}/tickets/{suffix}" : $"{BaseUrl}/tickets";
        var uriBuilder = new UriBuilder(baseUrl);
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString(); // Enforce API limit
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.Q))
        {
            query["q"] = parameters.Q;
        }

        if (!string.IsNullOrWhiteSpace(parameters.FilterId))
        {
            query["filterId"] = parameters.FilterId;
        }

        if (parameters.Fields != null && parameters.Fields.Count > 0)
        {
            query["fields"] = string.Join(",", parameters.Fields);
        }

        if (parameters.BrandIds != null && parameters.BrandIds.Count > 0)
        {
            query["brandIds"] = string.Join(",", parameters.BrandIds);
        }

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["orderBy"] = parameters.OrderBy;
        }
#pragma warning disable CS0618 // Type or member is obsolete
        else if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            // Support legacy SortBy for backward compatibility
            query["orderBy"] = parameters.SortBy;
        }
#pragma warning restore CS0618

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildTicketMessagesUrl(int ticketId, TicketMessageQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/{ticketId}/messages");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["orderBy"] = parameters.OrderBy;
        }

        if (parameters.MessageTypeIds != null && parameters.MessageTypeIds.Count > 0)
        {
            query["messageTypeIds"] = string.Join(",", parameters.MessageTypeIds);
        }

        if (parameters.MessageTagIds != null && parameters.MessageTagIds.Count > 0)
        {
            query["messageTagIds"] = string.Join(",", parameters.MessageTagIds);
        }

        if (parameters.IsFirstUpdateRequired.HasValue)
        {
            query["isFirstUpdateRequired"] = parameters.IsFirstUpdateRequired.Value.ToString().ToLower();
        }

        if (parameters.AttachmentsCount.HasValue)
        {
            query["attachmentsCount"] = parameters.AttachmentsCount.Value.ToString();
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildTicketHistoryUrl(int ticketId, TicketHistoryQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/{ticketId}/histories");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["orderBy"] = parameters.OrderBy;
        }

        if (parameters.UpdatedFromDate.HasValue)
        {
            query["updatedFromDate"] = parameters.UpdatedFromDate.Value.ToString("yyyy-MM-dd");
        }

        if (parameters.UpdatedToDate.HasValue)
        {
            query["updatedToDate"] = parameters.UpdatedToDate.Value.ToString("yyyy-MM-dd");
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildTicketHistoriesUrl(TicketHistoryQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/histories");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["orderBy"] = parameters.OrderBy;
        }

        if (parameters.UpdatedFromDate.HasValue)
        {
            query["updatedFromDate"] = parameters.UpdatedFromDate.Value.ToString("yyyy-MM-dd");
        }

        if (parameters.UpdatedToDate.HasValue)
        {
            query["updatedToDate"] = parameters.UpdatedToDate.Value.ToString("yyyy-MM-dd");
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildPublicMessagesUrl(int ticketId, TicketMessageQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/{ticketId}/publicmessages");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["orderBy"] = parameters.OrderBy;
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildEmailDeliveryLogsUrl(int ticketId, EmailDeliveryLogQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/{ticketId}/emaildeliverylogs");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.Status))
        {
            query["status"] = parameters.Status;
        }

        if (parameters.FromDate.HasValue)
        {
            query["fromDate"] = parameters.FromDate.Value.ToString("yyyy-MM-dd");
        }

        if (parameters.ToDate.HasValue)
        {
            query["toDate"] = parameters.ToDate.Value.ToString("yyyy-MM-dd");
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildPermanentDeleteLogsUrl(TicketQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/permanentdeletelogs");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.Q))
        {
            query["q"] = parameters.Q;
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildTicketUpdatesUrl(int ticketId, TicketUpdatesQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/{ticketId}/updates");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["orderBy"] = parameters.OrderBy;
        }

        if (parameters.IsPublic.HasValue)
        {
            query["isPublic"] = parameters.IsPublic.Value.ToString().ToLower();
        }

        if (parameters.FromDate.HasValue)
        {
            query["fromDate"] = parameters.FromDate.Value.ToString("yyyy-MM-dd");
        }

        if (parameters.ToDate.HasValue)
        {
            query["toDate"] = parameters.ToDate.Value.ToString("yyyy-MM-dd");
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildAllTicketUpdatesUrl(TicketUpdatesQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/updates");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            query["orderBy"] = parameters.OrderBy;
        }

        if (parameters.IsPublic.HasValue)
        {
            query["isPublic"] = parameters.IsPublic.Value.ToString().ToLower();
        }

        if (parameters.FromDate.HasValue)
        {
            query["fromDate"] = parameters.FromDate.Value.ToString("yyyy-MM-dd");
        }

        if (parameters.ToDate.HasValue)
        {
            query["toDate"] = parameters.ToDate.Value.ToString("yyyy-MM-dd");
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildTicketMetricsUrl(TicketMetricsQueryParameters parameters)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/metrics");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["page"] = parameters.Page.ToString();
        query["perPage"] = Math.Min(parameters.PerPage, 100).ToString();
        query["requiresCounts"] = parameters.RequiresCounts.ToString().ToLower();

        if (parameters.FromDate.HasValue)
        {
            query["fromDate"] = parameters.FromDate.Value.ToString("yyyy-MM-dd");
        }

        if (parameters.ToDate.HasValue)
        {
            query["toDate"] = parameters.ToDate.Value.ToString("yyyy-MM-dd");
        }

        if (parameters.TicketIds != null && parameters.TicketIds.Count > 0)
        {
            query["ticketIds"] = string.Join(",", parameters.TicketIds);
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }

    private string BuildSearchForLinkingUrl(SearchTicketForLinkingRequest request)
    {
        var uriBuilder = new UriBuilder($"{BaseUrl}/tickets/search/forlinking");
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["searchText"] = request.SearchText;

        if (request.ExcludeTicketId.HasValue)
        {
            query["excludeTicketId"] = request.ExcludeTicketId.Value.ToString();
        }

        if (request.Limit.HasValue)
        {
            query["limit"] = request.Limit.Value.ToString();
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}

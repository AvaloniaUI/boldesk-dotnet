using BoldDesk.Models;

namespace BoldDesk.Services;

/// <summary>
/// Service for managing BoldDesk tickets
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Fetches a single page of tickets from the BoldDesk API
    /// </summary>
    Task<BoldDeskResponse<Ticket>> GetTicketsAsync(TicketQueryParameters? parameters = null);

    /// <summary>
    /// Fetches all tickets using pagination, respecting rate limits
    /// </summary>
    IAsyncEnumerable<Ticket> GetAllTicketsAsync(TicketQueryParameters? parameters = null, IProgress<string>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a count of tickets matching the query without fetching all data
    /// </summary>
    Task<int> GetTicketCountAsync(TicketQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets the date range from existing tickets
    /// </summary>
    Task<(DateTime? oldestDate, DateTime? newestDate)> GetTicketDateRangeAsync();

    /// <summary>
    /// Gets a specific ticket by ID
    /// </summary>
    Task<Ticket> GetTicketAsync(int ticketId);
    
    /// <summary>
    /// Creates a new ticket
    /// </summary>
    Task<Ticket> CreateTicketAsync(CreateTicketRequest request);
    
    /// <summary>
    /// Updates an existing ticket
    /// </summary>
    Task<Ticket> UpdateTicketAsync(int ticketId, UpdateTicketRequest request);
    
    /// <summary>
    /// Deletes a ticket (moves to trash)
    /// </summary>
    Task<bool> DeleteTicketAsync(int ticketId);
    
    /// <summary>
    /// Permanently deletes a ticket
    /// </summary>
    Task<bool> DeleteTicketPermanentlyAsync(int ticketId);
    
    /// <summary>
    /// Deletes multiple tickets
    /// </summary>
    Task<bool> DeleteTicketsAsync(List<int> ticketIds);
    
    /// <summary>
    /// Permanently deletes multiple tickets
    /// </summary>
    Task<bool> DeleteTicketsPermanentlyAsync(List<int> ticketIds);
    
    /// <summary>
    /// Restores a deleted ticket
    /// </summary>
    Task<bool> RestoreTicketAsync(int ticketId);
    
    /// <summary>
    /// Restores multiple deleted tickets
    /// </summary>
    Task<bool> RestoreTicketsAsync(List<int> ticketIds);
    
    /// <summary>
    /// Marks a ticket as spam
    /// </summary>
    Task<bool> MarkAsSpamAsync(int ticketId);
    
    /// <summary>
    /// Removes a ticket from spam
    /// </summary>
    Task<bool> RemoveFromSpamAsync(int ticketId);
    
    /// <summary>
    /// Archives a ticket
    /// </summary>
    Task<bool> ArchiveTicketAsync(int ticketId);
    
    /// <summary>
    /// Archives multiple tickets
    /// </summary>
    Task<bool> ArchiveTicketsAsync(List<int> ticketIds);
    
    /// <summary>
    /// Locks a ticket
    /// </summary>
    Task<bool> LockTicketAsync(int ticketId);
    
    /// <summary>
    /// Unlocks a ticket
    /// </summary>
    Task<bool> UnlockTicketAsync(int ticketId);
    
    // Reply and Update Operations
    
    /// <summary>
    /// Replies to a ticket
    /// </summary>
    Task<Ticket> ReplyTicketAsync(int ticketId, ReplyTicketRequest request);
    
    /// <summary>
    /// Gets ticket updates/messages
    /// </summary>
    Task<BoldDeskResponse<TicketMessage>> GetTicketMessagesAsync(int ticketId, TicketMessageQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets a specific message
    /// </summary>
    Task<TicketMessage> GetTicketMessageAsync(int ticketId, int messageId);
    
    /// <summary>
    /// Edits a ticket message
    /// </summary>
    Task<bool> EditMessageAsync(int ticketId, int messageId, EditMessageRequest request);
    
    /// <summary>
    /// Deletes a ticket message
    /// </summary>
    Task<bool> DeleteMessageAsync(int ticketId, int messageId);
    
    // Note Operations
    
    /// <summary>
    /// Gets ticket notes
    /// </summary>
    Task<BoldDeskResponse<TicketNote>> GetTicketNotesAsync(int ticketId);
    
    /// <summary>
    /// Adds a note to a ticket
    /// </summary>
    Task<TicketNote> AddTicketNoteAsync(int ticketId, AddTicketNoteRequest request);
    
    // Attachment Operations
    
    /// <summary>
    /// Gets ticket attachments
    /// </summary>
    Task<BoldDeskResponse<TicketAttachment>> GetTicketAttachmentsAsync(int ticketId);
    
    /// <summary>
    /// Adds an attachment to a ticket
    /// </summary>
    Task<TicketAttachment> AddAttachmentAsync(int ticketId, Stream fileStream, string fileName, string? contentType = null);

    /// <summary>
    /// Uploads an attachment and returns a token that can be used when creating/replying to tickets
    /// </summary>
    Task<string> UploadAttachmentAsync(Stream fileStream, string fileName, string? contentType = null);
    
    /// <summary>
    /// Deletes a ticket attachment
    /// </summary>
    Task<bool> DeleteAttachmentAsync(int ticketId, int attachmentId);
    
    // Tag Operations
    
    /// <summary>
    /// Gets ticket tags
    /// </summary>
    Task<BoldDeskResponse<Tag>> GetTicketTagsAsync(int ticketId);
    
    /// <summary>
    /// Adds tags to a ticket
    /// </summary>
    Task<bool> AddTagsAsync(int ticketId, List<string> tags);
    
    /// <summary>
    /// Removes tags from a ticket
    /// </summary>
    Task<bool> RemoveTagsAsync(int ticketId, List<string> tags);
    
    // Watcher Operations
    
    /// <summary>
    /// Gets ticket watchers
    /// </summary>
    Task<BoldDeskResponse<TicketWatcher>> GetWatchersAsync(int ticketId);
    
    /// <summary>
    /// Adds watchers to a ticket
    /// </summary>
    Task<bool> AddWatchersAsync(int ticketId, List<int> userIds);
    
    /// <summary>
    /// Removes watchers from a ticket
    /// </summary>
    Task<bool> RemoveWatchersAsync(int ticketId, List<int> userIds);
    
    // Relationship Operations
    
    /// <summary>
    /// Merges tickets
    /// </summary>
    Task<bool> MergeTicketsAsync(MergeTicketsRequest request);
    
    /// <summary>
    /// Splits a ticket into a new ticket
    /// </summary>
    Task<Ticket> SplitTicketAsync(int ticketId, SplitTicketRequest request);
    
    /// <summary>
    /// Links tickets together
    /// </summary>
    Task<bool> LinkTicketsAsync(int ticketId, LinkTicketsRequest request);
    
    /// <summary>
    /// Gets linked tickets
    /// </summary>
    Task<BoldDeskResponse<LinkedTicket>> GetLinkedTicketsAsync(int ticketId);
    
    /// <summary>
    /// Removes a ticket link
    /// </summary>
    Task<bool> RemoveTicketLinkAsync(int ticketId, int linkId);
    
    // Sharing Operations
    
    /// <summary>
    /// Shares a ticket
    /// </summary>
    Task<bool> ShareTicketAsync(int ticketId, ShareTicketRequest request);
    
    /// <summary>
    /// Gets ticket sharing details
    /// </summary>
    Task<TicketShare> GetTicketShareAsync(int ticketId);
    
    /// <summary>
    /// Removes ticket sharing
    /// </summary>
    Task<bool> RemoveTicketShareAsync(int ticketId, int shareId, bool isAgent);
    
    // Metadata Operations
    
    /// <summary>
    /// Gets ticket priorities
    /// </summary>
    Task<BoldDeskResponse<Priority>> GetPrioritiesAsync();
    
    /// <summary>
    /// Gets ticket statuses
    /// </summary>
    Task<BoldDeskResponse<Status>> GetStatusesAsync();
    
    /// <summary>
    /// Gets ticket sources
    /// </summary>
    Task<BoldDeskResponse<TicketSource>> GetSourcesAsync();
    
    /// <summary>
    /// Gets ticket fields
    /// </summary>
    Task<BoldDeskResponse<TicketField>> GetFieldsAsync();
    
    /// <summary>
    /// Gets ticket forms
    /// </summary>
    Task<BoldDeskResponse<TicketForm>> GetFormsAsync();
    
    /// <summary>
    /// Gets a specific ticket form
    /// </summary>
    Task<TicketForm> GetFormAsync(int formId);
    
    // History Operations
    
    /// <summary>
    /// Gets ticket history
    /// </summary>
    Task<BoldDeskResponse<TicketHistory>> GetTicketHistoryAsync(int ticketId, TicketHistoryQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets filtered ticket histories
    /// </summary>
    Task<BoldDeskResponse<TicketHistory>> GetTicketHistoriesAsync(TicketHistoryQueryParameters? parameters = null);
    
    // Special Lists
    
    /// <summary>
    /// Gets deleted tickets
    /// </summary>
    Task<BoldDeskResponse<Ticket>> GetDeletedTicketsAsync(TicketQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets spam tickets
    /// </summary>
    Task<BoldDeskResponse<Ticket>> GetSpamTicketsAsync(TicketQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets archived tickets
    /// </summary>
    Task<BoldDeskResponse<Ticket>> GetArchivedTicketsAsync(TicketQueryParameters? parameters = null);
    
    // Additional Operations
    
    /// <summary>
    /// Downloads a ticket attachment
    /// </summary>
    Task<Stream> DownloadAttachmentAsync(int ticketId, int attachmentId);
    
    /// <summary>
    /// Converts link type between tickets
    /// </summary>
    Task<bool> ConvertLinkTypeAsync(int ticketId, int linkId, string newLinkType);
    
    /// <summary>
    /// Gets count of linked tickets
    /// </summary>
    Task<int> GetLinkedTicketsCountAsync(int ticketId);
    
    /// <summary>
    /// Gets public messages for a ticket
    /// </summary>
    Task<BoldDeskResponse<TicketMessage>> GetPublicMessagesAsync(int ticketId, TicketMessageQueryParameters? parameters = null);
    
    /// <summary>
    /// Updates message tag
    /// </summary>
    Task<bool> UpdateMessageTagAsync(int ticketId, int messageId, string tagName);
    
    // Article Links
    
    /// <summary>
    /// Gets article links for a ticket
    /// </summary>
    Task<BoldDeskResponse<TicketArticleLink>> GetArticleLinksAsync(int ticketId);
    
    /// <summary>
    /// Adds an article link to a ticket
    /// </summary>
    Task<TicketArticleLink> AddArticleLinkAsync(int ticketId, int articleId);
    
    /// <summary>
    /// Removes an article link from a ticket
    /// </summary>
    Task<bool> RemoveArticleLinkAsync(int ticketId, int linkId);
    
    // Related Contacts
    
    /// <summary>
    /// Gets related contacts for a ticket
    /// </summary>
    Task<BoldDeskResponse<RelatedContact>> GetRelatedContactsAsync(int ticketId);
    
    /// <summary>
    /// Adds related contacts to a ticket
    /// </summary>
    Task<bool> AddRelatedContactsAsync(int ticketId, AddRelatedContactRequest request);
    
    /// <summary>
    /// Removes a related contact from a ticket
    /// </summary>
    Task<bool> RemoveRelatedContactAsync(int ticketId, int contactId);
    
    // Logs and Recovery
    
    /// <summary>
    /// Gets email delivery logs for a ticket
    /// </summary>
    Task<BoldDeskResponse<EmailDeliveryLog>> GetEmailDeliveryLogsAsync(int ticketId, EmailDeliveryLogQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets permanent delete logs
    /// </summary>
    Task<BoldDeskResponse<PermanentDeleteLog>> GetPermanentDeleteLogsAsync(TicketQueryParameters? parameters = null);
    
    /// <summary>
    /// Recovers a suspended email as a ticket
    /// </summary>
    Task<Ticket> RecoverSuspendedEmailAsync(RecoverSuspendedEmailRequest request);
    
    // Updates and Metrics
    
    /// <summary>
    /// Gets ticket updates list
    /// </summary>
    Task<BoldDeskResponse<TicketUpdate>> GetTicketUpdatesAsync(int ticketId, TicketUpdatesQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets all ticket updates across tickets
    /// </summary>
    Task<BoldDeskResponse<TicketUpdate>> GetAllTicketUpdatesAsync(TicketUpdatesQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets ticket metrics list
    /// </summary>
    Task<BoldDeskResponse<TicketMetrics>> GetTicketMetricsListAsync(TicketMetricsQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets metrics for a specific ticket
    /// </summary>
    Task<TicketMetrics> GetTicketMetricsAsync(int ticketId);
    
    // Web Links
    
    /// <summary>
    /// Gets web links for a ticket
    /// </summary>
    Task<BoldDeskResponse<TicketWebLink>> GetWebLinksAsync(int ticketId);
    
    /// <summary>
    /// Adds a web link to a ticket
    /// </summary>
    Task<TicketWebLink> AddWebLinkAsync(int ticketId, AddWebLinkRequest request);
    
    /// <summary>
    /// Removes a web link from a ticket
    /// </summary>
    Task<bool> RemoveWebLinkAsync(int ticketId, int linkId);
    
    // Search and Stats
    
    /// <summary>
    /// Searches tickets for linking
    /// </summary>
    Task<BoldDeskResponse<Ticket>> SearchTicketsForLinkingAsync(SearchTicketForLinkingRequest request);
    
    /// <summary>
    /// Gets public messages min max statistics
    /// </summary>
    Task<PublicMessagesStats> GetPublicMessagesStatsAsync(int ticketId);
    
    // Filtered Special Lists
    
    /// <summary>
    /// Gets deleted tickets with advanced filters
    /// </summary>
    Task<BoldDeskResponse<Ticket>> GetDeletedTicketsFilteredAsync(TicketQueryParameters? parameters = null);
    
    /// <summary>
    /// Gets spam tickets with advanced filters
    /// </summary>
    Task<BoldDeskResponse<Ticket>> GetSpamTicketsFilteredAsync(TicketQueryParameters? parameters = null);
}

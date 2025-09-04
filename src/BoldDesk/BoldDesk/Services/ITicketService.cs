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
    
    // Future CRUD operations
    // Task<Ticket> GetTicketAsync(int ticketId);
    // Task<Ticket> CreateTicketAsync(CreateTicketRequest request);
    // Task<Ticket> UpdateTicketAsync(int ticketId, UpdateTicketRequest request);
    // Task<bool> DeleteTicketAsync(int ticketId);
}
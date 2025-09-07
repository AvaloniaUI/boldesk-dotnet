using BoldDesk;
using BoldDesk.Models;
using BoldDesk.QueryBuilder;

namespace BoldDesk.Cli;

public class TicketsExample
{
    private readonly IBoldDeskClient _client;

    public TicketsExample(IBoldDeskClient client)
    {
        _client = client;
    }

    public async Task RunExamplesAsync()
    {
        Console.WriteLine("\n=== BoldDesk Tickets API Examples ===\n");

        // Example 1: List tickets with basic pagination
        await ListTicketsBasic();

        // Example 2: Search tickets with query builder
        await SearchTicketsWithQueryBuilder();

        // Example 3: Get a specific ticket
        await GetTicketDetails();

        // Example 4: Create a new ticket
        await CreateTicket();

        // Example 5: Update a ticket
        await UpdateTicket();

        // Example 6: Reply to a ticket
        await ReplyToTicket();

        // Example 7: Add a note to a ticket
        await AddTicketNote();

        // Example 8: Manage ticket tags
        await ManageTicketTags();

        // Example 9: Get ticket metadata
        await GetTicketMetadata();

        // Example 10: Work with ticket history
        await GetTicketHistory();
    }

    private async Task ListTicketsBasic()
    {
        Console.WriteLine("1. Listing tickets with basic pagination:");
        
        var parameters = new TicketQueryParameters
        {
            Page = 1,
            PerPage = 10,
            OrderBy = "createdon desc",
            RequiresCounts = true
        };

        var tickets = await _client.Tickets.GetTicketsAsync(parameters);
        
        Console.WriteLine($"   Total tickets: {tickets.Count}");
        Console.WriteLine($"   Fetched: {tickets.Result.Count}");
        
        foreach (var ticket in tickets.Result.Take(3))
        {
            Console.WriteLine($"   - [{ticket.TicketId}] {ticket.Title} (Status: {ticket.Status?.Description})");
        }
        Console.WriteLine();
    }

    private async Task SearchTicketsWithQueryBuilder()
    {
        Console.WriteLine("2. Searching tickets with query builder:");
        
        // Build a complex query
        var queryBuilder = new TicketQueryBuilder()
            .WithPriority(1, 2) // High and Medium priority
            .CreatedLast7Days() // Created in last 7 days
            .WithStatus(1, 2, 3); // Open statuses
        
        var queryParams = queryBuilder.WithOrderBy("createdon desc");
        queryParams = queryBuilder.WithPerPage(5);

        var tickets = await _client.Tickets.GetTicketsAsync(queryParams);
        
        Console.WriteLine($"   Found {tickets.Count} tickets matching criteria");
        foreach (var ticket in tickets.Result)
        {
            Console.WriteLine($"   - [{ticket.TicketId}] {ticket.Title}");
            Console.WriteLine($"     Priority: {ticket.Priority?.Description}, Created: {ticket.CreatedOn:yyyy-MM-dd}");
        }
        Console.WriteLine();
    }

    private async Task GetTicketDetails()
    {
        Console.WriteLine("3. Getting specific ticket details:");
        
        // First, get any ticket ID
        var tickets = await _client.Tickets.GetTicketsAsync(new TicketQueryParameters { PerPage = 1 });
        if (tickets.Result.Any())
        {
            var ticketId = tickets.Result.First().TicketId;
            var ticket = await _client.Tickets.GetTicketAsync(ticketId);
            
            Console.WriteLine($"   Ticket #{ticket.TicketId}: {ticket.Title}");
            Console.WriteLine($"   - Status: {ticket.Status?.Description}");
            Console.WriteLine($"   - Priority: {ticket.Priority?.Description}");
            Console.WriteLine($"   - Agent: {ticket.Agent?.Name ?? "Unassigned"}");
            Console.WriteLine($"   - Created: {ticket.CreatedOn}");
            Console.WriteLine($"   - Last Updated: {ticket.LastUpdatedOn}");
        }
        else
        {
            Console.WriteLine("   No tickets found to display");
        }
        Console.WriteLine();
    }

    private async Task CreateTicket()
    {
        Console.WriteLine("4. Creating a new ticket (simulated):");
        
        var createRequest = new CreateTicketRequest
        {
            Title = "Test Ticket from API",
            Description = "This is a test ticket created via the BoldDesk .NET SDK",
            PriorityId = 2, // Medium priority
            CategoryId = 1,
            Tags = new List<string> { "api-test", "sdk" }
        };

        try
        {
            // Uncomment to actually create a ticket
            // var newTicket = await _client.Tickets.CreateTicketAsync(createRequest);
            // Console.WriteLine($"   Created ticket #{newTicket.TicketId}: {newTicket.Title}");
            
            Console.WriteLine("   [Simulated] Would create ticket with title: " + createRequest.Title);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Error creating ticket: {ex.Message}");
        }
        Console.WriteLine();
    }

    private async Task UpdateTicket()
    {
        Console.WriteLine("5. Updating a ticket (simulated):");
        
        var tickets = await _client.Tickets.GetTicketsAsync(new TicketQueryParameters { PerPage = 1 });
        if (tickets.Result.Any())
        {
            var ticketId = tickets.Result.First().TicketId;
            
            var updateRequest = new UpdateTicketRequest
            {
                Title = "Updated Title",
                PriorityId = 1, // High priority
                Tags = new List<string> { "updated", "high-priority" }
            };

            try
            {
                // Uncomment to actually update a ticket
                // var updatedTicket = await _client.Tickets.UpdateTicketAsync(ticketId, updateRequest);
                // Console.WriteLine($"   Updated ticket #{updatedTicket.TicketId}");
                
                Console.WriteLine($"   [Simulated] Would update ticket #{ticketId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error updating ticket: {ex.Message}");
            }
        }
        Console.WriteLine();
    }

    private async Task ReplyToTicket()
    {
        Console.WriteLine("6. Replying to a ticket (simulated):");
        
        var tickets = await _client.Tickets.GetTicketsAsync(new TicketQueryParameters { PerPage = 1 });
        if (tickets.Result.Any())
        {
            var ticketId = tickets.Result.First().TicketId;
            
            var replyRequest = new ReplyTicketRequest
            {
                Description = "Thank you for contacting support. We are looking into your issue.",
                IsPrivate = false
            };

            try
            {
                // Uncomment to actually reply to a ticket
                // var updatedTicket = await _client.Tickets.ReplyTicketAsync(ticketId, replyRequest);
                // Console.WriteLine($"   Replied to ticket #{updatedTicket.TicketId}");
                
                Console.WriteLine($"   [Simulated] Would reply to ticket #{ticketId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error replying to ticket: {ex.Message}");
            }
        }
        Console.WriteLine();
    }

    private async Task AddTicketNote()
    {
        Console.WriteLine("7. Adding a note to a ticket (simulated):");
        
        var tickets = await _client.Tickets.GetTicketsAsync(new TicketQueryParameters { PerPage = 1 });
        if (tickets.Result.Any())
        {
            var ticketId = tickets.Result.First().TicketId;
            
            var noteRequest = new AddTicketNoteRequest
            {
                Description = "Internal note: Customer has been contacted via phone.",
                IsPrivate = true
            };

            try
            {
                // Uncomment to actually add a note
                // var note = await _client.Tickets.AddTicketNoteAsync(ticketId, noteRequest);
                // Console.WriteLine($"   Added note to ticket #{ticketId}");
                
                Console.WriteLine($"   [Simulated] Would add note to ticket #{ticketId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error adding note: {ex.Message}");
            }
        }
        Console.WriteLine();
    }

    private async Task ManageTicketTags()
    {
        Console.WriteLine("8. Managing ticket tags:");
        
        var tickets = await _client.Tickets.GetTicketsAsync(new TicketQueryParameters { PerPage = 1 });
        if (tickets.Result.Any())
        {
            var ticketId = tickets.Result.First().TicketId;
            
            try
            {
                // Get current tags
                var tags = await _client.Tickets.GetTicketTagsAsync(ticketId);
                Console.WriteLine($"   Current tags for ticket #{ticketId}:");
                foreach (var tag in tags.Result)
                {
                    Console.WriteLine($"   - {tag.TagName}");
                }
                
                // Add tags (simulated)
                Console.WriteLine("   [Simulated] Would add tags: urgent, customer-issue");
                // await _client.Tickets.AddTagsAsync(ticketId, new List<string> { "urgent", "customer-issue" });
                
                // Remove tags (simulated)
                Console.WriteLine("   [Simulated] Would remove tag: resolved");
                // await _client.Tickets.RemoveTagsAsync(ticketId, new List<string> { "resolved" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error managing tags: {ex.Message}");
            }
        }
        Console.WriteLine();
    }

    private async Task GetTicketMetadata()
    {
        Console.WriteLine("9. Getting ticket metadata:");
        
        try
        {
            // Get priorities
            var priorities = await _client.Tickets.GetPrioritiesAsync();
            Console.WriteLine($"   Available priorities: {priorities.Result.Count}");
            foreach (var priority in priorities.Result.Take(3))
            {
                Console.WriteLine($"   - [{priority.Id}] {priority.Description}");
            }
            
            // Get statuses
            var statuses = await _client.Tickets.GetStatusesAsync();
            Console.WriteLine($"\n   Available statuses: {statuses.Result.Count}");
            foreach (var status in statuses.Result.Take(3))
            {
                Console.WriteLine($"   - [{status.Id}] {status.Description}");
            }
            
            // Get sources
            var sources = await _client.Tickets.GetSourcesAsync();
            Console.WriteLine($"\n   Available sources: {sources.Result.Count}");
            foreach (var source in sources.Result.Take(3))
            {
                Console.WriteLine($"   - [{source.SourceId}] {source.SourceName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Error getting metadata: {ex.Message}");
        }
        Console.WriteLine();
    }

    private async Task GetTicketHistory()
    {
        Console.WriteLine("10. Getting ticket history:");
        
        var tickets = await _client.Tickets.GetTicketsAsync(new TicketQueryParameters { PerPage = 1 });
        if (tickets.Result.Any())
        {
            var ticketId = tickets.Result.First().TicketId;
            
            try
            {
                var historyParams = new TicketHistoryQueryParameters
                {
                    Page = 1,
                    PerPage = 5,
                    OrderBy = "createdon desc"
                };
                
                var history = await _client.Tickets.GetTicketHistoryAsync(ticketId, historyParams);
                Console.WriteLine($"   History for ticket #{ticketId}:");
                foreach (var entry in history.Result)
                {
                    Console.WriteLine($"   - {entry.CreatedOn:yyyy-MM-dd HH:mm}: {entry.Action}");
                    if (!string.IsNullOrWhiteSpace(entry.Field))
                    {
                        Console.WriteLine($"     Field: {entry.Field}, Old: {entry.OldValue}, New: {entry.NewValue}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error getting history: {ex.Message}");
            }
        }
        Console.WriteLine();
    }

    // Additional example: Stream all tickets with progress reporting
    public async Task StreamAllTickets()
    {
        Console.WriteLine("Streaming all tickets with progress:");
        
        var progress = new Progress<string>(message => 
        {
            Console.WriteLine($"   Progress: {message}");
        });
        
        var query = new TicketQueryBuilder()
            .CreatedThisMonth()
            .ToParameters();
        
        var ticketCount = 0;
        await foreach (var ticket in _client.Tickets.GetAllTicketsAsync(query, progress))
        {
            ticketCount++;
            if (ticketCount <= 5)
            {
                Console.WriteLine($"   - [{ticket.TicketId}] {ticket.Title}");
            }
        }
        
        Console.WriteLine($"   Total tickets processed: {ticketCount}");
    }
}
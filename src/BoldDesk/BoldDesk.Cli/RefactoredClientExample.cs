using BoldDesk;
using BoldDesk.Exceptions;
using BoldDesk.Models;

namespace BoldDesk.Examples;

/// <summary>
/// Example demonstrating how to use the refactored BoldDesk client with sub-services
/// </summary>
public class RefactoredClientExample
{
    public static async Task RunExampleAsync(string domain, string apiKey)
    {
        using var client = new BoldDeskClient(domain, apiKey);
        
        try
        {
            // Example 1: Working with Brands
            Console.WriteLine("=== BRANDS ===");
            var brands = await client.Brands.GetBrandsAsync();
            Console.WriteLine($"Found {brands.Result.Count} brands");
            
            foreach (var brand in brands.Result)
            {
                Console.WriteLine($"  Brand #{brand.BrandId}: {brand.BrandName}");
            }
            
            // Example 2: Working with Tickets
            Console.WriteLine("\n=== TICKETS ===");
            var ticketParams = new TicketQueryParameters
            {
                Page = 1,
                PerPage = 10,
                RequiresCounts = true
            };
            
            var tickets = await client.Tickets.GetTicketsAsync(ticketParams);
            Console.WriteLine($"Found {tickets.Count} total tickets");
            Console.WriteLine($"Showing {tickets.Result.Count} tickets on page 1:");
            
            foreach (var ticket in tickets.Result.Take(5))
            {
                Console.WriteLine($"  Ticket #{ticket.TicketId}: {ticket.Title}");
                Console.WriteLine($"    Status: {ticket.Status?.Description}, Priority: {ticket.Priority?.Description}");
                Console.WriteLine($"    Brand: {ticket.Brand}");
            }
            
            // Example 3: Working with Worklogs
            Console.WriteLine("\n=== WORKLOGS ===");
            var worklogParams = new WorklogQueryParameters
            {
                Page = 1,
                PerPage = 5,
                RequiresCounts = true
            };
            
            var worklogs = await client.Worklogs.GetWorklogsAsync(worklogParams);
            Console.WriteLine($"Found {worklogs.Count} total worklogs");
            
            foreach (var worklog in worklogs.Result)
            {
                Console.WriteLine($"  Worklog #{worklog.WorklogId}: Ticket #{worklog.TicketId}");
                Console.WriteLine($"    Created by: {worklog.CreatedBy?.Name}, Time: {worklog.TimeSpent} mins");
                Console.WriteLine($"    Description: {worklog.Description}");
            }
            
            // Example 4: Getting all tickets with progress reporting
            Console.WriteLine("\n=== FETCHING ALL TICKETS ===");
            var progress = new Progress<string>(message => Console.WriteLine($"  Progress: {message}"));
            
            var allTickets = new List<Ticket>();
            await foreach (var ticket in client.Tickets.GetAllTicketsAsync(progress: progress))
            {
                allTickets.Add(ticket);
                
                // Stop after first 20 for demo purposes
                if (allTickets.Count >= 20)
                    break;
            }
            
            Console.WriteLine($"Fetched {allTickets.Count} tickets total");
            
            // Example 5: Getting ticket count without fetching all data
            Console.WriteLine("\n=== TICKET COUNT ===");
            var ticketCount = await client.Tickets.GetTicketCountAsync();
            Console.WriteLine($"Total tickets in system: {ticketCount}");
            
            // Check rate limit status
            if (client.LastRateLimitInfo != null)
            {
                Console.WriteLine($"\n=== RATE LIMIT STATUS ===");
                Console.WriteLine($"  Limit: {client.LastRateLimitInfo.Limit} calls/min");
                Console.WriteLine($"  Remaining: {client.LastRateLimitInfo.Remaining} calls");
                Console.WriteLine($"  Reset: {client.LastRateLimitInfo.Reset:yyyy-MM-dd HH:mm:ss UTC}");
            }
        }
        catch (BoldDeskAuthenticationException ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
        }
        catch (BoldDeskRateLimitException ex)
        {
            Console.WriteLine($"Rate limit exceeded: {ex.Message}");
            if (ex.GetWaitTime() is TimeSpan waitTime)
            {
                Console.WriteLine($"Wait {waitTime.TotalSeconds:F1} seconds before retrying.");
            }
        }
        catch (BoldDeskApiException ex)
        {
            Console.WriteLine($"API Error: {ex.Message}");
            Console.WriteLine($"Status Code: {ex.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
using System.Text.Json;
using BoldDesk.Models;
using BoldDesk.QueryBuilder;
using Spectre.Console;

namespace BoldDesk.Cli.Commands;

public static class TicketCommands
{
    // Escapes dynamic strings to avoid Spectre.Console markup parsing errors
    private static string E(string? value) => Markup.Escape(value ?? string.Empty);
    public static async Task<int> ExecuteAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return 0;
        }

        var command = args[0].ToLowerInvariant();
        var commandArgs = args.Skip(1).ToArray();

        return command switch
        {
            "list" => await ListTicketsAsync(commandArgs, client),
            "get" => await GetTicketAsync(commandArgs, client),
            "create" => await CreateTicketAsync(commandArgs, client),
            "update" => await UpdateTicketAsync(commandArgs, client),
            "delete" => await DeleteTicketAsync(commandArgs, client),
            "restore" => await RestoreTicketAsync(commandArgs, client),
            "reply" => await ReplyToTicketAsync(commandArgs, client),
            "archive" => await ArchiveTicketAsync(commandArgs, client),
            "spam" => await ManageSpamAsync(commandArgs, client),
            "lock" => await LockTicketAsync(commandArgs, client),
            "unlock" => await UnlockTicketAsync(commandArgs, client),
            
            // Messages
            "messages" => await ManageMessagesAsync(commandArgs, client),
            "notes" => await ManageNotesAsync(commandArgs, client),
            
            // Attachments
            "attachments" => await ManageAttachmentsAsync(commandArgs, client),
            
            // Tags & Watchers
            "tags" => await ManageTagsAsync(commandArgs, client),
            "watchers" => await ManageWatchersAsync(commandArgs, client),
            
            // Relationships
            "merge" => await MergeTicketsAsync(commandArgs, client),
            "split" => await SplitTicketAsync(commandArgs, client),
            "link" => await LinkTicketsAsync(commandArgs, client),
            "links" => await ManageLinksAsync(commandArgs, client),
            
            // Article & Web Links
            "articles" => await ManageArticleLinksAsync(commandArgs, client),
            "weblinks" => await ManageWebLinksAsync(commandArgs, client),
            
            // Related Contacts
            "contacts" => await ManageRelatedContactsAsync(commandArgs, client),
            
            // Sharing
            "share" => await ShareTicketAsync(commandArgs, client),
            
            // Metadata
            "priorities" => await GetPrioritiesAsync(commandArgs, client),
            "statuses" => await GetStatusesAsync(commandArgs, client),
            "sources" => await GetSourcesAsync(commandArgs, client),
            "fields" => await GetFieldsAsync(commandArgs, client),
            "forms" => await GetFormsAsync(commandArgs, client),
            
            // History & Logs
            "history" => await GetHistoryAsync(commandArgs, client),
            "logs" => await ManageLogsAsync(commandArgs, client),
            
            // Metrics
            "metrics" => await GetMetricsAsync(commandArgs, client),
            
            // Special
            "deleted" => await GetDeletedTicketsAsync(commandArgs, client),
            "archived" => await GetArchivedTicketsAsync(commandArgs, client),
            "spam-list" => await GetSpamTicketsAsync(commandArgs, client),
            "search-link" => await SearchForLinkingAsync(commandArgs, client),
            "recover-email" => await RecoverSuspendedEmailAsync(commandArgs, client),
            
            _ => ShowHelpAndReturn()
        };
    }

    private static int ShowHelpAndReturn()
    {
        ShowHelp();
        return 0;
    }

    private static void ShowHelp()
    {
        AnsiConsole.WriteLine("BoldDesk Ticket Commands");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Usage: bolddesk tickets <command> [options]");
        AnsiConsole.WriteLine();
        
        var table = new Table();
        table.AddColumn("Command");
        table.AddColumn("Description");
        
        // Core Operations
        table.AddRow("[cyan]Core Operations[/]", "");
        table.AddRow("  list", "List tickets with filters");
        table.AddRow("  get <id>", "Get ticket details");
        table.AddRow("  create", "Create a new ticket");
        table.AddRow("  update <id>", "Update ticket");
        table.AddRow("  delete <id>", "Delete ticket (soft)");
        table.AddRow("  restore <id>", "Restore deleted ticket");
        table.AddRow("  archive <id>", "Archive ticket");
        table.AddRow("  lock <id>", "Lock ticket");
        table.AddRow("  unlock <id>", "Unlock ticket");
        
        // Communication
        table.AddRow("[cyan]Communication[/]", "");
        table.AddRow("  reply <id>", "Reply to ticket");
        table.AddRow("  messages <id>", "Manage ticket messages");
        table.AddRow("  notes <id>", "Manage ticket notes");
        
        // Attachments & Links
        table.AddRow("[cyan]Attachments & Links[/]", "");
        table.AddRow("  attachments <id>", "Manage attachments");
        table.AddRow("  articles <id>", "Manage article links");
        table.AddRow("  weblinks <id>", "Manage web links");
        
        // Organization
        table.AddRow("[cyan]Organization[/]", "");
        table.AddRow("  tags <id>", "Manage tags");
        table.AddRow("  watchers <id>", "Manage watchers");
        table.AddRow("  contacts <id>", "Manage related contacts");
        table.AddRow("  share <id>", "Share ticket");
        
        // Relationships
        table.AddRow("[cyan]Relationships[/]", "");
        table.AddRow("  merge", "Merge tickets");
        table.AddRow("  split <id>", "Split ticket");
        table.AddRow("  link <id>", "Link tickets");
        table.AddRow("  links <id>", "View linked tickets");
        
        // Metadata
        table.AddRow("[cyan]Metadata[/]", "");
        table.AddRow("  priorities", "List priorities");
        table.AddRow("  statuses", "List statuses");
        table.AddRow("  sources", "List sources");
        table.AddRow("  fields", "List fields");
        table.AddRow("  forms", "List forms");
        
        // Analytics
        table.AddRow("[cyan]Analytics[/]", "");
        table.AddRow("  history <id>", "View ticket history");
        table.AddRow("  metrics <id>", "View ticket metrics");
        table.AddRow("  logs <type>", "View logs (email/delete)");
        
        // Special Lists
        table.AddRow("[cyan]Special Lists[/]", "");
        table.AddRow("  deleted", "List deleted tickets");
        table.AddRow("  archived", "List archived tickets");
        table.AddRow("  spam-list", "List spam tickets");
        
        // Advanced
        table.AddRow("[cyan]Advanced[/]", "");
        table.AddRow("  spam <id> <mark|unmark>", "Manage spam status");
        table.AddRow("  search-link", "Search tickets for linking");
        table.AddRow("  recover-email", "Recover suspended email");
        
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Use 'bolddesk tickets <command> --help' for command-specific help");
    }

    private static async Task<int> ListTicketsAsync(string[] args, IBoldDeskClient client)
    {
        var parameters = ParseTicketQueryParameters(args);
        var format = GetFormat(args);
        
        if (HasFlag(args, "--all"))
        {
            var tickets = new List<Ticket>();
            await AnsiConsole.Status()
                .StartAsync("Fetching all tickets...", async ctx =>
                {
                    await foreach (var ticket in client.Tickets.GetAllTicketsAsync(parameters))
                    {
                        tickets.Add(ticket);
                        ctx.Status($"Fetched {tickets.Count} tickets...");
                    }
                });
            
            OutputTickets(tickets, format);
        }
        else
        {
            var response = await client.Tickets.GetTicketsAsync(parameters);
            OutputTicketResponse(response, format);
        }
        
        return 0;
    }

    private static async Task<int> GetTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID required[/]");
            return 1;
        }
        
        if (!int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid ticket ID[/]");
            return 1;
        }
        
        var ticket = await client.Tickets.GetTicketAsync(ticketId);
        var format = GetFormat(args);
        
        if (format == "json")
        {
            AnsiConsole.WriteLine(JsonSerializer.Serialize(ticket, new JsonSerializerOptions { WriteIndented = true }));
        }
        else
        {
            var table = new Table();
            table.AddColumn("Field");
            table.AddColumn("Value");
            
            table.AddRow("ID", ticket.TicketId.ToString());
            table.AddRow("Title", E(ticket.Title));
            table.AddRow("Status", E(ticket.Status?.Description));
            table.AddRow("Priority", E(ticket.Priority?.Description));
            table.AddRow("Agent", string.IsNullOrEmpty(ticket.Agent?.Name) ? "Unassigned" : E(ticket.Agent?.Name));
            var updated = ticket.LastUpdatedOn ?? ticket.LastStatusChangedOn ?? ticket.LastRepliedOn ?? ticket.CreatedOn;
            table.AddRow("Created", ticket.CreatedOn.ToString("yyyy-MM-dd HH:mm"));
            table.AddRow("Updated", updated.ToString("yyyy-MM-dd HH:mm"));
            table.AddRow("Category", E(ticket.Category?.Name));
            table.AddRow("Requester", E(ticket.RequestedBy?.Name));
            
            AnsiConsole.Write(table);
        }
        
        return 0;
    }

    private static async Task<int> CreateTicketAsync(string[] args, IBoldDeskClient client)
    {
        var request = new CreateTicketRequest
        {
            Title = GetOption(args, "--title") ?? throw new InvalidOperationException("--title required"),
            Description = GetOption(args, "--description") ?? throw new InvalidOperationException("--description required")
        };
        
        // Optional parameters
        if (int.TryParse(GetOption(args, "--priority"), out var priority))
            request.PriorityId = priority;
        if (int.TryParse(GetOption(args, "--category"), out var category))
            request.CategoryId = category;
        if (int.TryParse(GetOption(args, "--agent"), out var agent))
            request.AgentId = agent;
        
        var tags = GetOption(args, "--tags");
        if (!string.IsNullOrEmpty(tags))
            request.Tags = tags.Split(',').ToList();
        
        var ticket = await client.Tickets.CreateTicketAsync(request);
        AnsiConsole.MarkupLine($"[green]Ticket created: #{ticket.TicketId}[/]");
        
        return 0;
    }

    private static async Task<int> UpdateTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID required[/]");
            return 1;
        }
        
        if (!int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid ticket ID[/]");
            return 1;
        }
        
        var request = new UpdateTicketRequest();
        
        var title = GetOption(args, "--title");
        if (!string.IsNullOrEmpty(title))
            request.Title = title;
        
        if (int.TryParse(GetOption(args, "--priority"), out var priority))
            request.PriorityId = priority;
        if (int.TryParse(GetOption(args, "--status"), out var status))
            request.StatusId = status;
        if (int.TryParse(GetOption(args, "--agent"), out var agent))
            request.AgentId = agent;
        
        var ticket = await client.Tickets.UpdateTicketAsync(ticketId, request);
        AnsiConsole.MarkupLine($"[green]Ticket #{ticket.TicketId} updated[/]");
        
        return 0;
    }

    private static async Task<int> DeleteTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID(s) required[/]");
            return 1;
        }
        
        var permanent = HasFlag(args, "--permanent");
        var ticketIds = args[0].Split(',').Select(int.Parse).ToList();
        
        if (ticketIds.Count == 1)
        {
            if (permanent)
                await client.Tickets.DeleteTicketPermanentlyAsync(ticketIds[0]);
            else
                await client.Tickets.DeleteTicketAsync(ticketIds[0]);
        }
        else
        {
            if (permanent)
                await client.Tickets.DeleteTicketsPermanentlyAsync(ticketIds);
            else
                await client.Tickets.DeleteTicketsAsync(ticketIds);
        }
        
        AnsiConsole.MarkupLine($"[green]Ticket(s) deleted{(permanent ? " permanently" : "")}[/]");
        return 0;
    }

    private static async Task<int> RestoreTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID(s) required[/]");
            return 1;
        }
        
        var ticketIds = args[0].Split(',').Select(int.Parse).ToList();
        
        if (ticketIds.Count == 1)
            await client.Tickets.RestoreTicketAsync(ticketIds[0]);
        else
            await client.Tickets.RestoreTicketsAsync(ticketIds);
        
        AnsiConsole.MarkupLine("[green]Ticket(s) restored[/]");
        return 0;
    }

    private static async Task<int> ReplyToTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID required[/]");
            return 1;
        }
        
        if (!int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid ticket ID[/]");
            return 1;
        }
        
        var description = GetOption(args, "--message") ?? throw new InvalidOperationException("--message required");
        var isPrivate = HasFlag(args, "--private");
        
        var request = new ReplyTicketRequest
        {
            Description = description,
            IsPrivate = isPrivate
        };
        
        await client.Tickets.ReplyTicketAsync(ticketId, request);
        AnsiConsole.MarkupLine($"[green]Reply added to ticket #{ticketId}[/]");
        
        return 0;
    }

    private static async Task<int> ManageMessagesAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID required[/]");
            return 1;
        }
        
        if (!int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid ticket ID[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var messages = await client.Tickets.GetTicketMessagesAsync(ticketId);
                OutputMessages(messages);
                break;
                
            case "public":
                var publicMessages = await client.Tickets.GetPublicMessagesAsync(ticketId);
                OutputMessages(publicMessages);
                break;
                
            case "delete":
                if (args.Length < 3 || !int.TryParse(args[2], out var messageId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Message ID required[/]");
                    return 1;
                }
                await client.Tickets.DeleteMessageAsync(ticketId, messageId);
                AnsiConsole.MarkupLine("[green]Message deleted[/]");
                break;
                
            case "edit":
                if (args.Length < 3 || !int.TryParse(args[2], out var editMessageId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Message ID required[/]");
                    return 1;
                }
                var newText = GetOption(args, "--text") ?? throw new InvalidOperationException("--text required");
                await client.Tickets.EditMessageAsync(ticketId, editMessageId, new EditMessageRequest { Description = newText });
                AnsiConsole.MarkupLine("[green]Message updated[/]");
                break;
                
            case "stats":
                var stats = await client.Tickets.GetPublicMessagesStatsAsync(ticketId);
                AnsiConsole.MarkupLine($"Min Message ID: {stats.MinMessageId}");
                AnsiConsole.MarkupLine($"Max Message ID: {stats.MaxMessageId}");
                AnsiConsole.MarkupLine($"Total Count: {stats.TotalCount}");
                break;
        }
        
        return 0;
    }

    private static async Task<int> ManageNotesAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID required[/]");
            return 1;
        }
        
        if (!int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid ticket ID[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var notes = await client.Tickets.GetTicketNotesAsync(ticketId);
                OutputNotes(notes);
                break;
                
            case "add":
                var description = GetOption(args, "--text") ?? throw new InvalidOperationException("--text required");
                var isPrivate = !HasFlag(args, "--public");
                var note = await client.Tickets.AddTicketNoteAsync(ticketId, new AddTicketNoteRequest
                {
                    Description = description,
                    IsPrivate = isPrivate
                });
                AnsiConsole.MarkupLine("[green]Note added[/]");
                break;
        }
        
        return 0;
    }

    private static async Task<int> ManageAttachmentsAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID required[/]");
            return 1;
        }
        
        if (!int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid ticket ID[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var attachments = await client.Tickets.GetTicketAttachmentsAsync(ticketId);
                OutputAttachments(attachments);
                break;
                
            case "download":
                if (args.Length < 3 || !int.TryParse(args[2], out var attachmentId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Attachment ID required[/]");
                    return 1;
                }
                var outputPath = GetOption(args, "--output") ?? $"attachment_{attachmentId}";
                using (var stream = await client.Tickets.DownloadAttachmentAsync(ticketId, attachmentId))
                using (var fileStream = File.Create(outputPath))
                {
                    await stream.CopyToAsync(fileStream);
                }
                AnsiConsole.MarkupLine($"[green]Attachment saved to: {outputPath}[/]");
                break;
                
            case "delete":
                if (args.Length < 3 || !int.TryParse(args[2], out var deleteAttachmentId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Attachment ID required[/]");
                    return 1;
                }
                await client.Tickets.DeleteAttachmentAsync(ticketId, deleteAttachmentId);
                AnsiConsole.MarkupLine("[green]Attachment deleted[/]");
                break;
                
            case "add":
                var filePath = GetOption(args, "--file") ?? throw new InvalidOperationException("--file required");
                using (var fileStream = File.OpenRead(filePath))
                {
                    var fileName = Path.GetFileName(filePath);
                    var attachment = await client.Tickets.AddAttachmentAsync(ticketId, fileStream, fileName);
                    AnsiConsole.MarkupLine($"[green]Attachment added: #{attachment.AttachmentId}[/]");
                }
                break;
        }
        
        return 0;
    }

    private static async Task<int> ManageTagsAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID required[/]");
            return 1;
        }
        
        if (!int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid ticket ID[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var tags = await client.Tickets.GetTicketTagsAsync(ticketId);
                OutputTags(tags);
                break;
                
            case "add":
                var addTags = GetOption(args, "--tags")?.Split(',').ToList() 
                    ?? throw new InvalidOperationException("--tags required");
                await client.Tickets.AddTagsAsync(ticketId, addTags);
                AnsiConsole.MarkupLine("[green]Tags added[/]");
                break;
                
            case "remove":
                var removeTags = GetOption(args, "--tags")?.Split(',').ToList() 
                    ?? throw new InvalidOperationException("--tags required");
                await client.Tickets.RemoveTagsAsync(ticketId, removeTags);
                AnsiConsole.MarkupLine("[green]Tags removed[/]");
                break;
        }
        
        return 0;
    }

    private static async Task<int> MergeTicketsAsync(string[] args, IBoldDeskClient client)
    {
        var primary = GetOption(args, "--primary");
        var secondary = GetOption(args, "--secondary");
        
        if (string.IsNullOrEmpty(primary) || string.IsNullOrEmpty(secondary))
        {
            AnsiConsole.MarkupLine("[red]Error: --primary and --secondary required[/]");
            return 1;
        }
        
        var request = new MergeTicketsRequest
        {
            PrimaryTicketId = int.Parse(primary),
            SecondaryTicketIds = secondary.Split(',').Select(int.Parse).ToList()
        };
        
        await client.Tickets.MergeTicketsAsync(request);
        AnsiConsole.MarkupLine("[green]Tickets merged[/]");
        
        return 0;
    }

    private static async Task<int> GetMetricsAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length > 0 && int.TryParse(args[0], out var ticketId))
        {
            var metrics = await client.Tickets.GetTicketMetricsAsync(ticketId);
            OutputMetrics(metrics);
        }
        else
        {
            var parameters = new TicketMetricsQueryParameters();
            var metrics = await client.Tickets.GetTicketMetricsListAsync(parameters);
            OutputMetricsList(metrics);
        }
        
        return 0;
    }

    private static async Task<int> ArchiveTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID(s) required[/]");
            return 1;
        }
        
        var ticketIds = args[0].Split(',').Select(int.Parse).ToList();
        
        if (ticketIds.Count == 1)
            await client.Tickets.ArchiveTicketAsync(ticketIds[0]);
        else
            await client.Tickets.ArchiveTicketsAsync(ticketIds);
        
        AnsiConsole.MarkupLine("[green]Ticket(s) archived[/]");
        return 0;
    }

    private static async Task<int> ManageSpamAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length < 1)
        {
            AnsiConsole.MarkupLine("[red]Error: Ticket ID and action (mark/unmark) required[/]");
            return 1;
        }
        
        if (!int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid ticket ID[/]");
            return 1;
        }
        
        var action = args[0].ToLowerInvariant();
        
        if (action == "mark")
        {
            await client.Tickets.MarkAsSpamAsync(ticketId);
            AnsiConsole.MarkupLine("[green]Ticket marked as spam[/]");
        }
        else if (action == "unmark")
        {
            await client.Tickets.RemoveFromSpamAsync(ticketId);
            AnsiConsole.MarkupLine("[green]Ticket removed from spam[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid action. Use 'mark' or 'unmark'[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> LockTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        await client.Tickets.LockTicketAsync(ticketId);
        AnsiConsole.MarkupLine($"[green]Ticket #{ticketId} locked[/]");
        return 0;
    }

    private static async Task<int> UnlockTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        await client.Tickets.UnlockTicketAsync(ticketId);
        AnsiConsole.MarkupLine($"[green]Ticket #{ticketId} unlocked[/]");
        return 0;
    }

    // Additional command implementations...
    private static async Task<int> ManageWatchersAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var watchers = await client.Tickets.GetWatchersAsync(ticketId);
                OutputWatchers(watchers);
                break;
                
            case "add":
                var addUsers = GetOption(args, "--users")?.Split(',').Select(int.Parse).ToList() 
                    ?? throw new InvalidOperationException("--users required");
                await client.Tickets.AddWatchersAsync(ticketId, addUsers);
                AnsiConsole.MarkupLine("[green]Watchers added[/]");
                break;
                
            case "remove":
                var removeUsers = GetOption(args, "--users")?.Split(',').Select(int.Parse).ToList() 
                    ?? throw new InvalidOperationException("--users required");
                await client.Tickets.RemoveWatchersAsync(ticketId, removeUsers);
                AnsiConsole.MarkupLine("[green]Watchers removed[/]");
                break;
        }
        
        return 0;
    }

    private static async Task<int> SplitTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        var messageIds = GetOption(args, "--messages")?.Split(',').Select(int.Parse).ToList() 
            ?? throw new InvalidOperationException("--messages required");
        var title = GetOption(args, "--title") ?? throw new InvalidOperationException("--title required");
        
        var request = new SplitTicketRequest
        {
            MessageIds = messageIds,
            Title = title
        };
        
        var newTicket = await client.Tickets.SplitTicketAsync(ticketId, request);
        AnsiConsole.MarkupLine($"[green]New ticket created: #{newTicket.TicketId}[/]");
        
        return 0;
    }

    private static async Task<int> LinkTicketsAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        var linkedIds = GetOption(args, "--tickets")?.Split(',').Select(int.Parse).ToList() 
            ?? throw new InvalidOperationException("--tickets required");
        var linkType = GetOption(args, "--type") ?? "related";
        
        var request = new LinkTicketsRequest
        {
            LinkType = linkType,
            LinkedTicketIds = linkedIds
        };
        
        await client.Tickets.LinkTicketsAsync(ticketId, request);
        AnsiConsole.MarkupLine("[green]Tickets linked[/]");
        
        return 0;
    }

    private static async Task<int> ManageLinksAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var links = await client.Tickets.GetLinkedTicketsAsync(ticketId);
                OutputLinkedTickets(links);
                break;
                
            case "count":
                var count = await client.Tickets.GetLinkedTicketsCountAsync(ticketId);
                AnsiConsole.MarkupLine($"Linked tickets: {count}");
                break;
                
            case "convert":
                if (args.Length < 3 || !int.TryParse(args[2], out var linkId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Link ID required[/]");
                    return 1;
                }
                var newType = GetOption(args, "--type") ?? throw new InvalidOperationException("--type required");
                await client.Tickets.ConvertLinkTypeAsync(ticketId, linkId, newType);
                AnsiConsole.MarkupLine("[green]Link type converted[/]");
                break;
                
            case "remove":
                if (args.Length < 3 || !int.TryParse(args[2], out var removeLinkId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Link ID required[/]");
                    return 1;
                }
                await client.Tickets.RemoveTicketLinkAsync(ticketId, removeLinkId);
                AnsiConsole.MarkupLine("[green]Link removed[/]");
                break;
        }
        
        return 0;
    }

    private static async Task<int> ManageArticleLinksAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var articles = await client.Tickets.GetArticleLinksAsync(ticketId);
                OutputArticleLinks(articles);
                break;
                
            case "add":
                if (!int.TryParse(GetOption(args, "--article"), out var articleId))
                {
                    AnsiConsole.MarkupLine("[red]Error: --article ID required[/]");
                    return 1;
                }
                await client.Tickets.AddArticleLinkAsync(ticketId, articleId);
                AnsiConsole.MarkupLine("[green]Article linked[/]");
                break;
                
            case "remove":
                if (args.Length < 3 || !int.TryParse(args[2], out var linkId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Link ID required[/]");
                    return 1;
                }
                await client.Tickets.RemoveArticleLinkAsync(ticketId, linkId);
                AnsiConsole.MarkupLine("[green]Article link removed[/]");
                break;
        }
        
        return 0;
    }

    private static async Task<int> ManageWebLinksAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var weblinks = await client.Tickets.GetWebLinksAsync(ticketId);
                OutputWebLinks(weblinks);
                break;
                
            case "add":
                var url = GetOption(args, "--url") ?? throw new InvalidOperationException("--url required");
                var description = GetOption(args, "--description");
                var request = new AddWebLinkRequest { Url = url, Description = description };
                await client.Tickets.AddWebLinkAsync(ticketId, request);
                AnsiConsole.MarkupLine("[green]Web link added[/]");
                break;
                
            case "remove":
                if (args.Length < 3 || !int.TryParse(args[2], out var linkId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Link ID required[/]");
                    return 1;
                }
                await client.Tickets.RemoveWebLinkAsync(ticketId, linkId);
                AnsiConsole.MarkupLine("[green]Web link removed[/]");
                break;
        }
        
        return 0;
    }

    private static async Task<int> ManageRelatedContactsAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        var action = args.Length > 1 ? args[1] : "list";
        
        switch (action)
        {
            case "list":
                var contacts = await client.Tickets.GetRelatedContactsAsync(ticketId);
                OutputRelatedContacts(contacts);
                break;
                
            case "add":
                var userIds = GetOption(args, "--users")?.Split(',').Select(int.Parse).ToList() 
                    ?? throw new InvalidOperationException("--users required");
                var notes = GetOption(args, "--notes");
                var request = new AddRelatedContactRequest { UserIds = userIds, Notes = notes };
                await client.Tickets.AddRelatedContactsAsync(ticketId, request);
                AnsiConsole.MarkupLine("[green]Related contacts added[/]");
                break;
                
            case "remove":
                if (args.Length < 3 || !int.TryParse(args[2], out var contactId))
                {
                    AnsiConsole.MarkupLine("[red]Error: Contact ID required[/]");
                    return 1;
                }
                await client.Tickets.RemoveRelatedContactAsync(ticketId, contactId);
                AnsiConsole.MarkupLine("[green]Related contact removed[/]");
                break;
        }
        
        return 0;
    }

    private static async Task<int> ShareTicketAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid ticket ID required[/]");
            return 1;
        }
        
        if (args.Length > 1 && args[1] == "remove")
        {
            if (args.Length < 3 || !int.TryParse(args[2], out var shareId))
            {
                AnsiConsole.MarkupLine("[red]Error: Share ID required[/]");
                return 1;
            }
            var isAgent = HasFlag(args, "--agent");
            await client.Tickets.RemoveTicketShareAsync(ticketId, shareId, isAgent);
            AnsiConsole.MarkupLine("[green]Ticket share removed[/]");
        }
        else if (args.Length > 1 && args[1] == "get")
        {
            var share = await client.Tickets.GetTicketShareAsync(ticketId);
            OutputTicketShare(share);
        }
        else
        {
            var scopeId = int.Parse(GetOption(args, "--scope") ?? throw new InvalidOperationException("--scope required"));
            var request = new ShareTicketRequest { AccessScopeId = scopeId };
            await client.Tickets.ShareTicketAsync(ticketId, request);
            AnsiConsole.MarkupLine("[green]Ticket shared[/]");
        }
        
        return 0;
    }

    private static async Task<int> GetPrioritiesAsync(string[] args, IBoldDeskClient client)
    {
        var priorities = await client.Tickets.GetPrioritiesAsync();
        OutputPriorities(priorities);
        return 0;
    }

    private static async Task<int> GetStatusesAsync(string[] args, IBoldDeskClient client)
    {
        var statuses = await client.Tickets.GetStatusesAsync();
        OutputStatuses(statuses);
        return 0;
    }

    private static async Task<int> GetSourcesAsync(string[] args, IBoldDeskClient client)
    {
        var sources = await client.Tickets.GetSourcesAsync();
        OutputSources(sources);
        return 0;
    }

    private static async Task<int> GetFieldsAsync(string[] args, IBoldDeskClient client)
    {
        var fields = await client.Tickets.GetFieldsAsync();
        OutputFields(fields);
        return 0;
    }

    private static async Task<int> GetFormsAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length > 0 && int.TryParse(args[0], out var formId))
        {
            var form = await client.Tickets.GetFormAsync(formId);
            OutputForm(form);
        }
        else
        {
            var forms = await client.Tickets.GetFormsAsync();
            OutputForms(forms);
        }
        return 0;
    }

    private static async Task<int> GetHistoryAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0 || !int.TryParse(args[0], out var ticketId))
        {
            // Get all ticket histories
            var allHistory = await client.Tickets.GetTicketHistoriesAsync();
            OutputHistories(allHistory);
        }
        else
        {
            var history = await client.Tickets.GetTicketHistoryAsync(ticketId);
            OutputHistories(history);
        }
        return 0;
    }

    private static async Task<int> ManageLogsAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Log type required (email/delete)[/]");
            return 1;
        }
        
        var logType = args[0].ToLowerInvariant();
        
        switch (logType)
        {
            case "email":
                if (args.Length > 1 && int.TryParse(args[1], out var ticketId))
                {
                    var emailLogs = await client.Tickets.GetEmailDeliveryLogsAsync(ticketId);
                    OutputEmailLogs(emailLogs);
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Error: Ticket ID required for email logs[/]");
                    return 1;
                }
                break;
                
            case "delete":
                var deleteLogs = await client.Tickets.GetPermanentDeleteLogsAsync();
                OutputDeleteLogs(deleteLogs);
                break;
                
            default:
                AnsiConsole.MarkupLine("[red]Error: Invalid log type. Use 'email' or 'delete'[/]");
                return 1;
        }
        
        return 0;
    }

    private static async Task<int> GetDeletedTicketsAsync(string[] args, IBoldDeskClient client)
    {
        var parameters = ParseTicketQueryParameters(args);
        var tickets = await client.Tickets.GetDeletedTicketsAsync(parameters);
        OutputTicketResponse(tickets, GetFormat(args));
        return 0;
    }

    private static async Task<int> GetArchivedTicketsAsync(string[] args, IBoldDeskClient client)
    {
        var parameters = ParseTicketQueryParameters(args);
        var tickets = await client.Tickets.GetArchivedTicketsAsync(parameters);
        OutputTicketResponse(tickets, GetFormat(args));
        return 0;
    }

    private static async Task<int> GetSpamTicketsAsync(string[] args, IBoldDeskClient client)
    {
        var parameters = ParseTicketQueryParameters(args);
        var tickets = await client.Tickets.GetSpamTicketsAsync(parameters);
        OutputTicketResponse(tickets, GetFormat(args));
        return 0;
    }

    private static async Task<int> SearchForLinkingAsync(string[] args, IBoldDeskClient client)
    {
        var searchText = GetOption(args, "--text") ?? throw new InvalidOperationException("--text required");
        var request = new SearchTicketForLinkingRequest { SearchText = searchText };
        
        var excludeId = GetOption(args, "--exclude");
        if (!string.IsNullOrEmpty(excludeId) && int.TryParse(excludeId, out var exclude))
        {
            request.ExcludeTicketId = exclude;
        }
        
        var tickets = await client.Tickets.SearchTicketsForLinkingAsync(request);
        OutputTicketResponse(tickets, GetFormat(args));
        return 0;
    }

    private static async Task<int> RecoverSuspendedEmailAsync(string[] args, IBoldDeskClient client)
    {
        var emailId = int.Parse(GetOption(args, "--email-id") ?? throw new InvalidOperationException("--email-id required"));
        var request = new RecoverSuspendedEmailRequest { SuspendedEmailId = emailId };
        
        var ticketId = GetOption(args, "--ticket-id");
        if (!string.IsNullOrEmpty(ticketId) && int.TryParse(ticketId, out var tid))
        {
            request.TicketId = tid;
        }
        else
        {
            request.CreateNewTicket = true;
        }
        
        var ticket = await client.Tickets.RecoverSuspendedEmailAsync(request);
        AnsiConsole.MarkupLine($"[green]Email recovered as ticket #{ticket.TicketId}[/]");
        return 0;
    }

    // Helper methods
    private static TicketQueryParameters ParseTicketQueryParameters(string[] args)
    {
        var parameters = new TicketQueryParameters();
        
        var page = GetOption(args, "--page");
        if (!string.IsNullOrEmpty(page) && int.TryParse(page, out var p))
            parameters.Page = p;
        
        var perPage = GetOption(args, "--per-page");
        if (!string.IsNullOrEmpty(perPage) && int.TryParse(perPage, out var pp))
            parameters.PerPage = pp;
        
        parameters.Q = GetOption(args, "--query");
        parameters.FilterId = GetOption(args, "--filter");
        parameters.OrderBy = GetOption(args, "--order-by");
        
        var fields = GetOption(args, "--fields");
        if (!string.IsNullOrEmpty(fields))
            parameters.Fields = fields.Split(',').ToList();
        
        var brandIds = GetOption(args, "--brands");
        if (!string.IsNullOrEmpty(brandIds))
            parameters.BrandIds = brandIds.Split(',').Select(int.Parse).ToList();
        
        return parameters;
    }

    private static string? GetOption(string[] args, string option)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == option)
                return args[i + 1];
        }
        return null;
    }

    private static bool HasFlag(string[] args, string flag)
    {
        return args.Contains(flag);
    }

    private static string GetFormat(string[] args)
    {
        return GetOption(args, "--format") ?? "table";
    }

    // Output helpers
    private static void OutputTickets(List<Ticket> tickets, string format)
    {
        if (format == "json")
        {
            AnsiConsole.WriteLine(JsonSerializer.Serialize(tickets, new JsonSerializerOptions { WriteIndented = true }));
        }
        else
        {
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Title");
            table.AddColumn("Status");
            table.AddColumn("Priority");
            table.AddColumn("Agent");
            table.AddColumn("Created");
            
            foreach (var ticket in tickets)
            {
                table.AddRow(
                    ticket.TicketId.ToString(),
                    E(ticket.Title),
                    E(ticket.Status?.Description),
                    E(ticket.Priority?.Description),
                    string.IsNullOrEmpty(ticket.Agent?.Name) ? "Unassigned" : E(ticket.Agent?.Name),
                    ticket.CreatedOn.ToString("yyyy-MM-dd")
                );
            }
            
            AnsiConsole.Write(table);
        }
    }

    private static void OutputTicketResponse(BoldDeskResponse<Ticket> response, string format)
    {
        if (format == "json")
        {
            AnsiConsole.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        }
        else
        {
            AnsiConsole.MarkupLine($"[cyan]Total: {response.Count}[/]");
            OutputTickets(response.Result, format);
        }
    }

    private static void OutputMessages(BoldDeskResponse<TicketMessage> messages)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("From");
        table.AddColumn("Type");
        table.AddColumn("Created");
        table.AddColumn("Private");
        
        foreach (var message in messages.Result)
        {
            table.AddRow(
                message.MessageId.ToString(),
                string.IsNullOrEmpty(message.FromUser?.Name) ? "System" : E(message.FromUser?.Name),
                string.IsNullOrEmpty(message.MessageType) ? "Reply" : E(message.MessageType),
                message.CreatedOn.ToString("yyyy-MM-dd HH:mm"),
                message.IsPrivate ? "Yes" : "No"
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputNotes(BoldDeskResponse<TicketNote> notes)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("From");
        table.AddColumn("Created");
        table.AddColumn("Private");
        
        foreach (var note in notes.Result)
        {
            table.AddRow(
                note.NoteId.ToString(),
                string.IsNullOrEmpty(note.FromUser?.Name) ? "System" : E(note.FromUser?.Name),
                note.CreatedOn.ToString("yyyy-MM-dd HH:mm"),
                note.IsPrivate ? "Yes" : "No"
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputAttachments(BoldDeskResponse<TicketAttachment> attachments)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("File Name");
        table.AddColumn("Size");
        table.AddColumn("Type");
        table.AddColumn("Created");
        
        foreach (var attachment in attachments.Result)
        {
            table.AddRow(
                attachment.AttachmentId.ToString(),
                E(attachment.FileName),
                $"{attachment.FileSize / 1024}KB",
                E(attachment.ContentType) == string.Empty ? "Unknown" : E(attachment.ContentType),
                attachment.CreatedOn.ToString("yyyy-MM-dd HH:mm")
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputTags(BoldDeskResponse<Tag> tags)
    {
        AnsiConsole.MarkupLine("Tags: " + string.Join(", ", tags.Result.Select(t => $"[{t.Color ?? "white"}]{E(t.TagName)}[/]")));
    }

    private static void OutputWatchers(BoldDeskResponse<TicketWatcher> watchers)
    {
        var table = new Table();
        table.AddColumn("User");
        table.AddColumn("Added On");
        
        foreach (var watcher in watchers.Result)
        {
            table.AddRow(
                string.IsNullOrEmpty(watcher.User?.Name) ? $"User {watcher.UserId}" : E(watcher.User?.Name),
                watcher.AddedOn.ToString("yyyy-MM-dd HH:mm")
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputMetrics(TicketMetrics metrics)
    {
        var table = new Table();
        table.AddColumn("Metric");
        table.AddColumn("Value");
        
        table.AddRow("First Response Time", metrics.FirstResponseTime?.ToString() ?? "N/A");
        table.AddRow("Resolution Time", metrics.ResolutionTime?.ToString() ?? "N/A");
        table.AddRow("Response Count", metrics.ResponseCount.ToString());
        table.AddRow("Reopen Count", metrics.ReopenCount.ToString());
        table.AddRow("Agent Interactions", metrics.AgentInteractions.ToString());
        table.AddRow("Customer Interactions", metrics.CustomerInteractions.ToString());
        
        AnsiConsole.Write(table);
    }

    private static void OutputMetricsList(BoldDeskResponse<TicketMetrics> metrics)
    {
        var table = new Table();
        table.AddColumn("Ticket ID");
        table.AddColumn("First Response");
        table.AddColumn("Resolution Time");
        table.AddColumn("Responses");
        
        foreach (var metric in metrics.Result)
        {
            table.AddRow(
                metric.TicketId.ToString(),
                metric.FirstResponseTime?.ToString() ?? "N/A",
                metric.ResolutionTime?.ToString() ?? "N/A",
                metric.ResponseCount.ToString()
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputLinkedTickets(BoldDeskResponse<LinkedTicket> links)
    {
        var table = new Table();
        table.AddColumn("Link ID");
        table.AddColumn("Ticket ID");
        table.AddColumn("Type");
        table.AddColumn("Title");
        
        foreach (var link in links.Result)
        {
            table.AddRow(
                link.LinkId.ToString(),
                link.LinkedTicketId.ToString(),
                E(link.LinkType),
                E(link.LinkedTicketData?.Title)
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputArticleLinks(BoldDeskResponse<TicketArticleLink> articles)
    {
        var table = new Table();
        table.AddColumn("Link ID");
        table.AddColumn("Article ID");
        table.AddColumn("Title");
        table.AddColumn("Created");
        
        foreach (var article in articles.Result)
        {
            table.AddRow(
                article.LinkId.ToString(),
                article.ArticleId.ToString(),
                E(article.ArticleTitle),
                article.CreatedOn.ToString("yyyy-MM-dd HH:mm")
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputWebLinks(BoldDeskResponse<TicketWebLink> weblinks)
    {
        var table = new Table();
        table.AddColumn("Link ID");
        table.AddColumn("URL");
        table.AddColumn("Description");
        table.AddColumn("Created");
        
        foreach (var link in weblinks.Result)
        {
            table.AddRow(
                link.LinkId.ToString(),
                E(link.Url),
                E(link.Description),
                link.CreatedOn.ToString("yyyy-MM-dd HH:mm")
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputRelatedContacts(BoldDeskResponse<RelatedContact> contacts)
    {
        var table = new Table();
        table.AddColumn("Link ID");
        table.AddColumn("Contact");
        table.AddColumn("Notes");
        table.AddColumn("Created");
        
        foreach (var contact in contacts.Result)
        {
            table.AddRow(
                contact.LinkId.ToString(),
                $"Contact {contact.UserId}",
                E(contact.Notes),
                contact.CreatedOn.ToString("yyyy-MM-dd HH:mm")
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputTicketShare(TicketShare share)
    {
        AnsiConsole.MarkupLine($"[cyan]Share ID: {share.ShareId}[/]");
        AnsiConsole.MarkupLine($"[cyan]Access Scope: {share.AccessScopeId}[/]");
        
        if (share.SharedWith != null && share.SharedWith.Count > 0)
        {
            var table = new Table();
            table.AddColumn("Type");
            table.AddColumn("ID");
            table.AddColumn("Name");
            
            foreach (var entity in share.SharedWith)
            {
                table.AddRow(
                    entity.IsAgent ? "Agent" : "Group",
                    entity.Id.ToString(),
                    E(entity.Name)
                );
            }
            
            AnsiConsole.Write(table);
        }
    }

    private static void OutputPriorities(BoldDeskResponse<Priority> priorities)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Description");
        
        foreach (var priority in priorities.Result)
        {
            table.AddRow(
                priority.Id.ToString(),
                E(priority.Description),
                E(priority.Description)
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputStatuses(BoldDeskResponse<BoldDesk.Models.Status> statuses)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Description");
        
        foreach (var status in statuses.Result)
        {
            table.AddRow(
                status.Id.ToString(),
                E(status.Description),
                E(status.Description)
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputSources(BoldDeskResponse<TicketSource> sources)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Icon");
        
        foreach (var source in sources.Result)
        {
            table.AddRow(
                source.SourceId.ToString(),
                E(source.SourceName),
                E(source.Icon)
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputFields(BoldDeskResponse<TicketField> fields)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Type");
        table.AddColumn("Required");
        
        foreach (var field in fields.Result)
        {
            table.AddRow(
                field.FieldId.ToString(),
                E(field.FieldName),
                E(field.FieldType),
                field.IsRequired ? "Yes" : "No"
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputForm(TicketForm form)
    {
        AnsiConsole.MarkupLine($"[cyan]Form: {E(form.FormName)}[/]");
        AnsiConsole.MarkupLine($"Active: {(form.IsActive ? "Yes" : "No")}");
        
        if (form.Fields != null && form.Fields.Count > 0)
        {
            OutputFields(new BoldDeskResponse<TicketField> { Result = form.Fields });
        }
    }

    private static void OutputForms(BoldDeskResponse<TicketForm> forms)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Active");
        table.AddColumn("Created");
        
        foreach (var form in forms.Result)
        {
            table.AddRow(
                form.FormId.ToString(),
                E(form.FormName),
                form.IsActive ? "Yes" : "No",
                form.CreatedOn.ToString("yyyy-MM-dd")
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputHistories(BoldDeskResponse<TicketHistory> histories)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Action");
        table.AddColumn("Field");
        table.AddColumn("Old Value");
        table.AddColumn("New Value");
        table.AddColumn("User");
        table.AddColumn("Date");
        
        foreach (var history in histories.Result)
        {
            table.AddRow(
                history.HistoryId.ToString(),
                E(history.Action),
                E(history.Field),
                E(history.OldValue),
                E(history.NewValue),
                E(history.User?.Name),
                history.CreatedOn.ToString("yyyy-MM-dd HH:mm")
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputEmailLogs(BoldDeskResponse<EmailDeliveryLog> logs)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Message");
        table.AddColumn("Recipient");
        table.AddColumn("Status");
        table.AddColumn("Sent");
        
        foreach (var log in logs.Result)
        {
            table.AddRow(
                log.LogId.ToString(),
                log.MessageId.ToString(),
                E(log.Recipient),
                E(log.Status),
                log.SentOn?.ToString("yyyy-MM-dd HH:mm") ?? "Not sent"
            );
        }
        
        AnsiConsole.Write(table);
    }

    private static void OutputDeleteLogs(BoldDeskResponse<PermanentDeleteLog> logs)
    {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Ticket");
        table.AddColumn("Deleted By");
        table.AddColumn("Date");
        table.AddColumn("Reason");
        
        foreach (var log in logs.Result)
        {
            table.AddRow(
                log.LogId.ToString(),
                log.TicketId.ToString(),
                string.IsNullOrEmpty(log.DeletedByUser?.Name) ? $"User {log.DeletedBy}" : E(log.DeletedByUser?.Name),
                log.DeletedOn.ToString("yyyy-MM-dd HH:mm"),
                E(log.Reason)
            );
        }
        
        AnsiConsole.Write(table);
    }
}

using BoldDesk;
using BoldDesk.Models;
using BoldDesk.QueryBuilder;
using Spectre.Console;
using System.Text.Json;

namespace BoldDesk.Cli.Commands;

public static class ContactGroupCommands
{
    public static async Task<int> HandleContactGroupCommandAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length < 1)
        {
            ShowHelp();
            return 0;
        }

        var subCommand = args[0].ToLowerInvariant();
        var remainingArgs = args.Skip(1).ToArray();

        return subCommand switch
        {
            "list" => await ListContactGroupsAsync(remainingArgs, client),
            "get" => await GetContactGroupAsync(remainingArgs, client),
            "create" => await CreateContactGroupAsync(remainingArgs, client),
            "update" => await UpdateContactGroupAsync(remainingArgs, client),
            "delete" => await DeleteContactGroupAsync(remainingArgs, client),
            "members" => await GetContactGroupMembersAsync(remainingArgs, client),
            "add-members" => await AddMembersToGroupAsync(remainingArgs, client),
            "remove-members" => await RemoveMembersFromGroupAsync(remainingArgs, client),
            "help" or "--help" or "-h" => ShowHelpAndReturn(),
            _ => ShowInvalidCommandAndReturn(subCommand)
        };
    }

    private static void ShowHelp()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]BoldDesk Contact Group Commands[/]").RuleStyle("grey"));
        AnsiConsole.WriteLine();
        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[blue]Command[/]")
            .AddColumn("[green]Description[/]")
            .AddColumn("Options");

        table.AddRow("contact-groups list", "List all contact groups", "--page <number>, --per-page <number>, --search <term>");
        table.AddRow("contact-groups get", "Get a specific contact group", "--id <groupId>");
        table.AddRow("contact-groups create", "Create a new contact group", "--name <name>, --description <desc>");
        table.AddRow("contact-groups update", "Update a contact group", "--id <groupId>, --name <name>, --description <desc>");
        table.AddRow("contact-groups delete", "Delete a contact group", "--id <groupId>");
        table.AddRow("contact-groups members", "Get group members", "--id <groupId>");
        table.AddRow("contact-groups add-members", "Add members to group", "--id <groupId>, --contacts <id1,id2>");
        table.AddRow("contact-groups remove-members", "Remove members from group", "--id <groupId>, --contacts <id1,id2>");
        
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[grey]Examples:[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contact-groups list --search \"VIP\" --per-page 10[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contact-groups get --id 123[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contact-groups create --name \"Premium Customers\" --description \"High-value customers\"[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contact-groups add-members --id 123 --contacts 456,789[/]");
        AnsiConsole.WriteLine();
    }

    private static int ShowHelpAndReturn()
    {
        ShowHelp();
        return 0;
    }

    private static int ShowInvalidCommandAndReturn(string command)
    {
        AnsiConsole.MarkupLine($"[red]Invalid command: {command}[/]");
        ShowHelp();
        return 1;
    }

    private static string? GetOption(string[] args, string optionName)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i].Equals(optionName, StringComparison.OrdinalIgnoreCase))
            {
                return args[i + 1];
            }
        }
        return null;
    }

    private static bool HasFlag(string[] args, string flagName)
    {
        return args.Any(a => a.Equals(flagName, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<int> ListContactGroupsAsync(string[] args, IBoldDeskClient client)
    {
        try
        {
            var page = int.TryParse(GetOption(args, "--page"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page"), out var pp) ? pp : 20;
            var searchTerm = GetOption(args, "--search");
            
            var parameters = new ContactGroupQueryParameters
            {
                Page = page,
                PerPage = perPage,
                Filter = searchTerm,
                RequiresCounts = true
            };

            await AnsiConsole.Status()
                .StartAsync("Fetching contact groups...", async ctx =>
                {
                    var response = await client.ContactGroups.ListContactGroupsAsync(parameters);
                    
                    if (!response.Result.Any())
                    {
                        AnsiConsole.MarkupLine("[yellow]No contact groups found[/]");
                        return;
                    }

                    var table = new Table()
                        .Border(TableBorder.Rounded)
                        .Title($"[yellow]Contact Groups (Page {page}, Total: {response.Count})[/]")
                        .AddColumn("ID")
                        .AddColumn("Name")
                        .AddColumn("Description")
                        .AddColumn("Members")
                        .AddColumn("Created");

                    foreach (var group in response.Result)
                    {
                        table.AddRow(
                            group.ContactGroupId.ToString(),
                            group.ContactGroupName ?? "",
                            group.Description ?? "",
                            "N/A", // ContactsCount not available in ContactGroup model
                            group.CreatedOn.ToString("yyyy-MM-dd")
                        );
                    }

                    AnsiConsole.Write(table);
                    
                    if (response.Count > perPage)
                    {
                        AnsiConsole.MarkupLine($"[dim]Showing {response.Result.Count} of {response.Count} groups. Use --page to navigate.[/]");
                    }
                });
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> GetContactGroupAsync(string[] args, IBoldDeskClient client)
    {
        var groupId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(groupId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        try
        {
            if (!int.TryParse(groupId, out var id))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid group ID[/]");
                return 1;
            }

            var group = await client.ContactGroups.GetContactGroupAsync(id);
            
            if (group == null)
            {
                AnsiConsole.MarkupLine($"[red]Contact group with ID {id} not found[/]");
                return 1;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Contact Group Details - ID: {id}[/]")
                .AddColumn(new TableColumn("[blue]Property[/]").Width(20))
                .AddColumn("[green]Value[/]");

            table.AddRow("ID", group.ContactGroupId.ToString());
            table.AddRow("Name", group.ContactGroupName ?? "");
            table.AddRow("Description", group.Description ?? "");
            table.AddRow("Created On", group.CreatedOn.ToString("yyyy-MM-dd HH:mm"));
            table.AddRow("Modified On", group.LastModifiedOn.ToString("yyyy-MM-dd HH:mm"));
            
            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> CreateContactGroupAsync(string[] args, IBoldDeskClient client)
    {
        var name = GetOption(args, "--name");
        if (string.IsNullOrEmpty(name))
        {
            AnsiConsole.MarkupLine("[red]Error: --name is required[/]");
            return 1;
        }

        try
        {
            var request = new AddContactGroupRequest
            {
                ContactGroupName = name,
                ContactGroupDescription = GetOption(args, "--description")
            };

            // Note: Initial contact assignment during creation is not supported in this simplified command

            AnsiConsole.MarkupLine("[yellow]Creating contact group...[/]");
            var response = await client.ContactGroups.AddContactGroupAsync(request);
            
            // Determine success robustly: some tenants return only message or id
            var isSuccess = response.IsSuccess == true
                            || (response.Id.HasValue && response.Id.Value > 0)
                            || (!string.IsNullOrWhiteSpace(response.Message) &&
                                response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));

            if (isSuccess)
            {
                var idText = response.Id.HasValue ? response.Id.Value.ToString() : "(unknown)";
                AnsiConsole.MarkupLine($"[green]✓ Contact group created successfully with ID: {idText}[/]");
            }
            else
            {
                var msg = string.IsNullOrWhiteSpace(response.Message) ? "Unknown error" : response.Message;
                AnsiConsole.MarkupLine($"[red]Failed to create contact group: {msg}[/]");
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> UpdateContactGroupAsync(string[] args, IBoldDeskClient client)
    {
        var groupId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(groupId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        if (!int.TryParse(groupId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid group ID[/]");
            return 1;
        }

        try
        {
            var request = new UpdateContactGroupFieldsRequest();
            
            var name = GetOption(args, "--name");
            var description = GetOption(args, "--description");
            
            if (!string.IsNullOrEmpty(name))
                request.Fields["contactGroupName"] = name;
            if (!string.IsNullOrEmpty(description))
                request.Fields["contactGroupDescription"] = description;

            AnsiConsole.MarkupLine($"[yellow]Updating contact group {id}...[/]");
            var response = await client.ContactGroups.UpdateContactGroupAsync(id, request);
            var isSuccess = response.IsSuccess == true
                            || (response.Id.HasValue && response.Id.Value > 0)
                            || (!string.IsNullOrWhiteSpace(response.Message) && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (isSuccess)
                AnsiConsole.MarkupLine($"[green]✓ Contact group {id} updated successfully[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to update contact group {id}: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> DeleteContactGroupAsync(string[] args, IBoldDeskClient client)
    {
        var groupId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(groupId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        if (!int.TryParse(groupId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid group ID[/]");
            return 1;
        }

        try
        {
            if (!HasFlag(args, "--force"))
            {
                if (!AnsiConsole.Confirm($"Are you sure you want to delete contact group {id}?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            AnsiConsole.MarkupLine($"[yellow]Deleting contact group {id}...[/]");
            var deleteRequest = new DeleteContactGroupsRequest
            {
                ContactGroupIds = new List<long> { id }
            };
            var delResp = await client.ContactGroups.DeleteContactGroupsAsync(deleteRequest);
            var delOk = (delResp.Result?.IsSuccess == true)
                        || (!string.IsNullOrWhiteSpace(delResp.Message) && delResp.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (delOk)
                AnsiConsole.MarkupLine($"[green]✓ Contact group {id} deleted successfully[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to delete contact group {id}: {delResp.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> GetContactGroupMembersAsync(string[] args, IBoldDeskClient client)
    {
        var groupId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(groupId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        try
        {
            if (!int.TryParse(groupId, out var id))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid group ID[/]");
                return 1;
            }

            var page = int.TryParse(GetOption(args, "--page"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page"), out var pp) ? pp : 20;

            var parameters = new ContactQueryParameters
            {
                Page = page,
                PerPage = perPage
            };

            var members = await client.ContactGroups.GetContactsByGroupAsync(id, new ContactGroupContactsQueryParameters
            {
                Page = parameters.Page,
                PerPage = parameters.PerPage
            });
            
            if (!members.Result.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]No members found in contact group {id}[/]");
                return 0;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Contact Group {id} Members (Total: {members.Count})[/]")
                .AddColumn("Contact ID")
                .AddColumn("Name")
                .AddColumn("Email")
                .AddColumn("Phone")
                .AddColumn("Company");

            foreach (var member in members.Result)
            {
                table.AddRow(
                    member.UserId.ToString(),
                    member.ContactName ?? member.ContactDisplayName ?? "",
                    member.EmailId ?? "",
                    member.ContactPhoneNo ?? "",
                    "N/A" // Company name not directly available
                );
            }

            AnsiConsole.Write(table);
            
            if (members.Count > perPage)
            {
                AnsiConsole.MarkupLine($"[dim]Showing {members.Result.Count} of {members.Count} members. Use --page to navigate.[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> AddMembersToGroupAsync(string[] args, IBoldDeskClient client)
    {
        var groupId = GetOption(args, "--id");
        var contactIds = GetOption(args, "--contacts");

        if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(contactIds))
        {
            AnsiConsole.MarkupLine("[red]Error: --id and --contacts are required[/]");
            return 1;
        }

        if (!int.TryParse(groupId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid group ID[/]");
            return 1;
        }

        try
        {
            var contactIdList = contactIds.Split(',')
                .Select(s => int.TryParse(s.Trim(), out var cid) ? cid : 0)
                .Where(cid => cid > 0)
                .ToList();

            if (!contactIdList.Any())
            {
                AnsiConsole.MarkupLine("[red]Error: No valid contact IDs provided[/]");
                return 1;
            }

            var addRequests = contactIdList.Select(contactId => new AddContactToGroupRequest
            {
                UserId = contactId,
                AccessScopeId = 1  // Default access scope
            }).ToList();

            AnsiConsole.MarkupLine($"[yellow]Adding {contactIdList.Count} members to group {id}...[/]");
            var addResp = await client.ContactGroups.AddContactsToGroupAsync(id, addRequests);
            var addOk = (addResp.Result?.IsSuccess == true)
                        || (!string.IsNullOrWhiteSpace(addResp.Message) && addResp.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (addOk)
                AnsiConsole.MarkupLine($"[green]✓ Successfully added {contactIdList.Count} members to group {id}[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to add members to group {id}: {addResp.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> RemoveMembersFromGroupAsync(string[] args, IBoldDeskClient client)
    {
        var groupId = GetOption(args, "--id");
        var contactIds = GetOption(args, "--contacts");

        if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(contactIds))
        {
            AnsiConsole.MarkupLine("[red]Error: --id and --contacts are required[/]");
            return 1;
        }

        if (!int.TryParse(groupId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid group ID[/]");
            return 1;
        }

        try
        {
            var contactIdList = contactIds.Split(',')
                .Select(s => int.TryParse(s.Trim(), out var cid) ? cid : 0)
                .Where(cid => cid > 0)
                .ToList();

            if (!contactIdList.Any())
            {
                AnsiConsole.MarkupLine("[red]Error: No valid contact IDs provided[/]");
                return 1;
            }

            AnsiConsole.MarkupLine($"[yellow]Removing {contactIdList.Count} members from group {id}...[/]");
            
            // Note: The service only supports removing one contact at a time
            int success = 0, fail = 0;
            foreach (var contactId in contactIdList)
            {
                var r = await client.ContactGroups.RemoveContactFromGroupAsync(id, contactId);
                var ok = r.IsSuccess == true
                         || (r.Id.HasValue && r.Id.Value > 0)
                         || (!string.IsNullOrWhiteSpace(r.Message) && r.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
                if (ok) success++; else fail++;
            }
            if (fail == 0)
                AnsiConsole.MarkupLine($"[green]✓ Successfully removed {success} members from group {id}[/]");
            else
                AnsiConsole.MarkupLine($"[yellow]Removed {success} members; {fail} failed for group {id}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }
}

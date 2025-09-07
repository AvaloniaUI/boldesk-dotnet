using BoldDesk;
using BoldDesk.Models;
using Spectre.Console;
using System.Text.Json;

namespace BoldDesk.Cli.Commands;

public static class ContactCommands
{
    public static async Task<int> HandleContactCommandAsync(string[] args, IBoldDeskClient client)
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
            // Core CRUD operations
            "list" => await ListContactsAsync(remainingArgs, client),
            "get" => await GetContactAsync(remainingArgs, client),
            "create" => await CreateContactAsync(remainingArgs, client),
            "update" => await UpdateContactAsync(remainingArgs, client),
            "delete" => await DeleteContactAsync(remainingArgs, client),
            "permanent-delete" => await PermanentDeleteContactAsync(remainingArgs, client),
            
            // Contact management
            "block" => await BlockContactAsync(remainingArgs, client),
            "unblock" => await UnblockContactAsync(remainingArgs, client),
            "merge" => await MergeContactsAsync(remainingArgs, client),
            "convert-to-agent" => await ConvertToAgentAsync(remainingArgs, client),
            
            // Contact groups
            "add-groups" => await AddContactGroupsAsync(remainingArgs, client),
            "remove-groups" => await RemoveContactGroupsAsync(remainingArgs, client),
            "list-groups" => await ListContactGroupsAsync(remainingArgs, client),
            "set-primary-group" => await SetPrimaryGroupAsync(remainingArgs, client),
            
            // Notes
            "notes" => await ManageNotesAsync(remainingArgs, client),
            
            // Fields
            "fields" => await ManageFieldsAsync(remainingArgs, client),
            
            "help" or "--help" or "-h" => ShowHelpAndReturn(),
            _ => ShowInvalidCommandAndReturn(subCommand)
        };
    }

    private static void ShowHelp()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]BoldDesk Contact Commands[/]").RuleStyle("grey"));
        AnsiConsole.WriteLine();
        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[blue]Command[/]")
            .AddColumn("[green]Description[/]")
            .AddColumn("Options");

        // Core operations
        table.AddRow("[cyan]Core Operations[/]", "", "");
        table.AddRow("contacts list", "List contacts", "--page <number>, --per-page <number>, --search <term>");
        table.AddRow("contacts get", "Get contact details", "--id <contactId> OR --email <email>");
        table.AddRow("contacts create", "Create a new contact", "--email <email>, --name <name>, --phone <phone>");
        table.AddRow("contacts update", "Update an existing contact", "--id <contactId>, --name <name>, --email <email>");
        table.AddRow("contacts delete", "Delete contacts (soft)", "--ids <id1,id2,id3>");
        table.AddRow("contacts permanent-delete", "Permanently delete contacts", "--ids <id1,id2,id3>");
        
        table.AddRow("", "", "");
        
        // Contact management
        table.AddRow("[cyan]Contact Management[/]", "", "");
        table.AddRow("contacts block", "Block a contact", "--id <contactId>, --mark-spam");
        table.AddRow("contacts unblock", "Unblock a contact", "--id <contactId>, --remove-spam");
        table.AddRow("contacts merge", "Merge contacts", "--primary <id>, --secondary <id1,id2>");
        table.AddRow("contacts convert-to-agent", "Convert contact to agent", "--id <contactId>, --role <roleId>");
        
        table.AddRow("", "", "");
        
        // Contact groups
        table.AddRow("[cyan]Contact Groups[/]", "", "");
        table.AddRow("contacts add-groups", "Add contact to groups", "--id <contactId>, --groups <groupId1,groupId2>");
        table.AddRow("contacts remove-groups", "Remove contact from groups", "--id <contactId>, --groups <groupId1,groupId2>");
        table.AddRow("contacts list-groups", "List contact's groups", "--id <contactId>");
        table.AddRow("contacts set-primary-group", "Set primary contact group", "--id <contactId>, --group <groupId>");
        
        table.AddRow("", "", "");
        
        // Additional features
        table.AddRow("[cyan]Additional Features[/]", "", "");
        table.AddRow("contacts notes <action>", "Manage contact notes", "list, add, update, delete");
        table.AddRow("contacts fields <action>", "Manage contact fields", "list, get");
        
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[grey]Examples:[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contacts list --search \"john\" --per-page 10[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contacts get --email john@example.com[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contacts create --email user@example.com --name \"John Doe\" --phone \"+1234567890\"[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contacts add-groups --id 123 --groups 456,789[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk contacts notes list --id 123[/]");
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

    private static async Task<int> ListContactsAsync(string[] args, IBoldDeskClient client)
    {
        try
        {
            var page = int.TryParse(GetOption(args, "--page"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page"), out var pp) ? pp : 20;
            var searchTerm = GetOption(args, "--search");
            var filter = GetOption(args, "--filter");
            var view = GetOption(args, "--view");
            var groupId = GetOption(args, "--group");

            var parameters = new ContactQueryParameters
            {
                Page = page,
                PerPage = perPage,
                Filter = searchTerm ?? filter,
                View = view,
                RequiresCounts = true
            };

            if (!string.IsNullOrEmpty(groupId) && long.TryParse(groupId, out var gId))
            {
                parameters.ContactGroupId = gId;
            }

            await AnsiConsole.Status()
                .StartAsync("Fetching contacts...", async ctx =>
                {
                    var response = await client.Contacts.ListContactsAsync(parameters);
                    
                    if (!response.Result.Any())
                    {
                        AnsiConsole.MarkupLine("[yellow]No contacts found[/]");
                        return;
                    }

                    var table = new Table()
                        .Border(TableBorder.Rounded)
                        .Title($"[yellow]Contacts (Page {page}, Total: {response.Count})[/]")
                        .AddColumn("ID")
                        .AddColumn("Name")
                        .AddColumn("Email")
                        .AddColumn("Company")
                        .AddColumn("Status")
                        .AddColumn("Created");

                    foreach (var contact in response.Result)
                    {
                        table.AddRow(
                            contact.UserId.ToString(),
                            contact.ContactName ?? contact.ContactDisplayName ?? "",
                            contact.EmailId ?? "",
                            contact.PrimaryContactGroup?.Name ?? "",
                            contact.IsBlocked ? "[red]Blocked[/]" : "[green]Active[/]",
                            contact.CreatedOn.ToString("yyyy-MM-dd")
                        );
                    }

                    AnsiConsole.Write(table);
                    
                    if (response.Count > perPage)
                    {
                        AnsiConsole.MarkupLine($"[dim]Showing {response.Result.Count} of {response.Count} contacts. Use --page to navigate.[/]");
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

    private static async Task<int> GetContactAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        var email = GetOption(args, "--email");
        
        if (string.IsNullOrEmpty(contactId) && string.IsNullOrEmpty(email))
        {
            AnsiConsole.MarkupLine("[red]Error: Either --id or --email is required[/]");
            return 1;
        }

        try
        {
            Contact contact;
            
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    contact = await client.Contacts.GetContactByEmailAsync(email);
                }
                catch (Exception emailEx)
                {
                    if (emailEx.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) || 
                        emailEx.Message.Contains("404", StringComparison.OrdinalIgnoreCase))
                    {
                        AnsiConsole.MarkupLine($"[red]Contact with email '{email}' not found[/]");
                        return 1;
                    }
                    throw; // Re-throw other exceptions
                }
            }
            else if (long.TryParse(contactId, out var id))
            {
                contact = await client.Contacts.GetContactAsync(id);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid contact ID[/]");
                return 1;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Contact Details - ID: {contact.UserId}[/]")
                .AddColumn(new TableColumn("[blue]Property[/]").Width(25))
                .AddColumn("[green]Value[/]");

            table.AddRow("ID", contact.UserId.ToString());
            table.AddRow("Name", contact.ContactName ?? "");
            table.AddRow("Display Name", contact.ContactDisplayName ?? "");
            table.AddRow("Email", contact.EmailId ?? "");
            table.AddRow("Company", contact.PrimaryContactGroup?.Name ?? "");
            table.AddRow("Phone", contact.ContactPhoneNo ?? "");
            table.AddRow("Mobile", contact.ContactMobileNo ?? "");
            table.AddRow("Job Title", contact.ContactJobTitle ?? "");
            table.AddRow("Status", contact.IsBlocked ? "[red]Blocked[/]" : "[green]Active[/]");
            table.AddRow("Verified", contact.IsVerified ? "[green]Yes[/]" : "[grey]No[/]");
            table.AddRow("Created On", contact.CreatedOn.ToString("yyyy-MM-dd HH:mm"));
            table.AddRow("Last Activity", contact.LastActivityOn.HasValue ? contact.LastActivityOn.Value.ToString("yyyy-MM-dd HH:mm") : "-");
            table.AddRow("Time Zone", contact.TimeZoneId?.Name ?? "Default");
            table.AddRow("Language", contact.LanguageId?.Name ?? "Default");
            
            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> CreateContactAsync(string[] args, IBoldDeskClient client)
    {
        var email = GetOption(args, "--email");
        var name = GetOption(args, "--name");
        
        if (string.IsNullOrEmpty(email))
        {
            AnsiConsole.MarkupLine("[red]Error: --email is required[/]");
            return 1;
        }

        try
        {
            var request = new CreateContactRequest
            {
                EmailId = email,
                ContactName = name ?? "",
                ContactDisplayName = GetOption(args, "--display-name") ?? name ?? "",
                ContactPhoneNo = GetOption(args, "--phone"),
                ContactMobileNo = GetOption(args, "--mobile"),
                ContactJobTitle = GetOption(args, "--job-title"),
                TimeZoneId = int.TryParse(GetOption(args, "--timezone-id"), out var tzId) ? tzId : null,
                LanguageId = int.TryParse(GetOption(args, "--language-id"), out var langId) ? langId : null
            };

            AnsiConsole.MarkupLine("[yellow]Creating contact...[/]");
            var response = await client.Contacts.CreateContactAsync(request);
            var isSuccess = (response.Id > 0)
                            || (!string.IsNullOrWhiteSpace(response.Message) && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (isSuccess)
            {
                AnsiConsole.MarkupLine($"[green]✓ Contact created successfully with ID: {response.Id}[/]");
            }
            else
            {
                var msg = string.IsNullOrWhiteSpace(response.Message) ? "Unknown error" : response.Message;
                AnsiConsole.MarkupLine($"[red]Failed to create contact: {msg}[/]");
                return 1;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> UpdateContactAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(contactId) || !long.TryParse(contactId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --id is required[/]");
            return 1;
        }

        try
        {
            var fields = new Dictionary<string, object>();
            
            var name = GetOption(args, "--name");
            var displayName = GetOption(args, "--display-name");
            var email = GetOption(args, "--email");
            var phone = GetOption(args, "--phone");
            var mobile = GetOption(args, "--mobile");
            var jobTitle = GetOption(args, "--job-title");
            var company = GetOption(args, "--company");
            var timezone = GetOption(args, "--timezone");
            var language = GetOption(args, "--language");
            
            if (!string.IsNullOrEmpty(name)) fields["contactName"] = name;
            if (!string.IsNullOrEmpty(displayName)) fields["contactDisplayName"] = displayName;
            if (!string.IsNullOrEmpty(email)) fields["emailId"] = email;
            if (!string.IsNullOrEmpty(phone)) fields["contactPhoneNo"] = phone;
            if (!string.IsNullOrEmpty(mobile)) fields["contactMobileNo"] = mobile;
            if (!string.IsNullOrEmpty(jobTitle)) fields["contactJobTitle"] = jobTitle;
            if (!string.IsNullOrEmpty(company)) fields["companyName"] = company;
            if (!string.IsNullOrEmpty(timezone)) fields["contactTimeZone"] = timezone;
            if (!string.IsNullOrEmpty(language)) fields["contactLanguage"] = language;
            
            if (HasFlag(args, "--verify")) fields["isVerified"] = true;
            else if (HasFlag(args, "--unverify")) fields["isVerified"] = false;
            
            var request = new UpdateContactRequest { Fields = fields };

            AnsiConsole.MarkupLine($"[yellow]Updating contact {id}...[/]");
            var response = await client.Contacts.UpdateContactAsync(id, request);
            var ok = string.IsNullOrWhiteSpace(response.Message) || response.Message.Contains("success", StringComparison.OrdinalIgnoreCase);
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Contact {id} updated successfully[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to update contact {id}: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> DeleteContactAsync(string[] args, IBoldDeskClient client)
    {
        var contactIds = GetOption(args, "--ids");
        if (string.IsNullOrEmpty(contactIds))
        {
            AnsiConsole.MarkupLine("[red]Error: --ids is required (comma-separated list)[/]");
            return 1;
        }

        try
        {
            var idList = contactIds.Split(',')
                .Select(s => long.TryParse(s.Trim(), out var id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            if (!idList.Any())
            {
                AnsiConsole.MarkupLine("[red]Error: No valid contact IDs provided[/]");
                return 1;
            }

            if (!HasFlag(args, "--force"))
            {
                if (!AnsiConsole.Confirm($"Are you sure you want to delete {idList.Count} contact(s)?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            var request = new DeleteContactRequest
            {
                ContactId = idList.ToArray()
            };

            AnsiConsole.MarkupLine($"[yellow]Deleting {idList.Count} contacts...[/]");
            var response = await client.Contacts.DeleteContactsAsync(request);
            var ok = (response.Result?.IsSuccess == true)
                     || (!string.IsNullOrWhiteSpace(response.Message) && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Successfully deleted {idList.Count} contacts[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to delete contacts: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> PermanentDeleteContactAsync(string[] args, IBoldDeskClient client)
    {
        var contactIds = GetOption(args, "--ids");
        if (string.IsNullOrEmpty(contactIds))
        {
            AnsiConsole.MarkupLine("[red]Error: --ids is required (comma-separated list)[/]");
            return 1;
        }

        try
        {
            var idList = contactIds.Split(',')
                .Select(s => long.TryParse(s.Trim(), out var id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            if (!idList.Any())
            {
                AnsiConsole.MarkupLine("[red]Error: No valid contact IDs provided[/]");
                return 1;
            }

            if (!HasFlag(args, "--force"))
            {
                AnsiConsole.MarkupLine("[red]WARNING: This will permanently delete contacts and cannot be undone![/]");
                if (!AnsiConsole.Confirm($"Are you sure you want to PERMANENTLY delete {idList.Count} contact(s)?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            var request = new PermanentDeleteContactRequest
            {
                ContactIdList = idList.ToArray()
            };

            AnsiConsole.MarkupLine($"[yellow]Permanently deleting {idList.Count} contacts...[/]");
            var response = await client.Contacts.PermanentDeleteContactsAsync(request);
            var ok = (response.Result?.IsSuccess == true)
                     || (!string.IsNullOrWhiteSpace(response.Message) && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Successfully permanently deleted {idList.Count} contacts[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to permanently delete contacts: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> BlockContactAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(contactId) || !long.TryParse(contactId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --id is required[/]");
            return 1;
        }

        try
        {
            var markAsSpam = HasFlag(args, "--mark-spam");
            
            AnsiConsole.MarkupLine($"[yellow]Blocking contact {id}...[/]");
            var response = await client.Contacts.BlockContactAsync(id, markAsSpam);
            var ok = string.IsNullOrWhiteSpace(response.Message) || response.Message.Contains("success", StringComparison.OrdinalIgnoreCase);
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Contact {id} blocked successfully{(markAsSpam ? " and marked as spam" : "")}[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to block contact {id}: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> UnblockContactAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(contactId) || !long.TryParse(contactId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --id is required[/]");
            return 1;
        }

        try
        {
            var removeFromSpam = HasFlag(args, "--remove-spam");
            
            AnsiConsole.MarkupLine($"[yellow]Unblocking contact {id}...[/]");
            var response = await client.Contacts.UnblockContactAsync(id, removeFromSpam);
            var ok = string.IsNullOrWhiteSpace(response.Message) || response.Message.Contains("success", StringComparison.OrdinalIgnoreCase);
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Contact {id} unblocked successfully{(removeFromSpam ? " and removed from spam" : "")}[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to unblock contact {id}: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> MergeContactsAsync(string[] args, IBoldDeskClient client)
    {
        var primaryId = GetOption(args, "--primary");
        var secondaryIds = GetOption(args, "--secondary");
        
        if (string.IsNullOrEmpty(primaryId) || string.IsNullOrEmpty(secondaryIds))
        {
            AnsiConsole.MarkupLine("[red]Error: Both --primary and --secondary are required[/]");
            return 1;
        }

        try
        {
            if (!long.TryParse(primaryId, out var pId))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid primary contact ID[/]");
                return 1;
            }

            var secondaryIdList = secondaryIds.Split(',')
                .Select(s => long.TryParse(s.Trim(), out var id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            if (!secondaryIdList.Any())
            {
                AnsiConsole.MarkupLine("[red]Error: No valid secondary contact IDs provided[/]");
                return 1;
            }

            var request = new MergeContactRequest
            {
                PrimaryContactId = pId,
                SecondaryContactIdList = secondaryIdList.ToArray()
            };

            if (!HasFlag(args, "--force"))
            {
                AnsiConsole.MarkupLine($"[yellow]This will merge {secondaryIdList.Count} contacts into contact {pId}[/]");
                if (!AnsiConsole.Confirm("Are you sure you want to merge these contacts?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            AnsiConsole.MarkupLine($"[yellow]Merging contacts into {pId}...[/]");
            var response = await client.Contacts.MergeContactsAsync(request);
            var ok = (response.Result?.IsSuccess == true)
                     || (!string.IsNullOrWhiteSpace(response.Message) && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Successfully merged {secondaryIdList.Count} contacts into {pId}[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to merge contacts: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> ConvertToAgentAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        var roleId = GetOption(args, "--role");
        
        if (string.IsNullOrEmpty(contactId) || !long.TryParse(contactId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --id is required[/]");
            return 1;
        }

        try
        {
            var request = new ConvertContactToAgentRequest
            {
                RoleIds = roleId ?? "1" // Default role ID
            };

            if (!HasFlag(args, "--force"))
            {
                if (!AnsiConsole.Confirm($"Are you sure you want to convert contact {id} to an agent?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            AnsiConsole.MarkupLine($"[yellow]Converting contact {id} to agent...[/]");
            var response = await client.Contacts.ConvertContactToAgentAsync(id, request);
            var ok = string.IsNullOrWhiteSpace(response.Message) || response.Message.Contains("success", StringComparison.OrdinalIgnoreCase);
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Contact {id} converted to agent successfully[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to convert contact {id} to agent: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    // Contact Groups Management
    private static async Task<int> AddContactGroupsAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        var groupIds = GetOption(args, "--groups");
        
        if (string.IsNullOrEmpty(contactId) || string.IsNullOrEmpty(groupIds))
        {
            AnsiConsole.MarkupLine("[red]Error: Both --id and --groups are required[/]");
            return 1;
        }

        try
        {
            if (!long.TryParse(contactId, out var id))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid contact ID[/]");
                return 1;
            }

            var groupIdList = groupIds.Split(',')
                .Select(s => long.TryParse(s.Trim(), out var gid) ? gid : 0)
                .Where(gid => gid > 0)
                .ToList();

            if (!groupIdList.Any())
            {
                AnsiConsole.MarkupLine("[red]Error: No valid group IDs provided[/]");
                return 1;
            }

            var requests = groupIdList.Select(gId => new AddContactGroupsRequest
            {
                ContactGroupId = gId,
                AccessScopeId = 1 // Default access scope
            }).ToList();

            AnsiConsole.MarkupLine($"[yellow]Adding contact {id} to {groupIdList.Count} groups...[/]");
            var response = await client.Contacts.AddContactGroupsAsync(id, requests);
            var ok = (response.Result.IsSuccess)
                     || (!string.IsNullOrWhiteSpace(response.Message) && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Successfully added contact to {groupIdList.Count} groups[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to add groups: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> RemoveContactGroupsAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        var groupIds = GetOption(args, "--groups");
        
        if (string.IsNullOrEmpty(contactId) || string.IsNullOrEmpty(groupIds))
        {
            AnsiConsole.MarkupLine("[red]Error: Both --id and --groups are required[/]");
            return 1;
        }

        try
        {
            if (!long.TryParse(contactId, out var id))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid contact ID[/]");
                return 1;
            }

            var groupIdList = groupIds.Split(',')
                .Select(s => long.TryParse(s.Trim(), out var gid) ? gid : 0)
                .Where(gid => gid > 0)
                .ToList();

            if (!groupIdList.Any())
            {
                AnsiConsole.MarkupLine("[red]Error: No valid group IDs provided[/]");
                return 1;
            }

            var request = new RemoveContactGroupsRequest
            {
                ContactGroupIds = groupIdList.ToArray()
            };

            AnsiConsole.MarkupLine($"[yellow]Removing contact {id} from {groupIdList.Count} groups...[/]");
            var response = await client.Contacts.RemoveContactGroupsAsync(id, request);
            var ok = (response.Result.Any(r => r.IsSuccess))
                     || (!string.IsNullOrWhiteSpace(response.Message) && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Successfully removed contact from {groupIdList.Count} groups[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to remove from groups: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> ListContactGroupsAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(contactId) || !long.TryParse(contactId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --id is required[/]");
            return 1;
        }

        try
        {
            var page = int.TryParse(GetOption(args, "--page"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page"), out var pp) ? pp : 20;

            var parameters = new ContactGroupQueryParameters
            {
                Page = page,
                PerPage = perPage
            };

            var groups = await client.Contacts.GetContactGroupsByContactIdAsync(id, parameters);
            
            if (!groups.Result.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]No groups found for contact {id}[/]");
                return 0;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Contact {id} Groups (Total: {groups.Count})[/]")
                .AddColumn("Group ID")
                .AddColumn("Name")
                .AddColumn("Description");

            foreach (var group in groups.Result)
            {
                table.AddRow(
                    group.Id.ToString(),
                    group.Name ?? "",
                    group.AccessScopeId.ToString()
                );
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> SetPrimaryGroupAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        var groupId = GetOption(args, "--group");
        
        if (string.IsNullOrEmpty(contactId) || string.IsNullOrEmpty(groupId))
        {
            AnsiConsole.MarkupLine("[red]Error: Both --id and --group are required[/]");
            return 1;
        }

        try
        {
            if (!long.TryParse(contactId, out var cId) || !long.TryParse(groupId, out var gId))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid contact ID or group ID[/]");
                return 1;
            }

            AnsiConsole.MarkupLine($"[yellow]Setting primary group for contact {cId} to {gId}...[/]");
            var response = await client.Contacts.ChangePrimaryContactGroupAsync(cId, gId);
            
            AnsiConsole.MarkupLine($"[green]✓ Successfully set primary group for contact {cId}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    // Notes Management
    private static async Task<int> ManageNotesAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Notes action is required (list, add, update, delete)[/]");
            return 1;
        }

        var action = args[0].ToLowerInvariant();
        var remainingArgs = args.Skip(1).ToArray();

        return action switch
        {
            "list" => await ListContactNotesAsync(remainingArgs, client),
            "add" => await AddContactNoteAsync(remainingArgs, client),
            "update" => await UpdateContactNoteAsync(remainingArgs, client),
            "delete" => await DeleteContactNoteAsync(remainingArgs, client),
            _ => ShowInvalidNoteActionAndReturn(action)
        };
    }

    private static int ShowInvalidNoteActionAndReturn(string action)
    {
        AnsiConsole.MarkupLine($"[red]Invalid notes action: {action}[/]");
        AnsiConsole.MarkupLine("Available actions: list, add, update, delete");
        return 1;
    }

    private static async Task<int> ListContactNotesAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(contactId) || !long.TryParse(contactId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --id is required[/]");
            return 1;
        }

        try
        {
            var page = int.TryParse(GetOption(args, "--page"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page"), out var pp) ? pp : 20;

            var parameters = new ContactQueryParameters
            {
                Page = page,
                PerPage = perPage
            };

            var notes = await client.Contacts.ListContactNotesAsync(id, parameters);
            
            if (!notes.ContactNoteObjects.Any())
            {
                AnsiConsole.MarkupLine($"[yellow]No notes found for contact {id}[/]");
                return 0;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Contact {id} Notes (Total: {notes.TotalListCount})[/]")
                .AddColumn("Note ID")
                .AddColumn("Subject")
                .AddColumn("Content")
                .AddColumn("Created")
                .AddColumn("Author");

            foreach (var note in notes.ContactNoteObjects)
            {
                table.AddRow(
                    note.Id.ToString(),
                    note.Subject ?? "",
                    (note.Description?.Length > 50 ? note.Description.Substring(0, 47) + "..." : note.Description) ?? "",
                    note.CreatedOn.ToString("yyyy-MM-dd HH:mm"),
                    note.CreatedBy?.Name ?? "System"
                );
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> AddContactNoteAsync(string[] args, IBoldDeskClient client)
    {
        var contactId = GetOption(args, "--id");
        var subject = GetOption(args, "--subject");
        var description = GetOption(args, "--description");
        
        if (string.IsNullOrEmpty(contactId) || !long.TryParse(contactId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --id is required[/]");
            return 1;
        }

        if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(description))
        {
            AnsiConsole.MarkupLine("[red]Error: Both --subject and --description are required[/]");
            return 1;
        }

        try
        {
            var request = new ContactNoteRequest
            {
                Subject = subject,
                Description = description
            };

            AnsiConsole.MarkupLine($"[yellow]Adding note to contact {id}...[/]");
            var response = await client.Contacts.AddContactNoteAsync(id, request);
            var ok = (response.Id > 0) || (!string.IsNullOrWhiteSpace(response.Message) && response.Message.Contains("success", StringComparison.OrdinalIgnoreCase));
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Note added successfully to contact {id}[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to add note to contact {id}: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> UpdateContactNoteAsync(string[] args, IBoldDeskClient client)
    {
        var noteId = GetOption(args, "--note-id");
        var subject = GetOption(args, "--subject");
        var description = GetOption(args, "--description");
        
        if (string.IsNullOrEmpty(noteId) || !long.TryParse(noteId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --note-id is required[/]");
            return 1;
        }

        try
        {
            var request = new ContactNoteRequest
            {
                Subject = subject,
                Description = description
            };

            AnsiConsole.MarkupLine($"[yellow]Updating note {id}...[/]");
            var response = await client.Contacts.UpdateContactNoteAsync(id, request);
            var ok = string.IsNullOrWhiteSpace(response.Message) || response.Message.Contains("success", StringComparison.OrdinalIgnoreCase);
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Note {id} updated successfully[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to update note {id}: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> DeleteContactNoteAsync(string[] args, IBoldDeskClient client)
    {
        var noteId = GetOption(args, "--note-id");
        
        if (string.IsNullOrEmpty(noteId) || !long.TryParse(noteId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --note-id is required[/]");
            return 1;
        }

        try
        {
            if (!HasFlag(args, "--force"))
            {
                if (!AnsiConsole.Confirm($"Are you sure you want to delete note {id}?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            AnsiConsole.MarkupLine($"[yellow]Deleting note {id}...[/]");
            var response = await client.Contacts.DeleteContactNoteAsync(id);
            var ok = string.IsNullOrWhiteSpace(response.Message) || response.Message.Contains("success", StringComparison.OrdinalIgnoreCase);
            if (ok)
                AnsiConsole.MarkupLine($"[green]✓ Note {id} deleted successfully[/]");
            else
                AnsiConsole.MarkupLine($"[red]Failed to delete note {id}: {response.Message}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    // Fields Management
    private static async Task<int> ManageFieldsAsync(string[] args, IBoldDeskClient client)
    {
        if (args.Length == 0)
        {
            AnsiConsole.MarkupLine("[red]Error: Fields action is required (list, get)[/]");
            return 1;
        }

        var action = args[0].ToLowerInvariant();
        var remainingArgs = args.Skip(1).ToArray();

        return action switch
        {
            "list" => await ListContactFieldsAsync(remainingArgs, client),
            "get" => await GetContactFieldAsync(remainingArgs, client),
            _ => ShowInvalidFieldActionAndReturn(action)
        };
    }

    private static int ShowInvalidFieldActionAndReturn(string action)
    {
        AnsiConsole.MarkupLine($"[red]Invalid fields action: {action}[/]");
        AnsiConsole.MarkupLine("Available actions: list, get");
        return 1;
    }

    private static async Task<int> ListContactFieldsAsync(string[] args, IBoldDeskClient client)
    {
        try
        {
            var page = int.TryParse(GetOption(args, "--page"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page"), out var pp) ? pp : 20;

            var parameters = new ContactQueryParameters
            {
                Page = page,
                PerPage = perPage
            };

            var fields = await client.Contacts.ListContactFieldsAsync(parameters);
            
            if (!fields.Result.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No contact fields found[/]");
                return 0;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Contact Fields (Total: {fields.Count})[/]")
                .AddColumn("Field ID")
                .AddColumn("Name")
                .AddColumn("Type")
                .AddColumn("System")
                .AddColumn("Status");

            foreach (var field in fields.Result)
            {
                table.AddRow(
                    field.FieldId.ToString(),
                    field.LabelForAgentPortal ?? field.ApiName ?? "",
                    field.FieldType ?? "",
                    field.IsDefaultSystemField ? "[yellow]System[/]" : "[green]Custom[/]",
                    field.IsActive ? "[green]Active[/]" : "[red]Inactive[/]"
                );
            }

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> GetContactFieldAsync(string[] args, IBoldDeskClient client)
    {
        var fieldId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(fieldId) || !int.TryParse(fieldId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --id is required[/]");
            return 1;
        }

        try
        {
            var field = await client.Contacts.GetContactFieldAsync(id);
            
            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Contact Field Details - ID: {id}[/]")
                .AddColumn(new TableColumn("[blue]Property[/]").Width(20))
                .AddColumn("[green]Value[/]");

            table.AddRow("Field ID", field.FieldId.ToString());
            table.AddRow("Name", field.LabelForAgentPortal ?? field.ApiName ?? "");
            table.AddRow("API Name", field.ApiName ?? "");
            table.AddRow("Type", field.FieldType ?? "");
            table.AddRow("System Field", field.IsDefaultSystemField ? "[yellow]Yes[/]" : "[green]No[/]");
            table.AddRow("Active", field.IsActive ? "[green]Yes[/]" : "[red]No[/]");
            
            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }
}

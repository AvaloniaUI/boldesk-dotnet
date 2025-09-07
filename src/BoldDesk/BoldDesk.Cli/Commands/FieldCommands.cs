using BoldDesk;
using BoldDesk.Models;
using Spectre.Console;
using System.Text.Json;

namespace BoldDesk.Cli.Commands;

public static class FieldCommands
{
    public static async Task<int> HandleFieldCommandAsync(string[] args, IBoldDeskClient client)
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
            // Field option operations
            "list-options" => await ListFieldOptionsAsync(remainingArgs, client),
            "add-options" => await AddFieldOptionsAsync(remainingArgs, client),
            "remove-option" => await RemoveFieldOptionAsync(remainingArgs, client),
            
            // Field option management
            "set-readonly" => await SetFieldOptionReadOnlyAsync(remainingArgs, client),
            "set-default" => await SetDefaultFieldOptionAsync(remainingArgs, client),
            "remove-default" => await RemoveDefaultFieldOptionAsync(remainingArgs, client),
            "change-position" => await ChangeFieldOptionPositionAsync(remainingArgs, client),
            
            // Help
            "help" or "--help" or "-h" => ShowHelpAndReturn(),
            _ => ShowInvalidCommandAndReturn(subCommand)
        };
    }

    private static void ShowHelp()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]BoldDesk Field Commands[/]").RuleStyle("grey"));
        AnsiConsole.WriteLine();
        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[blue]Command[/]")
            .AddColumn("[green]Description[/]")
            .AddColumn("Options");

        // Field option operations
        table.AddRow("fields list-options", "List field options for a field", "--field <apiName>, --filter, --parent-id, --page, --per-page, --order, --format, --include-readonly");
        table.AddRow("fields add-options", "Add new field options", "--field <apiName>, --options <\"opt1,opt2,opt3\">");
        table.AddRow("fields remove-option", "Remove a field option", "--option-id <id>");
        table.AddEmptyRow();
        
        // Field option management
        table.AddRow("fields set-readonly", "Set field option read-only status", "--option-id <id>, --readonly <true|false>");
        table.AddRow("fields set-default", "Set field option as default", "--field-id <id>, --option-id <id>");
        table.AddRow("fields remove-default", "Remove field option as default", "--field-id <id>, --option-id <id>");
        table.AddRow("fields change-position", "Change field option position", "--field-id <id>, --option-id <id>, --position <num>, --top, --bottom, --alphabetical");
        
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[bold]Common Options:[/]");
        AnsiConsole.MarkupLine("  --field <apiName>      Field API name (e.g., 'cf_priority', 'cf_category')");
        AnsiConsole.MarkupLine("  --field-id <number>    Numeric field ID");
        AnsiConsole.MarkupLine("  --option-id <number>   Field option ID");
        AnsiConsole.MarkupLine("  --page, -p <number>    Page number (default: 1)");
        AnsiConsole.MarkupLine("  --per-page, -n <number> Items per page (default: 50)");
        AnsiConsole.MarkupLine("  --format <json|table>  Output format (default: table)");
        AnsiConsole.MarkupLine("  --filter <text>        Filter options by name");
        AnsiConsole.MarkupLine("  --parent-id <number>   Parent option ID for hierarchical fields");
        AnsiConsole.MarkupLine("  --order <field direction> Sort order (e.g., 'name asc', 'sortOrder desc')");
        AnsiConsole.MarkupLine("  --include-readonly     Include read-only options");
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[bold]Position Options:[/]");
        AnsiConsole.MarkupLine("  --position <number>    Move to specific position");
        AnsiConsole.MarkupLine("  --top                  Move to top");
        AnsiConsole.MarkupLine("  --bottom               Move to bottom");
        AnsiConsole.MarkupLine("  --alphabetical         Sort alphabetically");
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[grey]Examples:[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk fields list-options --field cf_priority[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk fields add-options --field cf_category --options \"Bug,Feature,Enhancement\"[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk fields remove-option --option-id 12345[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk fields set-default --field-id 100 --option-id 12345[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk fields change-position --field-id 100 --option-id 12345 --position 1[/]");
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

    private static async Task<int> ListFieldOptionsAsync(string[] args, IBoldDeskClient client)
    {
        var fieldApiName = GetOption(args, "--field");
        if (string.IsNullOrEmpty(fieldApiName))
        {
            AnsiConsole.MarkupLine("[red]Error: --field is required[/]");
            return 1;
        }

        try
        {
            var page = int.TryParse(GetOption(args, "--page") ?? GetOption(args, "-p"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page") ?? GetOption(args, "-n"), out var pp) ? Math.Min(pp, 100) : 50;
            var format = GetOption(args, "--format") ?? "table";
            var filter = GetOption(args, "--filter");
            var order = GetOption(args, "--order");
            var includeReadOnly = HasFlag(args, "--include-readonly");
            
            int? parentId = null;
            if (int.TryParse(GetOption(args, "--parent-id"), out var pid)) 
                parentId = pid;

            var parameters = new FieldOptionQueryParameters
            {
                Page = page,
                PerPage = perPage,
                Filter = filter,
                ParentOptionId = parentId,
                OrderBy = order,
                RequiresCounts = true,
                IncludeReadOnlyAlso = includeReadOnly
            };

            var response = await client.Fields.ListFieldOptionsAsync(fieldApiName, parameters);

            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                var json = JsonSerializer.Serialize(response.Result, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
            else
            {
                if (!response.Result.Any())
                {
                    AnsiConsole.MarkupLine($"[yellow]No options found for field '{fieldApiName}'[/]");
                    return 0;
                }

                RenderFieldOptionsTable(fieldApiName, response.Result);
                
                AnsiConsole.MarkupLine($"[grey]Page {page}, showing {response.Result.Count} options[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> AddFieldOptionsAsync(string[] args, IBoldDeskClient client)
    {
        var fieldApiName = GetOption(args, "--field");
        var optionsStr = GetOption(args, "--options");
        
        if (string.IsNullOrEmpty(fieldApiName))
        {
            AnsiConsole.MarkupLine("[red]Error: --field is required[/]");
            return 1;
        }
        
        if (string.IsNullOrEmpty(optionsStr))
        {
            AnsiConsole.MarkupLine("[red]Error: --options is required[/]");
            return 1;
        }

        try
        {
            var options = optionsStr.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            
            if (!options.Any())
            {
                AnsiConsole.MarkupLine("[red]Error: No valid options provided[/]");
                return 1;
            }

            AnsiConsole.MarkupLine($"[yellow]Adding {options.Count} options to field '{fieldApiName}'...[/]");
            var response = await client.Fields.AddFieldOptionsAsync(fieldApiName, options);
            
            if (!string.IsNullOrEmpty(response.Message))
            {
                if (response.Message.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[green]✓ Successfully added options: {string.Join(", ", options)}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Response: {response.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]✓ Options added successfully[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> RemoveFieldOptionAsync(string[] args, IBoldDeskClient client)
    {
        var optionIdStr = GetOption(args, "--option-id");
        
        if (string.IsNullOrEmpty(optionIdStr) || !long.TryParse(optionIdStr, out var optionId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --option-id is required[/]");
            return 1;
        }

        try
        {
            if (!HasFlag(args, "--force"))
            {
                if (!AnsiConsole.Confirm($"Are you sure you want to remove field option {optionId}?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            AnsiConsole.MarkupLine($"[yellow]Removing field option {optionId}...[/]");
            var response = await client.Fields.RemoveFieldOptionAsync(optionId);
            
            if (!string.IsNullOrEmpty(response.Message))
            {
                if (response.Message.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[green]✓ Field option {optionId} removed successfully[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Response: {response.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]✓ Field option removed[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> SetFieldOptionReadOnlyAsync(string[] args, IBoldDeskClient client)
    {
        var optionIdStr = GetOption(args, "--option-id");
        var readOnlyStr = GetOption(args, "--readonly");
        
        if (string.IsNullOrEmpty(optionIdStr) || !long.TryParse(optionIdStr, out var optionId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --option-id is required[/]");
            return 1;
        }
        
        if (string.IsNullOrEmpty(readOnlyStr) || !bool.TryParse(readOnlyStr, out var isReadOnly))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --readonly (true|false) is required[/]");
            return 1;
        }

        try
        {
            var action = isReadOnly ? "read-only" : "editable";
            AnsiConsole.MarkupLine($"[yellow]Setting field option {optionId} as {action}...[/]");
            
            var response = await client.Fields.SetFieldOptionReadOnlyAsync(optionId, isReadOnly);
            
            if (!string.IsNullOrEmpty(response.Message))
            {
                if (response.Message.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[green]✓ Field option {optionId} is now {action}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Response: {response.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]✓ Field option updated[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> SetDefaultFieldOptionAsync(string[] args, IBoldDeskClient client)
    {
        var fieldIdStr = GetOption(args, "--field-id");
        var optionIdStr = GetOption(args, "--option-id");
        
        if (string.IsNullOrEmpty(fieldIdStr) || !int.TryParse(fieldIdStr, out var fieldId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --field-id is required[/]");
            return 1;
        }
        
        if (string.IsNullOrEmpty(optionIdStr) || !long.TryParse(optionIdStr, out var optionId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --option-id is required[/]");
            return 1;
        }

        try
        {
            AnsiConsole.MarkupLine($"[yellow]Setting option {optionId} as default for field {fieldId}...[/]");
            var response = await client.Fields.SetDefaultFieldOptionAsync(fieldId, optionId);
            
            if (!string.IsNullOrEmpty(response.Message))
            {
                if (response.Message.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[green]✓ Option {optionId} is now the default for field {fieldId}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Response: {response.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]✓ Default option set[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> RemoveDefaultFieldOptionAsync(string[] args, IBoldDeskClient client)
    {
        var fieldIdStr = GetOption(args, "--field-id");
        var optionIdStr = GetOption(args, "--option-id");
        
        if (string.IsNullOrEmpty(fieldIdStr) || !int.TryParse(fieldIdStr, out var fieldId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --field-id is required[/]");
            return 1;
        }
        
        if (string.IsNullOrEmpty(optionIdStr) || !long.TryParse(optionIdStr, out var optionId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --option-id is required[/]");
            return 1;
        }

        try
        {
            AnsiConsole.MarkupLine($"[yellow]Removing option {optionId} as default for field {fieldId}...[/]");
            var response = await client.Fields.RemoveDefaultFieldOptionAsync(fieldId, optionId);
            
            if (!string.IsNullOrEmpty(response.Message))
            {
                if (response.Message.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[green]✓ Option {optionId} is no longer the default for field {fieldId}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Response: {response.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]✓ Default option removed[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> ChangeFieldOptionPositionAsync(string[] args, IBoldDeskClient client)
    {
        var fieldIdStr = GetOption(args, "--field-id");
        var optionIdStr = GetOption(args, "--option-id");
        
        if (string.IsNullOrEmpty(fieldIdStr) || !int.TryParse(fieldIdStr, out var fieldId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --field-id is required[/]");
            return 1;
        }
        
        if (string.IsNullOrEmpty(optionIdStr) || !long.TryParse(optionIdStr, out var optionId))
        {
            AnsiConsole.MarkupLine("[red]Error: Valid --option-id is required[/]");
            return 1;
        }

        try
        {
            var parameters = new FieldPositionChangeParameters();
            
            // Determine positioning strategy
            if (HasFlag(args, "--top"))
            {
                parameters.IsMoveToTopPosition = true;
                AnsiConsole.MarkupLine($"[yellow]Moving option {optionId} to top position...[/]");
            }
            else if (HasFlag(args, "--bottom"))
            {
                parameters.IsMoveToBottomPosition = true;
                AnsiConsole.MarkupLine($"[yellow]Moving option {optionId} to bottom position...[/]");
            }
            else if (HasFlag(args, "--alphabetical"))
            {
                parameters.IsSortByAlphabeticalOrder = true;
                AnsiConsole.MarkupLine($"[yellow]Sorting options alphabetically...[/]");
            }
            else if (int.TryParse(GetOption(args, "--position"), out var position))
            {
                parameters.ToPosition = position;
                AnsiConsole.MarkupLine($"[yellow]Moving option {optionId} to position {position}...[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error: Must specify --position <num>, --top, --bottom, or --alphabetical[/]");
                return 1;
            }

            var response = await client.Fields.ChangeFieldOptionPositionAsync(fieldId, optionId, parameters);
            
            if (!string.IsNullOrEmpty(response.Message))
            {
                if (response.Message.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[green]✓ Option position changed successfully[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Response: {response.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[green]✓ Option position updated[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static void RenderFieldOptionsTable(string fieldApiName, IEnumerable<FieldOption> options)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title($"[yellow]Field Options: {fieldApiName}[/]")
            .AddColumn("ID")
            .AddColumn("Name")
            .AddColumn("Position")
            .AddColumn("Status")
            .AddColumn("Properties");

        foreach (var option in options.Take(50))
        {
            var status = new List<string>();
            if (option.IsDefault) status.Add("[green]Default[/]");
            if (option.IsReadOnly) status.Add("[yellow]ReadOnly[/]");
            if (option.IsPrivate) status.Add("[red]Private[/]");
            if (option.IsSystemDefault) status.Add("[blue]System[/]");
            if (!status.Any()) status.Add("[grey]Normal[/]");

            var properties = new List<string>();
            if (option.CanDelete) properties.Add("✓ Deletable");
            if (option.ParentOptionId?.Any() == true) 
                properties.Add($"Parent: {string.Join(",", option.ParentOptionId)}");
            
            table.AddRow(
                option.Id.ToString(),
                option.Name.Length > 30 ? option.Name[..27] + "..." : option.Name,
                option.SortOrder.ToString(),
                string.Join(" ", status),
                properties.Any() ? string.Join(", ", properties) : "-"
            );
        }

        AnsiConsole.Write(table);
    }
}
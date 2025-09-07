using BoldDesk;
using BoldDesk.Models;
using Spectre.Console;
using System.Text.Json;

namespace BoldDesk.Cli.Commands;

public static class WorklogCommands
{
    public static async Task<int> HandleWorklogCommandAsync(string[] args, IBoldDeskClient client)
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
            // Core operations
            "list" => await ListWorklogsAsync(remainingArgs, client),
            "count" => await GetWorklogCountAsync(remainingArgs, client),
            
            // Utility commands
            "export" => await ExportWorklogsAsync(remainingArgs, client),
            
            // Help
            "help" or "--help" or "-h" => ShowHelpAndReturn(),
            _ => ShowInvalidCommandAndReturn(subCommand)
        };
    }

    private static void ShowHelp()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]BoldDesk Worklog Commands[/]").RuleStyle("grey"));
        AnsiConsole.WriteLine();
        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[blue]Command[/]")
            .AddColumn("[green]Description[/]")
            .AddColumn("Options");

        // Core operations
        table.AddRow("worklogs list", "List worklogs with filtering", "--page, --per-page, --from, --to, --created-from, --created-to, --include-deleted, --order, --format, --all");
        table.AddRow("worklogs count", "Get worklog count", "--from, --to, --created-from, --created-to, --include-deleted");
        table.AddEmptyRow();
        
        // Utility operations
        table.AddRow("worklogs export", "Export worklogs to file", "--output, --format, --from, --to, --all");
        
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[bold]Date Filtering:[/]");
        AnsiConsole.MarkupLine("  --from, --to           Filter by last updated date (ISO format: 2024-01-01 or 2024-01-01T10:30:00)");
        AnsiConsole.MarkupLine("  --created-from, --created-to  Filter by created date");
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[bold]Common Options:[/]");
        AnsiConsole.MarkupLine("  --page, -p <number>    Page number (default: 1)");
        AnsiConsole.MarkupLine("  --per-page, -n <number> Items per page (default: 50, max: 100)");
        AnsiConsole.MarkupLine("  --format <json|table>  Output format (default: table)");
        AnsiConsole.MarkupLine("  --order <field direction>  Sort order (e.g., 'createdOn desc', 'timeSpent asc')");
        AnsiConsole.MarkupLine("  --include-deleted      Include deleted worklogs");
        AnsiConsole.MarkupLine("  --all                  Fetch all results (ignores pagination)");
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[grey]Examples:[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk worklogs list --from 2024-01-01 --per-page 20[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk worklogs list --created-from 2024-01-01T09:00 --created-to 2024-01-01T17:00[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk worklogs count --from 2024-01-01[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk worklogs export --output worklogs.json --from 2024-01-01 --all[/]");
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

    private static async Task<int> ListWorklogsAsync(string[] args, IBoldDeskClient client)
    {
        try
        {
            var page = int.TryParse(GetOption(args, "--page") ?? GetOption(args, "-p"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page") ?? GetOption(args, "-n"), out var pp) ? Math.Min(pp, 100) : 50;
            var format = GetOption(args, "--format") ?? "table";
            var order = GetOption(args, "--order");
            var includeDeleted = HasFlag(args, "--include-deleted");
            var all = HasFlag(args, "--all");

            // Parse date filters
            DateTime? from = null, to = null, createdFrom = null, createdTo = null;
            
            if (DateTime.TryParse(GetOption(args, "--from"), out var f)) from = f;
            if (DateTime.TryParse(GetOption(args, "--to"), out var t)) to = t;
            if (DateTime.TryParse(GetOption(args, "--created-from"), out var cf)) createdFrom = cf;
            if (DateTime.TryParse(GetOption(args, "--created-to"), out var ct)) createdTo = ct;

            var parameters = new WorklogQueryParameters
            {
                Page = page,
                PerPage = perPage,
                LastUpdatedDateFrom = from,
                LastUpdatedDateTo = to,
                LastCreatedDateFrom = createdFrom,
                LastCreatedDateTo = createdTo,
                OrderBy = order,
                IncludeDeletedWorklogs = includeDeleted,
                RequiresCounts = true
            };

            if (all)
            {
                await AnsiConsole.Status()
                    .StartAsync("Fetching all worklogs...", async ctx =>
                    {
                        var allWorklogs = new List<Worklog>();
                        await foreach (var worklog in client.Worklogs.GetAllWorklogsAsync(parameters))
                        {
                            allWorklogs.Add(worklog);
                            ctx.Status($"Fetched {allWorklogs.Count} worklogs...");
                        }

                        if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                        {
                            var json = JsonSerializer.Serialize(allWorklogs, new JsonSerializerOptions { WriteIndented = true });
                            Console.WriteLine(json);
                        }
                        else
                        {
                            if (!allWorklogs.Any())
                            {
                                AnsiConsole.MarkupLine("[yellow]No worklogs found[/]");
                                return;
                            }

                            RenderWorklogsTable(allWorklogs);
                            AnsiConsole.MarkupLine($"[grey]Total: {allWorklogs.Count} worklogs[/]");
                        }
                    });
            }
            else
            {
                var response = await client.Worklogs.GetWorklogsAsync(parameters);

                if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                {
                    var json = JsonSerializer.Serialize(response.Result, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(json);
                }
                else
                {
                    if (!response.Result.Any())
                    {
                        AnsiConsole.MarkupLine("[yellow]No worklogs found[/]");
                        return 0;
                    }

                    RenderWorklogsTable(response.Result);
                    
                    if (response.Count > 0)
                    {
                        AnsiConsole.MarkupLine($"[grey]Page {page}, showing {response.Result.Count} of {response.Count} total[/]");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> GetWorklogCountAsync(string[] args, IBoldDeskClient client)
    {
        try
        {
            var includeDeleted = HasFlag(args, "--include-deleted");

            // Parse date filters
            DateTime? from = null, to = null, createdFrom = null, createdTo = null;
            
            if (DateTime.TryParse(GetOption(args, "--from"), out var f)) from = f;
            if (DateTime.TryParse(GetOption(args, "--to"), out var t)) to = t;
            if (DateTime.TryParse(GetOption(args, "--created-from"), out var cf)) createdFrom = cf;
            if (DateTime.TryParse(GetOption(args, "--created-to"), out var ct)) createdTo = ct;

            var parameters = new WorklogQueryParameters
            {
                LastUpdatedDateFrom = from,
                LastUpdatedDateTo = to,
                LastCreatedDateFrom = createdFrom,
                LastCreatedDateTo = createdTo,
                IncludeDeletedWorklogs = includeDeleted
            };

            var count = await client.Worklogs.GetWorklogCountAsync(parameters);
            
            AnsiConsole.MarkupLine($"[green]Total worklogs: {count}[/]");
            
            if (includeDeleted)
            {
                AnsiConsole.MarkupLine($"[grey]  (includes deleted worklogs)[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> ExportWorklogsAsync(string[] args, IBoldDeskClient client)
    {
        var outputPath = GetOption(args, "--output");
        if (string.IsNullOrEmpty(outputPath))
        {
            AnsiConsole.MarkupLine("[red]Error: --output is required[/]");
            return 1;
        }

        try
        {
            var format = GetOption(args, "--format") ?? "json";
            var includeDeleted = HasFlag(args, "--include-deleted");
            var all = HasFlag(args, "--all");

            // Parse date filters
            DateTime? from = null, to = null, createdFrom = null, createdTo = null;
            
            if (DateTime.TryParse(GetOption(args, "--from"), out var f)) from = f;
            if (DateTime.TryParse(GetOption(args, "--to"), out var t)) to = t;
            if (DateTime.TryParse(GetOption(args, "--created-from"), out var cf)) createdFrom = cf;
            if (DateTime.TryParse(GetOption(args, "--created-to"), out var ct)) createdTo = ct;

            var parameters = new WorklogQueryParameters
            {
                Page = 1,
                PerPage = 100,
                LastUpdatedDateFrom = from,
                LastUpdatedDateTo = to,
                LastCreatedDateFrom = createdFrom,
                LastCreatedDateTo = createdTo,
                IncludeDeletedWorklogs = includeDeleted,
                RequiresCounts = true
            };

            await AnsiConsole.Status()
                .StartAsync("Exporting worklogs...", async ctx =>
                {
                    var allWorklogs = new List<Worklog>();
                    
                    if (all)
                    {
                        await foreach (var worklog in client.Worklogs.GetAllWorklogsAsync(parameters))
                        {
                            allWorklogs.Add(worklog);
                            ctx.Status($"Fetched {allWorklogs.Count} worklogs...");
                        }
                    }
                    else
                    {
                        var response = await client.Worklogs.GetWorklogsAsync(parameters);
                        allWorklogs.AddRange(response.Result);
                    }

                    if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
                    {
                        await ExportToCsvAsync(outputPath, allWorklogs);
                    }
                    else
                    {
                        await ExportToJsonAsync(outputPath, allWorklogs);
                    }
                });

            AnsiConsole.MarkupLine($"[green]✓ Exported to {outputPath}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task ExportToJsonAsync(string outputPath, List<Worklog> worklogs)
    {
        var json = JsonSerializer.Serialize(worklogs, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(outputPath, json);
    }

    private static async Task ExportToCsvAsync(string outputPath, List<Worklog> worklogs)
    {
        using var writer = new StreamWriter(outputPath);
        
        // CSV Header
        await writer.WriteLineAsync("WorklogId,TicketId,TimeSpent,IsBillable,IsDeleted,WorklogDate,Description,CreatedBy,CreatedOn,LastModifiedOn");
        
        foreach (var worklog in worklogs)
        {
            var line = $"{worklog.WorklogId}," +
                      $"{worklog.TicketId}," +
                      $"{worklog.TimeSpent}," +
                      $"{worklog.IsBillable}," +
                      $"{worklog.IsDeleted}," +
                      $"{worklog.WorklogDate:yyyy-MM-dd}," +
                      $"\"{EscapeCsv(worklog.Description)}\"," +
                      $"\"{EscapeCsv(worklog.CreatedBy?.Name ?? "")}\"," +
                      $"{worklog.CreatedOn:yyyy-MM-dd HH:mm:ss}," +
                      $"{worklog.LastModifiedOn:yyyy-MM-dd HH:mm:ss}";
            
            await writer.WriteLineAsync(line);
        }
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return value.Replace("\"", "\"\"");
    }

    private static void RenderWorklogsTable(IEnumerable<Worklog> worklogs)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("ID")
            .AddColumn("Ticket")
            .AddColumn("Time")
            .AddColumn("Date")
            .AddColumn("Billable")
            .AddColumn("Description")
            .AddColumn("Created By");

        foreach (var worklog in worklogs.Take(50)) // Limit display for readability
        {
            var timeFormatted = FormatTimeSpent(worklog.TimeSpent);
            var billable = worklog.IsBillable ? "[green]Yes[/]" : "[grey]No[/]";
            var description = TrimDescription(worklog.Description, 40);
            var createdBy = worklog.CreatedBy?.Name ?? "System";
            
            table.AddRow(
                worklog.WorklogId.ToString(),
                worklog.TicketId.ToString(),
                timeFormatted,
                worklog.WorklogDate.ToString("MM/dd"),
                billable,
                description,
                createdBy.Length > 15 ? createdBy[..12] + "..." : createdBy
            );
        }

        AnsiConsole.Write(table);
    }

    private static string FormatTimeSpent(int minutes)
    {
        if (minutes < 60)
            return $"{minutes}m";
            
        var hours = minutes / 60;
        var remainingMinutes = minutes % 60;
        
        if (remainingMinutes == 0)
            return $"{hours}h";
            
        return $"{hours}h {remainingMinutes}m";
    }

    private static string TrimDescription(string description, int maxLength)
    {
        if (string.IsNullOrEmpty(description)) return "";
        if (description.Length <= maxLength) return description;
        return description[..(maxLength - 3)] + "...";
    }
}

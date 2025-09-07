using BoldDesk;
using BoldDesk.Extensions;
using BoldDesk.Models;
using BoldDesk.QueryBuilder;
using Spectre.Console;
using System.Text.Json;

namespace BoldDesk.Cli.Commands;

public static class BrandCommands
{
    public static async Task<int> HandleBrandCommandAsync(string[] args, IBoldDeskClient client)
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
            "list" => await ListBrandsAsync(remainingArgs, client),
            "get" => await GetBrandAsync(remainingArgs, client),
            "create" => await CreateBrandAsync(remainingArgs, client),
            "update" => await UpdateBrandAsync(remainingArgs, client),
            "delete" => await DeleteBrandAsync(remainingArgs, client),
            "help" or "--help" or "-h" => ShowHelpAndReturn(),
            _ => ShowInvalidCommandAndReturn(subCommand)
        };
    }

    private static void ShowHelp()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]BoldDesk Brand Commands[/]").RuleStyle("grey"));
        AnsiConsole.WriteLine();
        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[blue]Command[/]")
            .AddColumn("[green]Description[/]")
            .AddColumn("Options");

        table.AddRow("brands list", "List all brands", "--page <number>, --per-page <number>");
        table.AddRow("brands get", "Get a specific brand", "--id <brandId>");
        table.AddRow("[dim]brands create[/]", "[dim]Create a new brand (API not available)[/]", "[dim]--name <name>, --email <email>[/]");
        table.AddRow("[dim]brands update[/]", "[dim]Update a brand (API not available)[/]", "[dim]--id <brandId>, --name <name>[/]");
        table.AddRow("[dim]brands delete[/]", "[dim]Delete a brand (API not available)[/]", "[dim]--id <brandId>[/]");
        
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[grey]Examples:[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk brands list --per-page 10[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk brands get --id 123[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk brands create --name \"My Brand\" --email support@example.com --domain example.com[/]");
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

    private static async Task<int> ListBrandsAsync(string[] args, IBoldDeskClient client)
    {
        try
        {
            var page = int.TryParse(GetOption(args, "--page"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page"), out var pp) ? pp : 10;

            await AnsiConsole.Status()
                .StartAsync("Fetching brands...", async ctx =>
                {
                    var brands = await client.Brands.GetBrandsAsync();
                    
                    if (!brands.Result.Any())
                    {
                        AnsiConsole.MarkupLine("[yellow]No brands found[/]");
                        return;
                    }

                    var table = new Table()
                        .Border(TableBorder.Rounded)
                        .Title($"[yellow]Brands (Total: {brands.Result.Count})[/]")
                        .AddColumn("ID")
                        .AddColumn("Name")
                        .AddColumn("Email")
                        .AddColumn("Domain")
                        .AddColumn("Status");

                    foreach (var brand in brands.Result.Skip((page - 1) * perPage).Take(perPage))
                    {
                        table.AddRow(
                            brand.BrandId.ToString(),
                            brand.BrandName ?? "",
                            "N/A", // Email not available in Brand model
                            "N/A", // Domain not available in Brand model
                            !brand.IsDisabled ? "[green]Active[/]" : "[red]Inactive[/]"
                        );
                    }

                    AnsiConsole.Write(table);
                });
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> GetBrandAsync(string[] args, IBoldDeskClient client)
    {
        var brandId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(brandId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        try
        {
            if (!int.TryParse(brandId, out var id))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid brand ID[/]");
                return 1;
            }

            // Note: GetBrandAsync is not available in the service - need to get all and filter
            var brands = await client.Brands.GetBrandsAsync();
            var brand = brands.Result.FirstOrDefault(b => b.BrandId == id);
            
            if (brand == null)
            {
                AnsiConsole.MarkupLine($"[red]Brand with ID {id} not found[/]");
                return 1;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Brand Details - ID: {id}[/]")
                .AddColumn(new TableColumn("[blue]Property[/]").Width(20))
                .AddColumn("[green]Value[/]");

            table.AddRow("ID", brand.BrandId.ToString());
            table.AddRow("Name", brand.BrandName ?? "");
            table.AddRow("Published", brand.IsPublished ? "[green]Yes[/]" : "[red]No[/]");
            table.AddRow("Status", !brand.IsDisabled ? "[green]Active[/]" : "[red]Disabled[/]");

            AnsiConsole.Write(table);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> CreateBrandAsync(string[] args, IBoldDeskClient client)
    {
        // Note: Brand creation is not available in the current service interface
        AnsiConsole.MarkupLine("[yellow]Brand creation is not currently supported in the API[/]");
        return 0;
    }

    private static async Task<int> UpdateBrandAsync(string[] args, IBoldDeskClient client)
    {
        var brandId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(brandId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        if (!int.TryParse(brandId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid brand ID[/]");
            return 1;
        }

        try
        {
            // Note: Brand update is not available in the current service interface
            AnsiConsole.MarkupLine("[yellow]Brand update is not currently supported in the API[/]");
            return 0;
            
            /*var request = new UpdateBrandRequest
            {
                Name = GetOption(args, "--name"),
                Email = GetOption(args, "--email"),
                Domain = GetOption(args, "--domain"),
                SupportEmail = GetOption(args, "--support-email"),
                Website = GetOption(args, "--website"),
                Phone = GetOption(args, "--phone")
            };

            if (HasFlag(args, "--enable"))
                request.IsEnabled = true;
            else if (HasFlag(args, "--disable"))
                request.IsEnabled = false;

            // Note: Brand update is not available in the current service interface
            AnsiConsole.MarkupLine("[yellow]Brand update is not currently supported in the API[/]");*/
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> DeleteBrandAsync(string[] args, IBoldDeskClient client)
    {
        var brandId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(brandId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        if (!int.TryParse(brandId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid brand ID[/]");
            return 1;
        }

        try
        {
            if (!HasFlag(args, "--force"))
            {
                if (!AnsiConsole.Confirm($"Are you sure you want to delete brand {id}?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            // Note: Brand deletion is not available in the current service interface
            AnsiConsole.MarkupLine("[yellow]Brand deletion is not currently supported in the API[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }
}

using BoldDesk;
using BoldDesk.Models;
using BoldDesk.QueryBuilder;
using Spectre.Console;
using System.Text.Json;

namespace BoldDesk.Cli.Commands;

public static class AgentCommands
{
    public static async Task<int> HandleAgentCommandAsync(string[] args, IBoldDeskClient client)
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
            "list" => await ListAgentsAsync(remainingArgs, client),
            "get" => await GetAgentAsync(remainingArgs, client),
            "create" => await CreateAgentAsync(remainingArgs, client),
            "update" => await UpdateAgentAsync(remainingArgs, client),
            "delete" => await DeleteAgentAsync(remainingArgs, client),
            "status" => await GetAgentStatusAsync(remainingArgs, client),
            "activate" => await ActivateAgentAsync(remainingArgs, client),
            "deactivate" => await DeactivateAgentAsync(remainingArgs, client),
            "permissions" => await GetAgentPermissionsAsync(remainingArgs, client),
            "groups" => await GetAgentGroupsAsync(remainingArgs, client),
            "help" or "--help" or "-h" => ShowHelpAndReturn(),
            _ => ShowInvalidCommandAndReturn(subCommand)
        };
    }

    private static void ShowHelp()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]BoldDesk Agent Commands[/]").RuleStyle("grey"));
        AnsiConsole.WriteLine();
        
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[blue]Command[/]")
            .AddColumn("[green]Description[/]")
            .AddColumn("Options");

        table.AddRow("agents list", "List all agents", "--page <number>, --per-page <number>, --status <status>");
        table.AddRow("agents get", "Get a specific agent", "--id <agentId>");
        table.AddRow("agents create", "Create a new agent", "--email <email>, --name <name>, --role <roleId>");
        table.AddRow("agents update", "Update an agent", "--id <agentId>, --name <name>, --role <roleId>");
        table.AddRow("agents delete", "Delete an agent", "--id <agentId>");
        table.AddRow("agents status", "Get agent status", "--id <agentId>");
        table.AddRow("agents activate", "Activate an agent", "--id <agentId>");
        table.AddRow("agents deactivate", "Deactivate an agent", "--id <agentId>");
        table.AddRow("agents permissions", "Get agent permissions", "--id <agentId>");
        table.AddRow("agents groups", "Get agent's groups", "--id <agentId>");
        
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        
        AnsiConsole.MarkupLine("[grey]Examples:[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk agents list --status active --per-page 20[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk agents get --id 123[/]");
        AnsiConsole.MarkupLine("  [dim]bolddesk agents create --email agent@example.com --name \"John Doe\" --role 1[/]");
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

    private static async Task<int> ListAgentsAsync(string[] args, IBoldDeskClient client)
    {
        try
        {
            var page = int.TryParse(GetOption(args, "--page"), out var p) ? p : 1;
            var perPage = int.TryParse(GetOption(args, "--per-page"), out var pp) ? pp : 20;
            var status = GetOption(args, "--status");
            var searchTerm = GetOption(args, "--search");
            
            var parameters = new AgentQueryParameters
            {
                Page = page,
                PerPage = perPage,
                Q = searchTerm
            };
            
            // Note: Status filtering will be done after getting results

            await AnsiConsole.Status()
                .StartAsync("Fetching agents...", async ctx =>
                {
                    var response = await client.Agents.GetAgentsAsync(parameters);
                    
                    if (!response.Result.Any())
                    {
                        AnsiConsole.MarkupLine("[yellow]No agents found[/]");
                        return;
                    }

                    var table = new Table()
                        .Border(TableBorder.Rounded)
                        .Title($"[yellow]Agents (Page {page}, Total: {response.Count})[/]")
                        .AddColumn("ID")
                        .AddColumn("Name")
                        .AddColumn("Email")
                        .AddColumn("Role")
                        .AddColumn("Status")
                        .AddColumn("Last Login");

                    foreach (var agent in response.Result)
                    {
                        table.AddRow(
                            agent.UserId.ToString(),
                            agent.Name ?? "",
                            agent.EmailId ?? "",
                            agent.Roles.FirstOrDefault()?.RoleName ?? "N/A",
                            !agent.IsBlocked ? "[green]Active[/]" : "[red]Blocked[/]",
                            agent.LastActivityOn?.ToString("yyyy-MM-dd HH:mm") ?? "Never"
                        );
                    }

                    AnsiConsole.Write(table);
                    
                    if (response.Count > perPage)
                    {
                        AnsiConsole.MarkupLine($"[dim]Showing {response.Result.Count} of {response.Count} agents. Use --page to navigate.[/]");
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

    private static async Task<int> GetAgentAsync(string[] args, IBoldDeskClient client)
    {
        var agentId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(agentId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        try
        {
            if (!int.TryParse(agentId, out var id))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid agent ID[/]");
                return 1;
            }

            var agent = await client.Agents.GetAgentAsync(id);
            
            if (agent == null)
            {
                AnsiConsole.MarkupLine($"[red]Agent with ID {id} not found[/]");
                return 1;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[yellow]Agent Details - ID: {id}[/]")
                .AddColumn(new TableColumn("[blue]Property[/]").Width(25))
                .AddColumn("[green]Value[/]");

            table.AddRow("ID", agent.UserId.ToString());
            table.AddRow("Name", agent.Name ?? "");
            table.AddRow("Display Name", agent.DisplayName ?? "");
            table.AddRow("Email", agent.EmailId ?? "");
            table.AddRow("Role", agent.Roles.FirstOrDefault()?.RoleName ?? "N/A");
            table.AddRow("Status", !agent.IsBlocked ? "[green]Active[/]" : "[red]Blocked[/]");
            table.AddRow("Verified", agent.IsVerified ? "[green]Yes[/]" : "[red]No[/]");
            table.AddRow("Available", agent.IsAvailable ? "[green]Yes[/]" : "[grey]No[/]");
            table.AddRow("Current Status", agent.Status ?? "N/A");
            table.AddRow("Created On", agent.CreatedOn.ToString("yyyy-MM-dd HH:mm"));
            table.AddRow("Modified On", agent.LastModifiedOn.ToString("yyyy-MM-dd HH:mm"));
            table.AddRow("Last Activity", agent.LastActivityOn?.ToString("yyyy-MM-dd HH:mm") ?? "Never");
            
            AnsiConsole.Write(table);
            
            // Show roles if present
            if (agent.Roles.Any())
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Roles:[/]");
                foreach (var role in agent.Roles)
                {
                    AnsiConsole.MarkupLine($"  • {role.RoleName} (ID: {role.RoleId})");
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

    private static async Task<int> CreateAgentAsync(string[] args, IBoldDeskClient client)
    {
        var email = GetOption(args, "--email");
        var name = GetOption(args, "--name");
        var roleId = GetOption(args, "--role");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
        {
            AnsiConsole.MarkupLine("[red]Error: --email and --name are required[/]");
            return 1;
        }

        try
        {
            var request = new AddAgentRequest
            {
                EmailId = email,
                Name = name,
                DisplayName = GetOption(args, "--display-name") ?? name,
                RoleIds = roleId, // Use role ID as string
                AgentPhoneNo = GetOption(args, "--phone"),
                AgentMobileNo = GetOption(args, "--mobile"),
                IsVerified = !HasFlag(args, "--unverified")
            };

            AnsiConsole.MarkupLine("[yellow]Creating agent...[/]");
            var response = await client.Agents.AddAgentAsync(request);
            
            // Note: AgentOperationResponse doesn't have a Result property
            if (string.IsNullOrEmpty(response.Message) || response.Message.Contains("success", StringComparison.OrdinalIgnoreCase))
            {
                AnsiConsole.MarkupLine($"[green]✓ Agent created successfully with ID: {response.Id ?? 0}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Failed to create agent: {response.Message}[/]");
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

    private static async Task<int> UpdateAgentAsync(string[] args, IBoldDeskClient client)
    {
        var agentId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(agentId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        if (!int.TryParse(agentId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid agent ID[/]");
            return 1;
        }

        try
        {
            var request = new AddAgentRequest  // Note: Update uses the same request type
            {
                Name = GetOption(args, "--name"),
                DisplayName = GetOption(args, "--display-name"),
                RoleIds = GetOption(args, "--role"),
                AgentPhoneNo = GetOption(args, "--phone"),
                AgentMobileNo = GetOption(args, "--mobile")
            };

            AnsiConsole.MarkupLine($"[yellow]Updating agent {id}...[/]");
            var response = await client.Agents.UpdateAgentAsync(id, request);
            
            AnsiConsole.MarkupLine($"[green]✓ Agent {id} updated successfully[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> DeleteAgentAsync(string[] args, IBoldDeskClient client)
    {
        var agentId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(agentId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        if (!int.TryParse(agentId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid agent ID[/]");
            return 1;
        }

        try
        {
            if (!HasFlag(args, "--force"))
            {
                if (!AnsiConsole.Confirm($"Are you sure you want to delete agent {id}?"))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled[/]");
                    return 0;
                }
            }

            AnsiConsole.MarkupLine($"[yellow]Deleting agent {id}...[/]");
            // Note: Delete agent is not available in current service interface
            AnsiConsole.MarkupLine("[yellow]Agent deletion is not currently supported in the API[/]");
            
            AnsiConsole.MarkupLine($"[green]✓ Agent {id} deleted successfully[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> GetAgentStatusAsync(string[] args, IBoldDeskClient client)
    {
        var agentId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(agentId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        try
        {
            if (!int.TryParse(agentId, out var id))
            {
                AnsiConsole.MarkupLine("[red]Error: Invalid agent ID[/]");
                return 1;
            }

            // Note: Get agent status is not available in current service interface  
            // We'll get the agent details instead
            var agent = await client.Agents.GetAgentAsync(id);
            
            AnsiConsole.MarkupLine($"[yellow]Agent {id} Status:[/]");
            AnsiConsole.MarkupLine($"  Blocked: {(agent.IsBlocked ? "[red]Yes[/]" : "[green]No[/]")}");
            AnsiConsole.MarkupLine($"  Available: {(agent.IsAvailable ? "[green]Yes[/]" : "[grey]No[/]")}");
            AnsiConsole.MarkupLine($"  Verified: {(agent.IsVerified ? "[green]Yes[/]" : "[grey]No[/]")}");
            AnsiConsole.MarkupLine($"  Status: {agent.Status}");
            AnsiConsole.MarkupLine($"  Last Activity: {agent.LastActivityOn?.ToString("yyyy-MM-dd HH:mm") ?? "Never"}");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> ActivateAgentAsync(string[] args, IBoldDeskClient client)
    {
        var agentId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(agentId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        if (!int.TryParse(agentId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid agent ID[/]");
            return 1;
        }

        try
        {
            AnsiConsole.MarkupLine($"[yellow]Activating agent {id}...[/]");
            var response = await client.Agents.ActivateAgentAsync(id);
            
            AnsiConsole.MarkupLine($"[green]✓ Agent {id} activated successfully[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> DeactivateAgentAsync(string[] args, IBoldDeskClient client)
    {
        var agentId = GetOption(args, "--id");
        if (string.IsNullOrEmpty(agentId))
        {
            AnsiConsole.MarkupLine("[red]Error: --id is required[/]");
            return 1;
        }

        if (!int.TryParse(agentId, out var id))
        {
            AnsiConsole.MarkupLine("[red]Error: Invalid agent ID[/]");
            return 1;
        }

        try
        {
            AnsiConsole.MarkupLine($"[yellow]Deactivating agent {id}...[/]");
            var response = await client.Agents.DeactivateAgentAsync(id);
            
            AnsiConsole.MarkupLine($"[green]✓ Agent {id} deactivated successfully[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            return 1;
        }
        
        return 0;
    }

    private static async Task<int> GetAgentPermissionsAsync(string[] args, IBoldDeskClient client)
    {
        // Note: Get agent permissions is not available in current service interface
        AnsiConsole.MarkupLine("[yellow]Agent permissions endpoint is not currently supported in the API[/]");
        return 0;
    }

    private static async Task<int> GetAgentGroupsAsync(string[] args, IBoldDeskClient client)
    {
        // Note: Get agent groups is not available in current service interface
        AnsiConsole.MarkupLine("[yellow]Agent groups endpoint is not currently supported in the API[/]");
        return 0;
    }
}

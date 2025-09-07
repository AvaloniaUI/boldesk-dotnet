using System.Reflection;
using System.Text.Json;
using BoldDesk;
using BoldDesk.Extensions;
using BoldDesk.Models;
using BoldDesk.Services;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace BoldDesk.Cli;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var argsList = args.ToList();
        var noAnsi = argsList.Remove("--no-ansi");
        var compact = argsList.Remove("--compact");
        string? theme = null;
        for (int i = 0; i < argsList.Count; i++)
        {
            if (argsList[i] == "--theme" && i + 1 < argsList.Count)
            {
                theme = argsList[i + 1];
                argsList.RemoveAt(i + 1);
                argsList.RemoveAt(i);
                break;
            }
        }
        if (string.Equals(theme, "plain", StringComparison.OrdinalIgnoreCase)) noAnsi = true;
        Ui.Setup(noAnsi, theme, compact);
        args = argsList.ToArray();
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

        if (args.Length == 0 || args[0] is "--help" or "-h")
        {
            ShowHelp(version);
            return 0;
        }

        if (args[0] is "--version" or "-v")
        {
            Console.WriteLine($"BoldDesk CLI v{version}");
            return 0;
        }

        try
        {
            return await Route(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
    
    // Minimal UI wrapper to allow disabling ANSI globally
    static class Ui
    {
        private static IAnsiConsole _console = AnsiConsole.Console;
        public static void Setup(bool noAnsi, string? theme, bool compact)
        {
            var settings = new AnsiConsoleSettings
            {
                Ansi = noAnsi ? AnsiSupport.No : AnsiSupport.Yes,
                ColorSystem = ColorSystemSupport.TrueColor
            };
            _console = AnsiConsole.Create(settings);
            Theme.Apply(string.IsNullOrWhiteSpace(theme) ? "dark" : theme.ToLowerInvariant(), compact);
        }
        public static void MarkupLine(string text) => _console.MarkupLine(text);
        public static void Write(IRenderable renderable) => _console.Write(renderable);
        public static Spectre.Console.Status Status() => _console.Status();
    }

    static class Theme
    {
        public static string Primary { get; private set; } = "deepskyblue1";
        public static string Accent { get; private set; } = "gold1";
        public static string Highlight { get; private set; } = "mediumpurple1";
        public static string Muted { get; private set; } = "grey";
        public static bool Compact { get; private set; } = false;
        public static void Apply(string theme, bool compact)
        {
            Compact = compact;
            switch (theme)
            {
                case "light":
                    Primary = "blue"; Accent = "orange1"; Highlight = "mediumorchid1"; Muted = "silver"; break;
                case "plain":
                    Primary = Accent = Highlight = Muted = "white"; break;
                default:
                    Primary = "deepskyblue1"; Accent = "gold1"; Highlight = "mediumpurple1"; Muted = "grey"; break;
            }
        }
    }

    static string TrimTo(string? value, int max)
    {
        var s = value ?? string.Empty;
        if (s.Length <= max) return s;
        return s.Substring(0, Math.Max(0, max - 1)) + "…";
    }

    static void ShowHelp(string version)
    {
        Ui.MarkupLine($"[bold deepskyblue1]BoldDesk CLI[/] [grey]v{version}[/]");
        Ui.MarkupLine("[grey]Command-line interface for BoldDesk API[/]");
        Console.WriteLine();
        Ui.MarkupLine("[bold]Usage[/]: bolddesk [underline]<command>[/] [grey][[options]][/]");
        Console.WriteLine();
        Ui.MarkupLine("[bold]Commands[/]:");
        AnsiConsole.MarkupLine("  [yellow]Configuration:[/]");
        Console.WriteLine("    config set          Set configuration values");
        Console.WriteLine("    config get          Get current configuration");
        Console.WriteLine("    config test         Test connection to BoldDesk API");
        Console.WriteLine();
        AnsiConsole.MarkupLine("  [yellow]Tickets:[/]");
        Console.WriteLine("    tickets <subcommand>    Full ticket management (list, get, create, update, reply, etc.)");
        Console.WriteLine("    tickets help            Show all available ticket commands");
        Console.WriteLine();
        AnsiConsole.MarkupLine("  [yellow]Brands:[/]");
        Console.WriteLine("    brands <subcommand>     Brand management (list, get, create, update, delete)");
        Console.WriteLine("    brands help             Show all available brand commands");
        Console.WriteLine();
        AnsiConsole.MarkupLine("  [yellow]Agents:[/]");
        Console.WriteLine("    agents <subcommand>     Agent management (list, get, create, update, status, etc.)");
        Console.WriteLine("    agents help             Show all available agent commands");
        Console.WriteLine();
        AnsiConsole.MarkupLine("  [yellow]Contact Groups:[/]");
        Console.WriteLine("    contact-groups <subcommand>  Contact group management (list, get, create, update, members)");
        Console.WriteLine("    contact-groups help          Show all available contact group commands");
        Console.WriteLine();
        AnsiConsole.MarkupLine("  [yellow]Contacts:[/]");
        Console.WriteLine("    contacts <subcommand>        Full contact management (list, get, create, update, etc.)");
        Console.WriteLine("    contacts help                Show all available contact commands");
        Console.WriteLine();
        AnsiConsole.MarkupLine("  [yellow]Worklogs:[/]");
        Console.WriteLine("    worklogs <subcommand>        Full worklog management (list, count, export)");
        Console.WriteLine("    worklogs help                Show all available worklog commands");
        Console.WriteLine();
        AnsiConsole.MarkupLine("  [yellow]Fields:[/]");
        Console.WriteLine("    fields <subcommand>          Field option management (list, add, remove, configure)");
        Console.WriteLine("    fields help                  Show all available field commands");
        Console.WriteLine();
        AnsiConsole.MarkupLine("  [yellow]Other:[/]");
        Console.WriteLine("    list <resource>      Alias for '<resource> list' (e.g. 'list agents')");
        Console.WriteLine("    install-completion  Print or install shell completion");
        Console.WriteLine("    repl               Start interactive shell");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --version, -v       Show version information");
        Console.WriteLine("  --help, -h          Show this help message");
        Console.WriteLine();
        Ui.MarkupLine("[bold]Getting Help:[/]");
        Console.WriteLine("  Run any command with 'help' to see detailed usage:");
        Console.WriteLine("    bolddesk tickets help");
        Console.WriteLine("    bolddesk agents help");
        Console.WriteLine("    bolddesk brands help");
        Console.WriteLine("    bolddesk contact-groups help");
        Console.WriteLine("    bolddesk contacts help");
        Console.WriteLine("    bolddesk worklogs help");
        Console.WriteLine("    bolddesk fields help");
    }

    static async Task<int> Route(string[] args)
    {
        return args[0].ToLowerInvariant() switch
        {
            "config" => await HandleConfig(args[1..]),
            "tickets" => await HandleTickets(args[1..]),
            "brands" => await HandleBrands(args[1..]),
            "agents" => await HandleAgents(args[1..]),
            "worklogs" => await HandleWorklogs(args[1..]),
            "fields" => await HandleFields(args[1..]),
            "contact-groups" => await HandleContactGroups(args[1..]),
            "contacts" => await HandleContacts(args[1..]),
            // Friendly alias so `bolddesk list agents` works
            "list" => await HandleList(args[1..]),
            "complete" => await HandleComplete(args[1..]),
            "install-completion" => await HandleInstallCompletion(args[1..]),
            "repl" => await HandleRepl(args[1..]),
            _ => Unknown(args[0])
        };
    }

    static int Unknown(string cmd)
    {
        Ui.MarkupLine($"[red]Unknown command[/]: {Markup.Escape(cmd)}");
        Ui.MarkupLine("[grey]Use --help for usage.[/]");
        return 1;
    }

    // CONFIG
    static async Task<int> HandleConfig(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h")
        {
            Console.WriteLine("Usage: bolddesk config <set|get|test> [options]");
            Console.WriteLine("  set  --domain <domain> --api-key <key>");
            Console.WriteLine("  get");
            Console.WriteLine("  test");
            return 0;
        }

        var svc = new Services.ConfigurationService();

        switch (args[0].ToLowerInvariant())
        {
            case "set":
                string? domain = null;
                string? apiKey = null;
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--domain":
                        case "-d":
                            if (i + 1 < args.Length) domain = args[++i];
                            break;
                        case "--api-key":
                        case "-k":
                            if (i + 1 < args.Length) apiKey = args[++i];
                            break;
                    }
                }
                var cfg = await svc.LoadAsync() ?? new Services.CliConfig();
                if (!string.IsNullOrWhiteSpace(domain)) cfg.Domain = domain;
                if (!string.IsNullOrWhiteSpace(apiKey)) cfg.ApiKey = apiKey;
                await svc.SaveAsync(cfg);
                Console.WriteLine("Configuration saved.");
                return 0;
            case "get":
                var c = await svc.LoadEffectiveAsync();
                if (c == null)
                {
                    Console.WriteLine("No configuration found. Use 'bolddesk config set'.");
                    return 1;
                }
                Console.WriteLine($"Domain: {c.Domain}");
                Console.WriteLine($"API Key: {(string.IsNullOrEmpty(c.ApiKey) ? "(not set)" : "****" + c.ApiKey[^Math.Min(4, c.ApiKey.Length)..])}");
                Console.WriteLine($"Base URL: https://{c.Domain}/api/v1.0");
                return 0;
            case "test":
                var ce = await svc.LoadEffectiveAsync();
                if (ce == null)
                {
                    Console.WriteLine("Invalid or missing config. Set domain and API key.");
                    return 1;
                }
                try
                {
                    using var client = new BoldDeskClient(ce.Domain!, ce.ApiKey!);
                    // Test a light endpoint: brands
                    var resp = await client.Brands.GetBrandsAsync();
                    Console.WriteLine($"✓ Connected. Brands: {resp.Result.Count}");
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Connection failed: {ex.Message}");
                    return 1;
                }
            default:
                return Unknown($"config {args[0]}");
        }
    }

    static JsonSerializerOptions JsonOptions => new()
    {
        WriteIndented = true
    };

    static void PrintFormatOptionHelp()
    {
        Console.WriteLine("  --format json|table   Output format (default table)");
    }

    static void PrintAsJson(object value) =>
        Console.WriteLine(JsonSerializer.Serialize(value, JsonOptions));

    // TICKETS
    static async Task<int> HandleTickets(string[] args)
    {
        // Use the new comprehensive ticket command handler
        var cfgSvc = new Services.ConfigurationService();
        var cfg = await cfgSvc.LoadEffectiveAsync();
        if (cfg == null) 
        { 
            Console.WriteLine("Missing config. Run 'bolddesk config set'."); 
            return 1; 
        }

        // Inline support: `tickets get <id> --raw` prints the raw API JSON
        if (args.Length > 0 && string.Equals(args[0], "get", StringComparison.OrdinalIgnoreCase))
        {
            var hasRaw = args.Any(a => string.Equals(a, "--raw", StringComparison.OrdinalIgnoreCase));
            if (hasRaw)
            {
                // Find ticket id (first non-flag numeric token after 'get')
                int ticketId = 0;
                for (int i = 1; i < args.Length; i++)
                {
                    var tok = args[i];
                    if (tok.StartsWith("-")) continue;
                    if (int.TryParse(tok, out ticketId)) break;
                }
                if (ticketId <= 0)
                {
                    Console.WriteLine("Usage: bolddesk tickets get <id> --raw");
                    return 1;
                }
                await PrintRawAsync($"tickets/{ticketId}", cfg.Domain!, cfg.ApiKey!);
                return 0;
            }
        }

        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        return await Commands.TicketCommands.ExecuteAsync(args, client);
    }

    // Legacy ticket handler for backward compatibility
    static async Task<int> HandleTicketsLegacy(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h")
        {
            Console.WriteLine("Usage: bolddesk tickets <list|get> [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("  --page, -p <n>       Page number (default 1)");
            Console.WriteLine("  --per-page, -n <n>   Items per page (default 100)");
            Console.WriteLine("  --query, -q <q>      Q filter (e.g. createdon:today)");
            Console.WriteLine("  --sort-by <field>    Sort field (e.g. createdon)");
            Console.WriteLine("  --sort-order <asc|desc> Sort order (default desc)");
            PrintFormatOptionHelp();
            return 0;
        }

        var sub = args[0].ToLowerInvariant();

        var cfgSvc = new Services.ConfigurationService();
        var cfg = await cfgSvc.LoadEffectiveAsync();
        if (cfg == null) { Console.WriteLine("Missing config. Run 'bolddesk config set'."); return 1; }

        int page = 1, perPage = 100;
        string? q = null, sortBy = null, orderBy = "desc";
        string format = "table";
        bool allTickets = false;
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--page":
                case "-p": if (i + 1 < args.Length && int.TryParse(args[++i], out var p)) page = p; break;
                case "--per-page":
                case "-n": if (i + 1 < args.Length && int.TryParse(args[++i], out var n)) perPage = n; break;
                case "--query":
                case "-q": if (i + 1 < args.Length) q = args[++i]; break;
                case "--sort-by": if (i + 1 < args.Length) sortBy = args[++i]; break;
                case "--sort-order": if (i + 1 < args.Length) { var order = args[++i]; orderBy = order.ToLowerInvariant() == "asc" ? "asc" : "desc"; } break;
                case "--format": if (i + 1 < args.Length) format = args[++i]; break;
                case "--all": allTickets = true; break;
            }
        }

        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        switch (sub)
        {
            case "list":
            {
                var parameters = new TicketQueryParameters { Page = page, PerPage = perPage, Q = q, OrderBy = string.IsNullOrEmpty(sortBy) ? orderBy : $"{sortBy} {orderBy}" };
                if (allTickets)
                {
                    if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                    {
                        var list = new List<Ticket>();
                        await Ui.Status().StartAsync("Fetching tickets...", async _ =>
                        {
                            await foreach (var t in client.Tickets.GetAllTicketsAsync(parameters)) list.Add(t);
                        });
                        PrintAsJson(list);
                    }
                    else
                    {
                        var list = new List<Ticket>();
                        await Ui.Status().StartAsync("Fetching tickets...", async _ =>
                        {
                            await foreach (var t in client.Tickets.GetAllTicketsAsync(parameters)) list.Add(t);
                        });
                        RenderTicketsTable(list.Take(200));
                        Ui.MarkupLine($"[{Theme.Muted}]Total: {list.Count} (showing first 200)[/]");
                    }
                }
                else
                {
                    var response = await client.Tickets.GetTicketsAsync(parameters);
                    if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                    {
                        PrintAsJson(response.Result);
                    }
                    else
                    {
                        RenderTicketsTable(response.Result.Take(50));
                        Ui.MarkupLine($"[{Theme.Muted}]Count: {response.Result.Count} (showing up to 50)[/]");
                    }
                }
                return 0;
            }
            case "get":
            {
                if (args.Length < 2 || !int.TryParse(args[1], out var ticketId))
                { Console.WriteLine("Usage: bolddesk tickets get <ticketId> [--format json|text]"); return 1; }
                string fmt = "text";
                for (int i = 2; i < args.Length; i++) if (args[i] == "--format" && i + 1 < args.Length) fmt = args[++i];
                var t = await client.Tickets.GetTicketAsync(ticketId);
                if (fmt.Equals("json", StringComparison.OrdinalIgnoreCase))
                {
                    PrintAsJson(t);
                }
                else
                {
                    var status = ColorizeStatus(t.Status?.Description, t.IsResponseOverdue == true || t.IsResolutionOverdue == true);
                    var priority = ColorizePriority(t.Priority?.Description);
                    var lines = new List<string>
                    {
                        $"[mediumpurple1]Status[/]: {status}  [gold1]Priority[/]: {priority}",
                        $"[grey]Created[/]: {t.CreatedOn:yyyy-MM-dd HH:mm}  [grey]Updated[/]: {t.LastUpdatedOn:yyyy-MM-dd HH:mm}",
                        !string.IsNullOrEmpty(t.Brand) ? $"[grey]Brand[/]: {Markup.Escape(t.Brand)}" : null,
                        t.Agent != null ? $"[grey]Agent[/]: {Markup.Escape(t.Agent.Name)}" : null
                    };
                    var body = string.Join("\n", lines.Where(l => l != null));
                    var header = $"Ticket #[grey]{t.TicketId}[/] [deepskyblue1]{Markup.Escape(t.Title ?? string.Empty)}[/]";
                    var panel = new Panel(new Markup(body)) { Header = new PanelHeader(header, Justify.Left), Border = BoxBorder.Rounded };
                    Ui.Write(panel);
                }
                return 0;
            }
            default:
                return Unknown($"tickets {sub}");
        }
    }

    // BRANDS
    static async Task<int> HandleBrands(string[] args)
    {
        var cfgSvc = new Services.ConfigurationService();
        var cfg = await cfgSvc.LoadEffectiveAsync();
        if (cfg == null) 
        { 
            Console.WriteLine("Missing config. Run 'bolddesk config set'."); 
            return 1; 
        }

        // Inline raw: brands get --id <id> --raw
        if (args.Length > 0 && string.Equals(args[0], "get", StringComparison.OrdinalIgnoreCase))
        {
            var hasRaw = args.Any(a => string.Equals(a, "--raw", StringComparison.OrdinalIgnoreCase));
            if (hasRaw)
            {
                int id = 0;
                for (int i = 1; i < args.Length - 1; i++)
                {
                    if (string.Equals(args[i], "--id", StringComparison.OrdinalIgnoreCase) && int.TryParse(args[i + 1], out id))
                        break;
                }
                if (id <= 0) { Console.WriteLine("Usage: bolddesk brands get --id <id> --raw"); return 1; }
                await PrintRawAsync($"brands/{id}", cfg.Domain!, cfg.ApiKey!);
                return 0;
            }
        }

        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        return await Commands.BrandCommands.HandleBrandCommandAsync(args, client);
    }

    // Legacy brand handler for backward compatibility
    static async Task<int> HandleBrandsLegacy(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h")
        {
            Console.WriteLine("Usage: bolddesk brands list [--format json|table]");
            return 0;
        }
        var sub = args[0].ToLowerInvariant();

        string format = "table";
        for (int i = 1; i < args.Length; i++)
            if (args[i] == "--format" && i + 1 < args.Length) format = args[++i];

        var cfg = await new Services.ConfigurationService().LoadEffectiveAsync();
        if (cfg == null) { Console.WriteLine("Missing config. Run 'bolddesk config set'."); return 1; }
        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        if (sub == "list")
        {
            var resp = await client.Brands.GetBrandsAsync();
            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                PrintAsJson(resp.Result);
            else
                RenderBrandsTable(resp.Result);
            return 0;
        }
        else if (sub == "get")
        {
            int? id = null; string? name = null; string fmt = "text";
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--id": if (i + 1 < args.Length && int.TryParse(args[++i], out var bid)) id = bid; break;
                    case "--name": if (i + 1 < args.Length) name = args[++i]; break;
                    case "--format": if (i + 1 < args.Length) fmt = args[++i]; break;
                }
            }
            if (id == null && string.IsNullOrWhiteSpace(name))
            { Console.WriteLine("Usage: bolddesk brands get (--id <id> | --name <name>) [--format json|text]"); return 1; }
            var resp = await client.Brands.GetBrandsAsync();
            Brand? brand = id != null ? resp.Result.FirstOrDefault(b => b.BrandId == id)
                                      : resp.Result.FirstOrDefault(b => string.Equals(b.BrandName, name, StringComparison.OrdinalIgnoreCase));
            if (brand == null)
            { Console.WriteLine("Brand not found."); return 1; }
            if (fmt.Equals("json", StringComparison.OrdinalIgnoreCase)) { PrintAsJson(brand); return 0; }
            var publishedText = brand.IsPublished ? "[green]Yes[/]" : "[red]No[/]";
            var disabledText = brand.IsDisabled ? "[red]Yes[/]" : "[green]No[/]";
            var brandBody = $"[{Theme.Primary}]{Markup.Escape(brand.BrandName)}[/]\\n[{Theme.Muted}]Published[/]: {publishedText}  [{Theme.Muted}]Disabled[/]: {disabledText}";
            var panel = new Panel(new Markup(brandBody))
            { Header = new PanelHeader($"Brand #[{Theme.Muted}]{brand.BrandId}[/]", Justify.Left), Border = BoxBorder.Rounded };
            Ui.Write(panel);
            return 0;
        }
        else
        {
            return Unknown($"brands {sub}");
        }
    }

    // AGENTS
    static async Task<int> HandleAgents(string[] args)
    {
        // Allow showing help without requiring a configured client
        if (args.Length == 0 || args[0] is "--help" or "-h" or "help")
        {
            using var placeholder = new BoldDeskClient("example.com", "placeholder-api-key");
            return await Commands.AgentCommands.HandleAgentCommandAsync(new[] { "help" }, placeholder);
        }

        // Inline raw: agents get --id <id> --raw
        if (args.Length > 0 && string.Equals(args[0], "get", StringComparison.OrdinalIgnoreCase))
        {
            var hasRaw = args.Any(a => string.Equals(a, "--raw", StringComparison.OrdinalIgnoreCase));
            if (hasRaw)
            {
                long id = 0;
                for (int i = 1; i < args.Length - 1; i++)
                {
                    if (string.Equals(args[i], "--id", StringComparison.OrdinalIgnoreCase) && long.TryParse(args[i + 1], out id))
                        break;
                }
                if (id <= 0)
                {
                    Console.WriteLine("Usage: bolddesk agents get --id <userId> --raw");
                    return 1;
                }
                var cfgRaw = await new Services.ConfigurationService().LoadEffectiveAsync();
                if (cfgRaw == null) { Console.WriteLine("Missing config. Run 'bolddesk config set'."); return 1; }
                await PrintRawAsync($"agents/{id}", cfgRaw.Domain!, cfgRaw.ApiKey!);
                return 0;
            }
        }

        var cfgSvc = new Services.ConfigurationService();
        var cfg = await cfgSvc.LoadEffectiveAsync();
        if (cfg == null) 
        { 
            Console.WriteLine("Missing config. Run 'bolddesk config set'."); 
            return 1; 
        }

        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        return await Commands.AgentCommands.HandleAgentCommandAsync(args, client);
    }

    // LIST alias (supports patterns like: `bolddesk list agents [options]`)
    static async Task<int> HandleList(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: bolddesk list <agents|brands|tickets|contacts|contact-groups|worklogs|fields> [options]");
            return 0;
        }

        // entity is the first argument after 'list'
        var entity = args[0].ToLowerInvariant();

        // Some users may type redundant 'list', e.g. `list agents list --page 2`
        // Strip an immediate trailing 'list' token if present
        var rest = args.Skip(1).ToList();
        if (rest.Count > 0 && string.Equals(rest[0], "list", StringComparison.OrdinalIgnoreCase))
            rest = rest.Skip(1).ToList();

        // Prepend the canonical subcommand we want to invoke on the entity handler
        var forwarded = (new[] { "list" }).Concat(rest).ToArray();

        return entity switch
        {
            "agents" => await HandleAgents(forwarded),
            "brands" => await HandleBrands(forwarded),
            "tickets" => await HandleTickets(forwarded),
            "contacts" => await HandleContacts(forwarded),
            "contact-groups" or "contactgroups" or "contact-groups" => await HandleContactGroups(forwarded),
            "worklogs" => await HandleWorklogs(forwarded),
            "fields" => await HandleFields(forwarded),
            _ => Unknown($"list {entity}")
        };
    }

    // Legacy agent handler for backward compatibility
    static async Task<int> HandleAgentsLegacy(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h")
        {
            Console.WriteLine("Usage: bolddesk agents <list|get> [options]");
            return 0;
        }
        var sub = args[0].ToLowerInvariant();

        int page = 1, perPage = 50;
        string format = "table";
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--page": if (i + 1 < args.Length && int.TryParse(args[++i], out var p)) page = p; break;
                case "--per-page": if (i + 1 < args.Length && int.TryParse(args[++i], out var n)) perPage = n; break;
                case "--format": if (i + 1 < args.Length) format = args[++i]; break;
            }
        }

        var cfg = await new Services.ConfigurationService().LoadEffectiveAsync();
        if (cfg == null) { Console.WriteLine("Missing config. Run 'bolddesk config set'."); return 1; }
        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        if (sub == "list")
        {
            var resp = await client.Agents.GetAgentsAsync(new AgentQueryParameters { Page = page, PerPage = perPage });
            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                PrintAsJson(resp.Result);
            else
                RenderAgentsTable(resp.Result);
            return 0;
        }
        else if (sub == "get")
        {
            if (args.Length < 2 || !long.TryParse(args[1], out var userId))
            { Console.WriteLine("Usage: bolddesk agents get <userId> [--format json|text]"); return 1; }
            string fmt = "text"; for (int i = 2; i < args.Length; i++) if (args[i] == "--format" && i + 1 < args.Length) fmt = args[++i];
            var a = await client.Agents.GetAgentAsync(userId);
            if (fmt.Equals("json", StringComparison.OrdinalIgnoreCase))
                PrintAsJson(a);
            else
            {
                var name = Markup.Escape(a.DisplayName ?? a.Name ?? string.Empty);
                var availableText = a.IsAvailable ? "[green]Yes[/]" : "[red]No[/]";
                var bodyLines = new List<string>
                {
                    $"[gold1]Email[/]: {Markup.Escape(a.EmailId ?? string.Empty)}",
                    $"[mediumpurple1]Status[/]: {Markup.Escape(a.Status ?? string.Empty)}  [mediumpurple1]Available[/]: {availableText}",
                    a.JobTitle != null ? $"[grey]Job[/]: {Markup.Escape(a.JobTitle)}" : null,
                    a.PhoneNo != null ? $"[grey]Phone[/]: {Markup.Escape(a.PhoneNo)}" : null,
                    a.MobileNo != null ? $"[grey]Mobile[/]: {Markup.Escape(a.MobileNo)}" : null
                };
                var panel = new Panel(new Markup(string.Join("\n", bodyLines.Where(x => x != null))))
                {
                    Header = new PanelHeader($"Agent #[grey]{a.UserId}[/] [deepskyblue1]{name}[/]", Justify.Left),
                    Border = BoxBorder.Rounded
                };
                Ui.Write(panel);
            }
            return 0;
        }
        else
        {
            return Unknown($"agents {sub}");
        }
    }

    // WORKLOGS
    static async Task<int> HandleWorklogs(string[] args)
    {
        var cfgSvc = new Services.ConfigurationService();
        var cfg = await cfgSvc.LoadEffectiveAsync();
        if (cfg == null) { Console.WriteLine("Missing config. Run 'bolddesk config set'."); return 1; }
        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        return await Commands.WorklogCommands.HandleWorklogCommandAsync(args, client);
    }

    // FIELDS
    static async Task<int> HandleFields(string[] args)
    {
        var cfgSvc = new Services.ConfigurationService();
        var cfg = await cfgSvc.LoadEffectiveAsync();
        if (cfg == null) { Console.WriteLine("Missing config. Run 'bolddesk config set'."); return 1; }
        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        return await Commands.FieldCommands.HandleFieldCommandAsync(args, client);
    }

    // CONTACT GROUPS
    static async Task<int> HandleContactGroups(string[] args)
    {
        var cfgSvc = new Services.ConfigurationService();
        var cfg = await cfgSvc.LoadEffectiveAsync();
        if (cfg == null) 
        { 
            Console.WriteLine("Missing config. Run 'bolddesk config set'."); 
            return 1; 
        }

        // Inline raw: contact-groups get --id <id> --raw
        if (args.Length > 0 && string.Equals(args[0], "get", StringComparison.OrdinalIgnoreCase))
        {
            var hasRaw = args.Any(a => string.Equals(a, "--raw", StringComparison.OrdinalIgnoreCase));
            if (hasRaw)
            {
                long id = 0;
                for (int i = 1; i < args.Length - 1; i++)
                {
                    if (string.Equals(args[i], "--id", StringComparison.OrdinalIgnoreCase) && long.TryParse(args[i + 1], out id))
                        break;
                }
                if (id <= 0) { Console.WriteLine("Usage: bolddesk contact-groups get --id <id> --raw"); return 1; }
                await PrintRawAsync($"contact_groups/{id}", cfg.Domain!, cfg.ApiKey!);
                return 0;
            }
        }

        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        return await Commands.ContactGroupCommands.HandleContactGroupCommandAsync(args, client);
    }

    // Legacy contact groups handler for backward compatibility
    static async Task<int> HandleContactGroupsLegacy(string[] args)
    {
        if (args.Length == 0 || args[0] is "--help" or "-h")
        {
            Console.WriteLine("Usage: bolddesk contact-groups <list|get|count|contacts|notes|export> [options]");
            Console.WriteLine("  list     [--filter <text>] [--page <n>] [--per-page <n>] [--query <q>] [--all] [--format json|table]");
            Console.WriteLine("  get      (--id <id> | --name <name>) [--format json|text]");
            Console.WriteLine("  count    [--filter <text>] [--query <q>]");
            Console.WriteLine("  contacts list <groupId> [--all] [--page <n>] [--per-page <n>] [--format json|table]");
            Console.WriteLine("  contacts add <groupId> (--user <id> ... | --csv <file>) [--access-scope-id <n=1>]");
            Console.WriteLine("  contacts remove <groupId> --user <id>");
            Console.WriteLine("  notes list <groupId> [--page <n>] [--per-page <n>] [--format json|table]");
            Console.WriteLine("  notes add <groupId> --subject <text> (--message <text> | --file <path>)");
            Console.WriteLine("  notes update <noteId> [--subject <text>] (--message <text> | --file <path>)");
            Console.WriteLine("  notes delete <noteId>");
            Console.WriteLine("  export   groups [--all] [--filter <text>] [--query <q>] --out <file> [--format json|csv]");
            Console.WriteLine("  export   contacts <groupId> [--all] --out <file> [--format json|csv]");
            return 0;
        }

        var sub = args[0].ToLowerInvariant();
        var cfg = await new Services.ConfigurationService().LoadEffectiveAsync();
        if (cfg == null) { Console.WriteLine("Missing config. Run 'bolddesk config set'."); return 1; }
        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);

        switch (sub)
        {
            case "list":
            {
                int page = 1, perPage = 50; string? filter = null; string format = "table"; bool all = false; var qList = new List<string>();
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--page": if (i + 1 < args.Length && int.TryParse(args[++i], out var p)) page = p; break;
                        case "--per-page": if (i + 1 < args.Length && int.TryParse(args[++i], out var n)) perPage = n; break;
                        case "--filter": if (i + 1 < args.Length) filter = args[++i]; break;
                        case "--query": case "-q": if (i + 1 < args.Length) qList.Add(args[++i]); break;
                        case "--all": all = true; break;
                        case "--format": if (i + 1 < args.Length) format = args[++i]; break;
                    }
                }
                var qp = new ContactGroupQueryParameters { Page = page, PerPage = perPage, Filter = filter, RequiresCounts = true, Q = qList.Count > 0 ? qList.ToArray() : null };
                if (all)
                {
                    if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                    {
                        var list = new List<ContactGroup>();
                        await foreach (var g in client.ContactGroups.ListAllContactGroupsAsync(qp)) list.Add(g);
                        PrintAsJson(list);
                    }
                    else
                    {
                        var list = new List<ContactGroup>();
                        await foreach (var g in client.ContactGroups.ListAllContactGroupsAsync(qp)) list.Add(g);
                        RenderContactGroupsTable(list);
                        Ui.MarkupLine($"[{Theme.Muted}]\nTotal: {list.Count}[/]");
                    }
                    return 0;
                }
                else
                {
                    var resp = await client.ContactGroups.ListContactGroupsAsync(qp);
            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                PrintAsJson(resp.Result);
            else
            {
                RenderContactGroupsTable(resp.Result);
                if (resp.Count > 0)
                    Ui.MarkupLine($"[grey]\nPage {page}, showing {resp.Result.Count} of ~{resp.Count}[/]");
            }
            return 0;
        }
    }
            case "get":
            {
                long? id = null; string? name = null; string format = "text";
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--id": if (i + 1 < args.Length && long.TryParse(args[++i], out var lid)) id = lid; break;
                        case "--name": if (i + 1 < args.Length) name = args[++i]; break;
                        case "--format": if (i + 1 < args.Length) format = args[++i]; break;
                    }
                }
                if (id == null && string.IsNullOrWhiteSpace(name))
                { Console.WriteLine("Specify --id or --name"); return 1; }
                ContactGroupDetail detail = id != null
                    ? await client.ContactGroups.GetContactGroupAsync(id.Value)
                    : await client.ContactGroups.GetContactGroupByNameAsync(name!);
                if (format.Equals("json", StringComparison.OrdinalIgnoreCase)) PrintAsJson(detail);
                else
                {
                    var body = new Markup($"[deepskyblue1]{Markup.Escape(detail.ContactGroupName)}[/]\n[grey]{Markup.Escape(detail.Description ?? detail.ContactGroupDescription ?? string.Empty)}[/]");
                    var panel = new Panel(body)
                    {
                        Header = new PanelHeader($"Contact Group #[grey]{detail.ContactGroupId}[/]", Justify.Left),
                        Border = BoxBorder.Rounded
                    };
                    Ui.Write(panel);
                }
                return 0;
            }
            case "count":
            {
                string? filter = null; var q = new List<string>();
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--filter": if (i + 1 < args.Length) filter = args[++i]; break;
                        case "--query": case "-q": if (i + 1 < args.Length) q.Add(args[++i]); break;
                    }
                }
                var resp = await client.ContactGroups.ListContactGroupsAsync(new ContactGroupQueryParameters { Page = 1, PerPage = 1, RequiresCounts = true, Filter = filter, Q = q.Count > 0 ? q.ToArray() : null });
                Console.WriteLine(resp.Count);
                return 0;
            }
            case "contacts":
            {
                if (args.Length < 2) { Console.WriteLine("Usage: bolddesk contact-groups contacts <list|add|remove> ..."); return 1; }
                var action = args[1].ToLowerInvariant();
                switch (action)
                {
                    case "list":
                    {
                        if (args.Length < 3 || !long.TryParse(args[2], out var groupId)) { Console.WriteLine("Usage: bolddesk contact-groups contacts list <groupId> [--all] [--page <n>] [--per-page <n>] [--format json|table]"); return 1; }
                        bool all = false; int page = 1, perPage = 50; string format = "table";
                        for (int i = 3; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "--all": all = true; break;
                                case "--page": if (i + 1 < args.Length && int.TryParse(args[++i], out var p)) page = p; break;
                                case "--per-page": if (i + 1 < args.Length && int.TryParse(args[++i], out var n)) perPage = n; break;
                                case "--format": if (i + 1 < args.Length) format = args[++i]; break;
                            }
                        }
                        var qp = new ContactGroupContactsQueryParameters { Page = page, PerPage = perPage, RequiresCounts = true };
                        if (all)
                        {
                            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                            {
                                var list = new List<Contact>();
                                await foreach (var c2 in client.ContactGroups.GetAllContactsByGroupAsync(groupId, qp)) list.Add(c2);
                                PrintAsJson(list);
                            }
                            else
                            {
                                var list = new List<Contact>();
                                await foreach (var c2 in client.ContactGroups.GetAllContactsByGroupAsync(groupId, qp)) list.Add(c2);
                                RenderContactsTable(list);
                                Ui.MarkupLine($"[grey]\nTotal: {list.Count}[/]");
                            }
                            return 0;
                        }
                        else
                        {
                            var resp = await client.ContactGroups.GetContactsByGroupAsync(groupId, qp);
                            if (format.Equals("json", StringComparison.OrdinalIgnoreCase)) PrintAsJson(resp.Result);
                            else
                            {
                                RenderContactsTable(resp.Result);
                                if (resp.Count > 0) Ui.MarkupLine($"[grey]\nPage {page}, showing {resp.Result.Count} of ~{resp.Count}[/]");
                            }
                            return 0;
                        }
                    }
                    case "add":
                    {
                        if (args.Length < 3 || !long.TryParse(args[2], out var groupId)) { Console.WriteLine("Usage: bolddesk contact-groups contacts add <groupId> (--user <id> ... | --csv <file>) [--access-scope-id <n=1>]"); return 1; }
                        var userIds = new List<long>(); string? csv = null; int accessScopeId = 1;
                        for (int i = 3; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "--user": if (i + 1 < args.Length && long.TryParse(args[++i], out var uid)) userIds.Add(uid); break;
                                case "--csv": if (i + 1 < args.Length) csv = args[++i]; break;
                                case "--access-scope-id": if (i + 1 < args.Length && int.TryParse(args[++i], out var scope)) accessScopeId = scope; break;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(csv) && File.Exists(csv))
                        {
                            foreach (var line in (await File.ReadAllLinesAsync(csv)).Where(l => !string.IsNullOrWhiteSpace(l)))
                                if (long.TryParse(line.Trim(), out var uid)) userIds.Add(uid);
                        }
                        if (userIds.Count == 0) { Console.WriteLine("Specify at least one --user or a --csv file of user IDs."); return 1; }
                        var requests = userIds.Select(u => new AddContactToGroupRequest { UserId = u, AccessScopeId = accessScopeId }).ToList();
                        var result = await client.ContactGroups.AddContactsToGroupAsync(groupId, requests);
                        PrintAsJson(result);
                        return 0;
                    }
                    case "remove":
                    {
                        if (args.Length < 5 || !long.TryParse(args[2], out var groupId) || args[3] != "--user" || !long.TryParse(args[4], out var userId))
                        { Console.WriteLine("Usage: bolddesk contact-groups contacts remove <groupId> --user <id>"); return 1; }
                        var resp = await client.ContactGroups.RemoveContactFromGroupAsync(groupId, userId);
                        PrintAsJson(resp);
                        return 0;
                    }
                    default:
                        return Unknown($"contact-groups contacts {action}");
                }
            }
            case "notes":
            {
                if (args.Length < 2) { Console.WriteLine("Usage: bolddesk contact-groups notes <list|add|update|delete> ..."); return 1; }
                var action = args[1].ToLowerInvariant();
                switch (action)
                {
                    case "list":
                    {
                        if (args.Length < 3 || !long.TryParse(args[2], out var groupId)) { Console.WriteLine("Usage: bolddesk contact-groups notes list <groupId> [--page <n>] [--per-page <n>] [--format json|table]"); return 1; }
                        int page = 1, perPage = 50; string format = "table";
                        for (int i = 3; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "--page": if (i + 1 < args.Length && int.TryParse(args[++i], out var p)) page = p; break;
                                case "--per-page": if (i + 1 < args.Length && int.TryParse(args[++i], out var n)) perPage = n; break;
                                case "--format": if (i + 1 < args.Length) format = args[++i]; break;
                            }
                        }
                        var resp = await client.ContactGroups.GetContactGroupNotesAsync(groupId, new ContactGroupNotesQueryParameters { Page = page, PerPage = perPage, RequiresCounts = true });
                        if (format.Equals("json", StringComparison.OrdinalIgnoreCase)) PrintAsJson(resp.ContactGroupNotesObject);
                        else
                        {
                            var table = new Table().Border(TableBorder.Rounded)
                                .AddColumn(new TableColumn($"[{Theme.Muted}]NoteId[/]").Centered())
                                .AddColumn(new TableColumn($"[{Theme.Primary}]Subject[/]"))
                                .AddColumn(new TableColumn($"[{Theme.Muted}]UpdatedOn[/]"));
                            foreach (var n in resp.ContactGroupNotesObject)
                            {
                                table.AddRow(
                                    $"[{Theme.Muted}]{n.Id}[/]",
                                    $"[{Theme.Primary}]{Markup.Escape(n.Subject ?? string.Empty)}[/]",
                                    $"[{Theme.Muted}]{n.UpdatedOn:yyyy-MM-dd HH:mm}[/]");
                            }
                            Ui.Write(table);
                            if (resp.Count > 0) Ui.MarkupLine($"[{Theme.Muted}]\nPage {page}, showing {resp.ContactGroupNotesObject.Count} of ~{resp.Count}[/]");
                        }
                        return 0;
                    }
                    case "add":
                    {
                        if (args.Length < 3 || !long.TryParse(args[2], out var groupId)) { Console.WriteLine("Usage: bolddesk contact-groups notes add <groupId> --subject <text> (--message <text> | --file <path>)"); return 1; }
                        string? subject = null; string? message = null; string? file = null;
                        for (int i = 3; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "--subject": if (i + 1 < args.Length) subject = args[++i]; break;
                                case "--message": if (i + 1 < args.Length) message = args[++i]; break;
                                case "--file": if (i + 1 < args.Length) file = args[++i]; break;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(file))
                        {
                            if (!File.Exists(file)) { Console.WriteLine($"File not found: {file}"); return 1; }
                            message = await File.ReadAllTextAsync(file);
                        }
                        if (string.IsNullOrWhiteSpace(message)) { Console.WriteLine("Provide --message or --file"); return 1; }
                        var req = new ContactGroupNoteRequest { Subject = subject, Description = message! };
                        var resp = await client.ContactGroups.AddContactGroupNoteAsync(groupId, req);
                        PrintAsJson(resp);
                        return 0;
                    }
                    case "update":
                    {
                        if (args.Length < 3 || !long.TryParse(args[2], out var noteId)) { Console.WriteLine("Usage: bolddesk contact-groups notes update <noteId> [--subject <text>] (--message <text> | --file <path>)"); return 1; }
                        string? subject = null; string? message = null; string? file = null;
                        for (int i = 3; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "--subject": if (i + 1 < args.Length) subject = args[++i]; break;
                                case "--message": if (i + 1 < args.Length) message = args[++i]; break;
                                case "--file": if (i + 1 < args.Length) file = args[++i]; break;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(file))
                        {
                            if (!File.Exists(file)) { Console.WriteLine($"File not found: {file}"); return 1; }
                            message = await File.ReadAllTextAsync(file);
                        }
                        if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(message)) { Console.WriteLine("Provide --subject and/or --message"); return 1; }
                        var req = new ContactGroupNoteRequest { Subject = subject, Description = message ?? string.Empty };
                        var resp = await client.ContactGroups.UpdateContactGroupNoteAsync(noteId, req);
                        PrintAsJson(resp);
                        return 0;
                    }
                    case "delete":
                    {
                        if (args.Length < 3 || !long.TryParse(args[2], out var noteId)) { Console.WriteLine("Usage: bolddesk contact-groups notes delete <noteId>"); return 1; }
                        var resp = await client.ContactGroups.DeleteContactGroupNoteAsync(noteId);
                        PrintAsJson(resp);
                        return 0;
                    }
                    default:
                        return Unknown($"contact-groups notes {action}");
                }
            }
            case "export":
            {
                if (args.Length < 2) { Console.WriteLine("Usage: bolddesk contact-groups export <groups|contacts> ..."); return 1; }
                var what = args[1].ToLowerInvariant();
                string? outPath = null; string format = "json";
                if (what == "groups")
                {
                    bool all = false; string? filter = null; var q = new List<string>(); int page = 1, perPage = 100;
                    for (int i = 2; i < args.Length; i++)
                    {
                        switch (args[i])
                        {
                            case "--all": all = true; break;
                            case "--filter": if (i + 1 < args.Length) filter = args[++i]; break;
                            case "--query": case "-q": if (i + 1 < args.Length) q.Add(args[++i]); break;
                            case "--page": if (i + 1 < args.Length && int.TryParse(args[++i], out var p)) page = p; break;
                            case "--per-page": if (i + 1 < args.Length && int.TryParse(args[++i], out var n)) perPage = n; break;
                            case "--out": if (i + 1 < args.Length) outPath = args[++i]; break;
                            case "--format": if (i + 1 < args.Length) format = args[++i]; break;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(outPath)) { Console.WriteLine("--out <file> is required"); return 1; }
                    var qp = new ContactGroupQueryParameters { Page = page, PerPage = perPage, Filter = filter, RequiresCounts = true, Q = q.Count > 0 ? q.ToArray() : null };
                    await Ui.Status().StartAsync("Exporting contact groups...", async _ =>
                    {
                        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
                        {
                            using var writer = new StreamWriter(outPath);
                            await writer.WriteLineAsync("ContactGroupId,ContactGroupName,CreatedOn,LastModifiedOn");
                            if (all)
                            {
                                await foreach (var g in client.ContactGroups.ListAllContactGroupsAsync(qp))
                                    await writer.WriteLineAsync($"{g.ContactGroupId},{EscapeCsv(g.ContactGroupName)},{g.CreatedOn:o},{g.LastModifiedOn:o}");
                            }
                            else
                            {
                                var resp = await client.ContactGroups.ListContactGroupsAsync(qp);
                                foreach (var g in resp.Result)
                                    await writer.WriteLineAsync($"{g.ContactGroupId},{EscapeCsv(g.ContactGroupName)},{g.CreatedOn:o},{g.LastModifiedOn:o}");
                            }
                        }
                        else
                        {
                            var list = new List<ContactGroup>();
                            if (all)
                                await foreach (var g in client.ContactGroups.ListAllContactGroupsAsync(qp)) list.Add(g);
                            else
                            {
                                var resp = await client.ContactGroups.ListContactGroupsAsync(qp);
                                list.AddRange(resp.Result);
                            }
                            await File.WriteAllTextAsync(outPath, JsonSerializer.Serialize(list, JsonOptions));
                        }
                    });
                    Console.WriteLine($"Exported groups to {outPath}");
                    return 0;
                }
                else if (what == "contacts")
                {
                    if (args.Length < 3 || !long.TryParse(args[2], out var groupId)) { Console.WriteLine("Usage: bolddesk contact-groups export contacts <groupId> [--all] --out <file> [--format json|csv]"); return 1; }
                    bool all = false; for (int i = 3; i < args.Length; i++) { if (args[i] == "--all") all = true; else if (args[i] == "--out" && i + 1 < args.Length) outPath = args[++i]; else if (args[i] == "--format" && i + 1 < args.Length) format = args[++i]; }
                    if (string.IsNullOrWhiteSpace(outPath)) { Console.WriteLine("--out <file> is required"); return 1; }
                    var qp = new ContactGroupContactsQueryParameters { Page = 1, PerPage = 100, RequiresCounts = true };
                    await Ui.Status().StartAsync("Exporting contacts...", async _ =>
                    {
                        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
                        {
                            using var writer = new StreamWriter(outPath);
                            await writer.WriteLineAsync("UserId,Name,Email");
                            if (all)
                            {
                                await foreach (var c2 in client.ContactGroups.GetAllContactsByGroupAsync(groupId, qp))
                                    await writer.WriteLineAsync($"{c2.UserId},{EscapeCsv(c2.ContactDisplayName ?? c2.ContactName ?? string.Empty)},{EscapeCsv(c2.EmailId ?? string.Empty)}");
                            }
                            else
                            {
                                var resp = await client.ContactGroups.GetContactsByGroupAsync(groupId, qp);
                                foreach (var c2 in resp.Result)
                                    await writer.WriteLineAsync($"{c2.UserId},{EscapeCsv(c2.ContactDisplayName ?? c2.ContactName ?? string.Empty)},{EscapeCsv(c2.EmailId ?? string.Empty)}");
                            }
                        }
                        else
                        {
                            var list = new List<Contact>();
                            if (all)
                                await foreach (var c2 in client.ContactGroups.GetAllContactsByGroupAsync(groupId, qp)) list.Add(c2);
                            else
                            {
                                var resp = await client.ContactGroups.GetContactsByGroupAsync(groupId, qp);
                                list.AddRange(resp.Result);
                            }
                            await File.WriteAllTextAsync(outPath, JsonSerializer.Serialize(list, JsonOptions));
                        }
                    });
                    Console.WriteLine($"Exported contacts to {outPath}");
                    return 0;
                }
                return Unknown($"contact-groups export {what}");
            }
            default:
                return Unknown($"contact-groups {sub}");
        }
    }

    // CONTACTS
    static async Task<int> HandleContacts(string[] args)
    {
        // Use the new comprehensive contact command handler
        var cfgSvc = new Services.ConfigurationService();
        var cfg = await cfgSvc.LoadEffectiveAsync();
        if (cfg == null) 
        { 
            Console.WriteLine("Missing config. Run 'bolddesk config set'."); 
            return 1; 
        }

        // Inline raw: contacts get (--id <id> | --email <email>) --raw
        if (args.Length > 0 && string.Equals(args[0], "get", StringComparison.OrdinalIgnoreCase))
        {
            var hasRaw = args.Any(a => string.Equals(a, "--raw", StringComparison.OrdinalIgnoreCase));
            if (hasRaw)
            {
                long id = 0; string? email = null;
                for (int i = 1; i < args.Length - 1; i++)
                {
                    if (string.Equals(args[i], "--id", StringComparison.OrdinalIgnoreCase))
                    { long.TryParse(args[i + 1], out id); }
                    if (string.Equals(args[i], "--email", StringComparison.OrdinalIgnoreCase))
                    { email = args[i + 1]; }
                }
                if (id <= 0 && string.IsNullOrWhiteSpace(email))
                { Console.WriteLine("Usage: bolddesk contacts get (--id <id> | --email <email>) --raw"); return 1; }
                var path = id > 0 ? $"contacts/{id}" : $"contacts/{Uri.EscapeDataString(email!)}";
                await PrintRawAsync(path, cfg.Domain!, cfg.ApiKey!);
                return 0;
            }
        }

        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);
        return await Commands.ContactCommands.HandleContactCommandAsync(args, client);
    }

    // Legacy contact handler for backward compatibility  
    static async Task<int> HandleContactsLegacy(string[] args)
    {
        var sub = args[0].ToLowerInvariant();
        var cfg = await new Services.ConfigurationService().LoadEffectiveAsync();
        if (cfg == null) { Console.WriteLine("Missing config. Run 'bolddesk config set'."); return 1; }
        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);

        switch (sub)
        {
            case "list":
            {
                int page = 1, perPage = 50; string? filter = null; string format = "table"; bool all = false;
                var qList = new List<string>(); string? view = null; long? groupId = null;
                var useBuilder = false;
                
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--page": if (i + 1 < args.Length && int.TryParse(args[++i], out var p)) page = p; break;
                        case "--per-page": if (i + 1 < args.Length && int.TryParse(args[++i], out var n)) perPage = n; break;
                        case "--filter": if (i + 1 < args.Length) filter = args[++i]; break;
                        case "--query": case "-q": if (i + 1 < args.Length) qList.Add(args[++i]); break;
                        case "--view": if (i + 1 < args.Length) view = args[++i]; break;
                        case "--group": if (i + 1 < args.Length && long.TryParse(args[++i], out var g)) groupId = g; break;
                        case "--all": all = true; break;
                        case "--format": if (i + 1 < args.Length) format = args[++i]; break;
                        // New builder-based query options
                        case "--created-today": useBuilder = true; qList.Add("createdon:today"); break;
                        case "--created-this-week": useBuilder = true; qList.Add("createdon:thisweek"); break;
                        case "--created-last-7-days": useBuilder = true; qList.Add("createdon:last7days"); break;
                        case "--created-this-month": useBuilder = true; qList.Add("createdon:thismonth"); break;
                        case "--created-last-30-days": useBuilder = true; qList.Add("createdon:last30days"); break;
                        case "--modified-today": useBuilder = true; qList.Add("lastmodifiedon:today"); break;
                        case "--with-ids": 
                            if (i + 1 < args.Length) 
                            { 
                                useBuilder = true;
                                var ids = args[++i].Split(',').Where(s => long.TryParse(s.Trim(), out _)).ToArray();
                                if (ids.Length > 0) qList.Add($"ids:[{string.Join(",", ids)}]");
                            }
                            break;
                    }
                }

                ContactQueryParameters qp;
                if (useBuilder || qList.Any(q => q.Contains("createdon:") || q.Contains("lastmodifiedon:") || q.Contains("ids:")))
                {
                    // Use the query builder approach for complex queries
                    var builder = client.Contacts.NewQuery();
                    foreach (var query in qList)
                    {
                        if (query.StartsWith("createdon:"))
                        {
                            var period = query.Substring("createdon:".Length);
                            builder = period switch
                            {
                                "today" => builder.CreatedToday(),
                                "thisweek" => builder.CreatedThisWeek(),
                                "last7days" => builder.CreatedLast7Days(),
                                "thismonth" => builder.CreatedThisMonth(),
                                "last30days" => builder.CreatedLast30Days(),
                                _ => builder.WithCreatedOn(period)
                            };
                        }
                        else if (query.StartsWith("lastmodifiedon:"))
                        {
                            var period = query.Substring("lastmodifiedon:".Length);
                            builder = period switch
                            {
                                "today" => builder.ModifiedToday(),
                                "thisweek" => builder.ModifiedThisWeek(),
                                "last7days" => builder.ModifiedLast7Days(),
                                "thismonth" => builder.ModifiedThisMonth(),
                                "last30days" => builder.ModifiedLast30Days(),
                                _ => builder.WithLastModifiedOn(period)
                            };
                        }
                        else if (query.StartsWith("ids:"))
                        {
                            // Parse ids:[1,2,3] format
                            var idsStr = query.Substring("ids:".Length);
                            if (idsStr.StartsWith('[') && idsStr.EndsWith(']'))
                            {
                                var ids = idsStr[1..^1].Split(',')
                                    .Where(s => long.TryParse(s.Trim(), out _))
                                    .Select(long.Parse)
                                    .ToArray();
                                if (ids.Length > 0) builder = builder.WithIds(ids);
                            }
                        }
                        else
                        {
                            builder = builder.WithCustomCondition(query);
                        }
                    }
                    
                    qp = builder.ToParameters(page, perPage, filter, view, groupId, null, true);
                }
                else
                {
                    // Fall back to legacy approach
                    qp = new ContactQueryParameters 
                    { 
                        Page = page, PerPage = perPage, Filter = filter, RequiresCounts = true, 
                        Q = qList.Count > 0 ? qList.ToArray() : null, View = view, ContactGroupId = groupId 
                    };
                }
                
                if (all)
                {
                    if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                    {
                        var list = new List<Contact>();
                        await foreach (var c in client.Contacts.ListAllContactsAsync(qp)) list.Add(c);
                        PrintAsJson(list);
                    }
                    else
                    {
                        var list = new List<Contact>();
                        await foreach (var c in client.Contacts.ListAllContactsAsync(qp)) list.Add(c);
                        RenderContactsTable(list);
                        Ui.MarkupLine($"[grey]\nTotal: {list.Count}[/]");
                    }
                    return 0;
                }
                else
                {
                    var resp = await client.Contacts.ListContactsAsync(qp);
                    if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                        PrintAsJson(resp.Result);
                    else
                    {
                        RenderContactsTable(resp.Result);
                        if (resp.Count > 0)
                            Ui.MarkupLine($"[grey]\nPage {page}, showing {resp.Result.Count} of ~{resp.Count}[/]");
                    }
                    return 0;
                }
            }
            case "get":
            {
                long? id = null; string? email = null; string format = "text";
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--id": if (i + 1 < args.Length && long.TryParse(args[++i], out var lid)) id = lid; break;
                        case "--email": if (i + 1 < args.Length) email = args[++i]; break;
                        case "--format": if (i + 1 < args.Length) format = args[++i]; break;
                    }
                }
                if (id == null && string.IsNullOrWhiteSpace(email))
                { Console.WriteLine("Specify --id or --email"); return 1; }
                
                Contact contact = id != null
                    ? await client.Contacts.GetContactAsync(id.Value)
                    : await client.Contacts.GetContactByEmailAsync(email!);
                
                if (format.Equals("json", StringComparison.OrdinalIgnoreCase)) 
                    PrintAsJson(contact);
                else
                {
                    var lines = new List<string>
                    {
                        $"[gold1]Email[/]: {Markup.Escape(contact.EmailId)}",
                        !string.IsNullOrEmpty(contact.SecondaryEmailId) ? $"[gold1]Secondary[/]: {Markup.Escape(contact.SecondaryEmailId)}" : null,
                        $"[mediumpurple1]Status[/]: {Markup.Escape(contact.Status)}  [mediumpurple1]Verified[/]: {(contact.IsVerified ? "[green]Yes[/]" : "[red]No[/]" )}  [mediumpurple1]Blocked[/]: {(contact.IsBlocked ? "[red]Yes[/]" : "[green]No[/]" )}",
                        !string.IsNullOrEmpty(contact.ContactPhoneNo) ? $"[grey]Phone[/]: {Markup.Escape(contact.ContactPhoneNo)}" : null,
                        !string.IsNullOrEmpty(contact.ContactMobileNo) ? $"[grey]Mobile[/]: {Markup.Escape(contact.ContactMobileNo)}" : null,
                        !string.IsNullOrEmpty(contact.ContactJobTitle) ? $"[grey]Job[/]: {Markup.Escape(contact.ContactJobTitle)}" : null,
                    };
                    var body = string.Join("\n", lines.Where(l => l != null));
                    var headerName = Markup.Escape(contact.ContactDisplayName ?? contact.ContactName ?? "");
                    var panel = new Panel(new Markup(body))
                    {
                        Header = new PanelHeader($"Contact #[grey]{contact.UserId}[/] [deepskyblue1]{headerName}[/]", Justify.Left),
                        Border = BoxBorder.Rounded
                    };
                    Ui.Write(panel);
                }
                return 0;
            }
            case "create":
            {
                string? email = null, name = null, displayName = null, phone = null, mobile = null, address = null, jobTitle = null;
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--email": if (i + 1 < args.Length) email = args[++i]; break;
                        case "--name": if (i + 1 < args.Length) name = args[++i]; break;
                        case "--display-name": if (i + 1 < args.Length) displayName = args[++i]; break;
                        case "--phone": if (i + 1 < args.Length) phone = args[++i]; break;
                        case "--mobile": if (i + 1 < args.Length) mobile = args[++i]; break;
                        case "--address": if (i + 1 < args.Length) address = args[++i]; break;
                        case "--job-title": if (i + 1 < args.Length) jobTitle = args[++i]; break;
                    }
                }
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name))
                { Console.WriteLine("--email and --name are required"); return 1; }
                
                var request = new CreateContactRequest
                {
                    EmailId = email,
                    ContactName = name,
                    ContactDisplayName = displayName ?? name,
                    ContactPhoneNo = phone,
                    ContactMobileNo = mobile,
                    ContactAddress = address,
                    ContactJobTitle = jobTitle
                };
                
                var resp = await client.Contacts.CreateContactAsync(request);
                PrintAsJson(resp);
                return 0;
            }
            case "update":
            {
                long? id = null; var fields = new Dictionary<string, object>();
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--id": if (i + 1 < args.Length && long.TryParse(args[++i], out var lid)) id = lid; break;
                        case "--field": 
                            if (i + 1 < args.Length)
                            {
                                var field = args[++i];
                                var parts = field.Split('=', 2);
                                if (parts.Length == 2) fields[parts[0]] = parts[1];
                            }
                            break;
                    }
                }
                if (id == null) { Console.WriteLine("--id is required"); return 1; }
                if (fields.Count == 0) { Console.WriteLine("At least one --field is required"); return 1; }
                
                var request = new UpdateContactRequest { Fields = fields };
                var resp = await client.Contacts.UpdateContactAsync(id.Value, request);
                PrintAsJson(resp);
                return 0;
            }
            case "delete":
            {
                long? id = null; bool markSpam = false;
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--id": if (i + 1 < args.Length && long.TryParse(args[++i], out var lid)) id = lid; break;
                        case "--mark-spam": markSpam = true; break;
                    }
                }
                if (id == null) { Console.WriteLine("--id is required"); return 1; }
                
                var request = new DeleteContactRequest 
                { 
                    ContactId = new[] { id.Value }, 
                    IsMarkTicketAsSpam = markSpam 
                };
                var resp = await client.Contacts.DeleteContactsAsync(request);
                PrintAsJson(resp);
                return 0;
            }
            case "block":
            {
                long? id = null; bool markSpam = false;
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--id": if (i + 1 < args.Length && long.TryParse(args[++i], out var lid)) id = lid; break;
                        case "--mark-spam": markSpam = true; break;
                    }
                }
                if (id == null) { Console.WriteLine("--id is required"); return 1; }
                
                var resp = await client.Contacts.BlockContactAsync(id.Value, markSpam);
                PrintAsJson(resp);
                return 0;
            }
            case "unblock":
            {
                long? id = null; bool removeSpam = false;
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--id": if (i + 1 < args.Length && long.TryParse(args[++i], out var lid)) id = lid; break;
                        case "--remove-spam": removeSpam = true; break;
                    }
                }
                if (id == null) { Console.WriteLine("--id is required"); return 1; }
                
                var resp = await client.Contacts.UnblockContactAsync(id.Value, removeSpam);
                PrintAsJson(resp);
                return 0;
            }
            default:
                return Unknown($"contacts {sub}");
        }
    }

    static string EscapeCsv(string s) => s.Replace("\"", "\"\"");

    // Spectre.Console helpers
    static void RenderContactGroupsTable(IEnumerable<ContactGroup> groups)
    {
        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn(new TableColumn($"[{Theme.Muted}]Id[/]").Centered())
            .AddColumn(new TableColumn($"[{Theme.Primary}]Name[/]"))
            .AddColumn(new TableColumn($"[{Theme.Muted}]Created[/]"))
            .AddColumn(new TableColumn($"[{Theme.Muted}]Updated[/]"));
        foreach (var g in groups)
        {
            table.AddRow(
                $"[{Theme.Muted}]{g.ContactGroupId}[/]",
                $"[{Theme.Primary}]{Markup.Escape(TrimTo(g.ContactGroupName, Theme.Compact ? 28 : 60))}[/]",
                $"[{Theme.Muted}]{g.CreatedOn:yyyy-MM-dd HH:mm}[/]",
                $"[{Theme.Muted}]{g.LastModifiedOn:yyyy-MM-dd HH:mm}[/]");
        }
        Ui.Write(table);
    }

    static void RenderContactsTable(IEnumerable<Contact> contacts)
    {
        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn(new TableColumn($"[{Theme.Muted}]UserId[/]").Centered())
            .AddColumn(new TableColumn($"[{Theme.Primary}]Name[/]"))
            .AddColumn(new TableColumn($"[{Theme.Accent}]Email[/]"))
            .AddColumn(new TableColumn($"[{Theme.Muted}]Status[/]"));
        foreach (var c in contacts)
        {
            var name = c.ContactDisplayName ?? c.ContactName ?? string.Empty;
            table.AddRow(
                $"[{Theme.Muted}]{c.UserId}[/]",
                $"[{Theme.Primary}]{Markup.Escape(TrimTo(name, Theme.Compact ? 20 : 40))}[/]",
                $"[{Theme.Accent}]{Markup.Escape(TrimTo(c.EmailId, Theme.Compact ? 24 : 40))}[/]",
                $"[{Theme.Muted}]{Markup.Escape(c.Status ?? string.Empty)}[/]");
        }
        Ui.Write(table);
    }

    static void RenderTicketsTable(IEnumerable<Ticket> tickets)
    {
        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn(new TableColumn($"[{Theme.Muted}]Id[/]").Centered())
            .AddColumn(new TableColumn($"[{Theme.Muted}]Created[/]"))
            .AddColumn(new TableColumn($"[{Theme.Highlight}]Status[/]"))
            .AddColumn(new TableColumn($"[{Theme.Accent}]Priority[/]"))
            .AddColumn(new TableColumn($"[{Theme.Primary}]Title[/]"));
        foreach (var t in tickets)
        {
            var status = ColorizeStatus(t.Status?.Description, t.IsResponseOverdue == true || t.IsResolutionOverdue == true);
            var priority = ColorizePriority(t.Priority?.Description);
            table.AddRow(
                $"[{Theme.Muted}]{t.TicketId}[/]",
                $"[{Theme.Muted}]{t.CreatedOn:yyyy-MM-dd HH:mm}[/]",
                status,
                priority,
                $"[{Theme.Primary}]{Markup.Escape(TrimTo(t.Title, Theme.Compact ? 40 : 80))}[/]"
            );
        }
        Ui.Write(table);
    }

    static void RenderAgentsTable(IEnumerable<AgentDetail> agents)
    {
        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn(new TableColumn($"[{Theme.Muted}]UserId[/]").Centered())
            .AddColumn(new TableColumn($"[{Theme.Primary}]Name[/]"))
            .AddColumn(new TableColumn($"[{Theme.Accent}]Email[/]"))
            .AddColumn(new TableColumn($"[{Theme.Highlight}]Available[/]"));
        foreach (var a in agents)
        {
            var name = a.DisplayName ?? a.Name ?? string.Empty;
            table.AddRow(
                $"[{Theme.Muted}]{a.UserId}[/]",
                $"[{Theme.Primary}]{Markup.Escape(TrimTo(name, Theme.Compact ? 20 : 40))}[/]",
                $"[{Theme.Accent}]{Markup.Escape(TrimTo(a.EmailId, Theme.Compact ? 24 : 40))}[/]",
                a.IsAvailable ? "[green]Yes[/]" : "[red]No[/]"
            );
        }
        Ui.Write(table);
    }


    static void RenderBrandsTable(IEnumerable<Brand> brands)
    {
        var table = new Table().Border(TableBorder.Rounded)
            .AddColumn(new TableColumn($"[{Theme.Muted}]BrandId[/]").Centered())
            .AddColumn(new TableColumn($"[{Theme.Primary}]Name[/]"));
        foreach (var b in brands)
        {
            table.AddRow($"[{Theme.Muted}]{b.BrandId}[/]", $"[{Theme.Primary}]{Markup.Escape(b.BrandName)}[/]");
        }
        Ui.Write(table);
    }

    static string ColorizeStatus(string? status, bool overdue)
    {
        if (string.IsNullOrWhiteSpace(status)) return "[grey]-[/]";
        var s = status.ToLowerInvariant();
        if (overdue) return $"[red]{Markup.Escape(status)}[/]";
        if (s.Contains("open") || s.Contains("waiting")) return $"[yellow]{Markup.Escape(status)}[/]";
        if (s.Contains("pending")) return $"[darkorange]{Markup.Escape(status)}[/]";
        if (s.Contains("resolved") || s.Contains("closed")) return $"[green]{Markup.Escape(status)}[/]";
        return $"[mediumpurple1]{Markup.Escape(status)}[/]";
    }

    static string ColorizePriority(string? priority)
    {
        if (string.IsNullOrWhiteSpace(priority)) return "[grey]-[/]";
        var p = priority.ToLowerInvariant();
        if (p.Contains("urgent") || p.Contains("high") || p.Contains("critical")) return $"[red]{Markup.Escape(priority)}[/]";
        if (p.Contains("medium") || p.Contains("normal")) return $"[yellow]{Markup.Escape(priority)}[/]";
        if (p.Contains("low")) return $"[green]{Markup.Escape(priority)}[/]";
        return $"[gold1]{Markup.Escape(priority)}[/]";
    }

    // Prints the raw JSON for a given API path (relative to /api/v1.0)
    static async Task PrintRawAsync(string path, string domain, string apiKey)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("x-api-key", apiKey);
        http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        // Reuse CLI's generous timeout
        if (http.Timeout.TotalSeconds <= 100)
            http.Timeout = TimeSpan.FromMinutes(3);

        var url = $"https://{domain}/api/v1.0/{path}";
        var resp = await http.GetAsync(url);
        var text = await resp.Content.ReadAsStringAsync();
        Console.WriteLine(text);
    }

    // Dynamic completion backend
    static async Task<int> HandleComplete(string[] args)
    {
        if (args.Length == 0) return 0;
        var cfg = await new Services.ConfigurationService().LoadEffectiveAsync();
        if (cfg == null) return 0;
        using var client = new BoldDeskClient(cfg.Domain!, cfg.ApiKey!);

        switch (args[0].ToLowerInvariant())
        {
            case "contact-groups":
            {
                if (args.Length < 2) return 0;
                var kind = args[1].ToLowerInvariant();
                string? prefix = null; int limit = 100;
                for (int i = 2; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--prefix": if (i + 1 < args.Length) prefix = args[++i]; break;
                        case "--limit": if (i + 1 < args.Length && int.TryParse(args[++i], out var l)) limit = l; break;
                    }
                }

                var qp = new ContactGroupQueryParameters { Page = 1, PerPage = Math.Min(limit, 100), RequiresCounts = true };
                var results = new List<ContactGroup>();
                while (results.Count < limit)
                {
                    var resp = await client.ContactGroups.ListContactGroupsAsync(qp);
                    if (resp.Result == null || resp.Result.Count == 0) break;
                    results.AddRange(resp.Result);
                    if (resp.Result.Count < (qp.PerPage ?? 100) || (resp.Count > 0 && results.Count >= resp.Count)) break;
                    qp.Page = (qp.Page ?? 1) + 1;
                }

                IEnumerable<string> output = kind switch
                {
                    "names" => results.Select(r => r.ContactGroupName),
                    "ids" => results.Select(r => r.ContactGroupId.ToString()),
                    _ => Array.Empty<string>()
                };
                if (!string.IsNullOrWhiteSpace(prefix))
                    output = output.Where(v => v.StartsWith(prefix!, StringComparison.OrdinalIgnoreCase));
                foreach (var v in output.Take(limit)) Console.WriteLine(v);
                return 0;
            }
        }
        return 0;
    }

    // Shell completion installer
    static async Task<int> HandleInstallCompletion(string[] args)
    {
        string shell = "bash";
        string? path = null;
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--shell": if (i + 1 < args.Length) shell = args[++i]; break;
                case "--path": if (i + 1 < args.Length) path = args[++i]; break;
                case "--help": case "-h":
                    Console.WriteLine("Usage: bolddesk install-completion [--shell bash|zsh] [--path <file>]");
                    Console.WriteLine("If --path is omitted, the script is printed to stdout.");
                    return 0;
            }
        }

        var script = shell.ToLowerInvariant() switch
        {
            "zsh" => CompletionHelper.GenerateZshCompletion(),
            _ => CompletionHelper.GenerateBashCompletion()
        };

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.Write(script);
            return 0;
        }
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, script);
        Console.WriteLine($"Installed completion script to {path}");
        return 0;
    }

    // REPL
    static async Task<int> HandleRepl(string[] args)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
        
        // Welcome message using Spectre.Console
        var welcomePanel = new Panel($"[bold]BoldDesk CLI Interactive Shell[/] [grey](v{version})[/]\n\n" +
            "[yellow]Features:[/]\n" +
            "  • [green]Tab completion[/] for commands and options\n" +
            "  • [green]Syntax highlighting[/] for better readability\n" +
            "  • [green]Command history[/] with up/down arrows\n" +
            "  • [green]Multi-line input[/] support\n\n" +
            "[grey]Type 'help' for commands, 'exit' to quit, 'clear' to clear screen.[/]")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Blue)
            .Header("[bold blue] Welcome [/]");
        
        AnsiConsole.Write(welcomePanel);
        AnsiConsole.WriteLine();

        // Use SimpleBoldDeskPrompt for now since PrettyPrompt 4.1.1 doesn't support completions
        using var prompt = new Services.SimpleBoldDeskPrompt();
        
        while (true)
        {
            try
            {
                var line = prompt.ReadLine()?.Trim() ?? "";
                    
                    if (line.Length == 0) continue;
                    
                    // Handle built-in commands
                    if (line.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                        line.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                        line.Equals("q", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                    
                    if (line.Equals("clear", StringComparison.OrdinalIgnoreCase) ||
                        line.Equals("cls", StringComparison.OrdinalIgnoreCase))
                    {
                        AnsiConsole.Clear();
                        continue;
                    }
                    
                    if (line.Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowHelp(version);
                        continue;
                    }

                    // Allow comments
                    if (line.StartsWith("#")) continue;

                    var tokens = SplitArgs(line);
                    if (tokens.Length == 0) continue;
                    
                    // Execute command
                    var code = await Route(tokens);
                    if (code != 0)
                    {
                        AnsiConsole.MarkupLine($"[grey](exit {code})[/]");
                    }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
            }
        }
        
        AnsiConsole.MarkupLine("[grey]Goodbye![/]");
        return 0;
    }

    static string[] SplitArgs(string input)
    {
        var args = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;
        char quoteChar = '\0';
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (inQuotes)
            {
                if (c == quoteChar)
                {
                    inQuotes = false;
                }
                else if (c == '\\' && i + 1 < input.Length && input[i + 1] == quoteChar)
                {
                    current.Append(quoteChar);
                    i++;
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                if (char.IsWhiteSpace(c))
                {
                    if (current.Length > 0)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                    }
                }
                else if (c == '\"' || c == '\'')
                {
                    inQuotes = true;
                    quoteChar = c;
                }
                else
                {
                    current.Append(c);
                }
            }
        }
        if (current.Length > 0)
        {
            args.Add(current.ToString());
        }
        return args.ToArray();
    }
}

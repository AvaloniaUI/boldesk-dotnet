using System.Text;
using Spectre.Console;

namespace BoldDesk.Cli.Services;

/// <summary>
/// Simple prompt service with manual tab completion for BoldDesk CLI
/// Since PrettyPrompt 4.1.1 doesn't support completions, we'll use a simpler approach
/// </summary>
public class SimpleBoldDeskPrompt : IDisposable
{
    private readonly List<string> _history = new();
    private int _historyIndex = -1;
    private readonly string _historyFile;
    private bool _disposed;

    // Available commands for completion
    private readonly string[] _mainCommands = 
    {
        "config", "tickets", "brands", "agents", "worklogs", "fields",
        "contact-groups", "contacts", "help", "exit", "quit", "clear"
    };

    private readonly Dictionary<string, string[]> _subCommands = new()
    {
        ["config"] = new[] { "set", "get", "test" },
        ["tickets"] = new[] { "list", "get", "create", "update", "reply", "close", "assign", "help" },
        ["brands"] = new[] { "list", "get", "help" },
        ["agents"] = new[] { "list", "get", "create", "update", "help" },
        ["worklogs"] = new[] { "list", "count", "export", "help" },
        ["fields"] = new[] { "list-options", "add-options", "remove-option", "set-default", "remove-default", "help" },
        ["contact-groups"] = new[] { "list", "get", "create", "update", "help" },
        ["contacts"] = new[] { "list", "get", "create", "update", "delete", "block", "unblock", "help" }
    };

    public SimpleBoldDeskPrompt()
    {
        _historyFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".bolddesk_history");

        // Load history
        if (File.Exists(_historyFile))
        {
            _history.AddRange(File.ReadAllLines(_historyFile).Where(l => !string.IsNullOrWhiteSpace(l)));
        }
    }

    public string ReadLine()
    {
        var input = new StringBuilder();
        var position = 0;

        AnsiConsole.Markup("[blue]bolddesk>[/] ");

        while (true)
        {
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    Console.WriteLine();
                    var result = input.ToString();
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        _history.Add(result);
                        _historyIndex = _history.Count;
                        SaveHistory();
                    }
                    return result;

                case ConsoleKey.Tab:
                    HandleTabCompletion(input, ref position);
                    break;

                case ConsoleKey.Backspace:
                    if (position > 0)
                    {
                        input.Remove(position - 1, 1);
                        position--;
                        RedrawLine(input.ToString(), position);
                    }
                    break;

                case ConsoleKey.Delete:
                    if (position < input.Length)
                    {
                        input.Remove(position, 1);
                        RedrawLine(input.ToString(), position);
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (position > 0)
                    {
                        position--;
                        Console.SetCursorPosition(9 + position, Console.CursorTop);
                    }
                    break;

                case ConsoleKey.RightArrow:
                    if (position < input.Length)
                    {
                        position++;
                        Console.SetCursorPosition(9 + position, Console.CursorTop);
                    }
                    break;

                case ConsoleKey.UpArrow:
                    if (_historyIndex > 0)
                    {
                        _historyIndex--;
                        input.Clear();
                        input.Append(_history[_historyIndex]);
                        position = input.Length;
                        RedrawLine(input.ToString(), position);
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (_historyIndex < _history.Count - 1)
                    {
                        _historyIndex++;
                        input.Clear();
                        input.Append(_history[_historyIndex]);
                        position = input.Length;
                        RedrawLine(input.ToString(), position);
                    }
                    else if (_historyIndex == _history.Count - 1)
                    {
                        _historyIndex = _history.Count;
                        input.Clear();
                        position = 0;
                        RedrawLine("", 0);
                    }
                    break;

                case ConsoleKey.Home:
                    position = 0;
                    Console.SetCursorPosition(9, Console.CursorTop);
                    break;

                case ConsoleKey.End:
                    position = input.Length;
                    Console.SetCursorPosition(9 + position, Console.CursorTop);
                    break;

                default:
                    if (!char.IsControl(key.KeyChar))
                    {
                        input.Insert(position, key.KeyChar);
                        position++;
                        RedrawLine(input.ToString(), position);
                    }
                    break;
            }
        }
    }

    private void HandleTabCompletion(StringBuilder input, ref int position)
    {
        var text = input.ToString();
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (words.Length == 0 || (words.Length == 1 && !text.EndsWith(' ')))
        {
            // Complete main command
            var partial = words.Length > 0 ? words[0] : "";
            var matches = _mainCommands.Where(c => c.StartsWith(partial, StringComparison.OrdinalIgnoreCase)).ToArray();
            
            if (matches.Length == 1)
            {
                // Single match - complete it
                input.Clear();
                input.Append(matches[0]);
                if (text.EndsWith(' '))
                    input.Append(' ');
                position = input.Length;
                RedrawLine(input.ToString(), position);
            }
            else if (matches.Length > 1)
            {
                // Multiple matches - show them
                Console.WriteLine();
                AnsiConsole.MarkupLine("[dim]Available commands:[/]");
                foreach (var match in matches)
                {
                    AnsiConsole.MarkupLine($"  [cyan]{match}[/]");
                }
                Console.Write("\r");
                AnsiConsole.Markup("[blue]bolddesk>[/] ");
                Console.Write(text);
            }
        }
        else if (words.Length >= 1 && _subCommands.ContainsKey(words[0].ToLowerInvariant()))
        {
            // Complete subcommand
            var mainCmd = words[0].ToLowerInvariant();
            var partial = words.Length > 1 && !text.EndsWith(' ') ? words[1] : "";
            var matches = _subCommands[mainCmd].Where(c => c.StartsWith(partial, StringComparison.OrdinalIgnoreCase)).ToArray();
            
            if (matches.Length == 1)
            {
                // Single match - complete it
                input.Clear();
                input.Append(words[0]);
                input.Append(' ');
                input.Append(matches[0]);
                if (text.EndsWith(' '))
                    input.Append(' ');
                position = input.Length;
                RedrawLine(input.ToString(), position);
            }
            else if (matches.Length > 1)
            {
                // Multiple matches - show them
                Console.WriteLine();
                AnsiConsole.MarkupLine($"[dim]Available subcommands for [cyan]{mainCmd}[/]:[/]");
                foreach (var match in matches)
                {
                    AnsiConsole.MarkupLine($"  [cyan]{match}[/]");
                }
                Console.Write("\r");
                AnsiConsole.Markup("[blue]bolddesk>[/] ");
                Console.Write(text);
            }
        }
    }

    private void RedrawLine(string text, int position)
    {
        Console.Write("\r");
        AnsiConsole.Markup("[blue]bolddesk>[/] ");
        Console.Write(text);
        Console.Write(new string(' ', Console.BufferWidth - 9 - text.Length - 1));
        Console.SetCursorPosition(9 + position, Console.CursorTop);
    }

    private void SaveHistory()
    {
        try
        {
            File.WriteAllLines(_historyFile, _history.TakeLast(100));
        }
        catch
        {
            // Ignore history save errors
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            SaveHistory();
            _disposed = true;
        }
    }
}
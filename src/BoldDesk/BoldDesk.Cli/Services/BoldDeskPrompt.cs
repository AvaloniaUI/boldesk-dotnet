using PrettyPrompt;
using PrettyPrompt.Highlighting;

namespace BoldDesk.Cli.Services;

/// <summary>
/// Enhanced prompt service using PrettyPrompt for rich input with Spectre.Console for output
/// </summary>
public class BoldDeskPrompt : IDisposable
{
    private readonly Prompt _prompt;
    private bool _disposed;
    private static readonly string HistoryFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
        ".bolddesk_history");

    public BoldDeskPrompt()
    {
        var configuration = new PromptConfiguration(
            prompt: new FormattedString("bolddesk> ", 
                new FormatSpan(0, 8, AnsiColor.BrightBlue), 
                new FormatSpan(8, 2, AnsiColor.White)),
            completionItemDescriptionPaneBackground: AnsiColor.Rgb(30, 30, 30),
            selectedCompletionItemBackground: AnsiColor.Rgb(30, 30, 30),
            selectedTextBackground: AnsiColor.Rgb(20, 61, 102)
        );

        _prompt = new Prompt(
            persistentHistoryFilepath: HistoryFilePath,
            configuration: configuration
        );
    }

    /// <summary>
    /// Read a line with rich editing capabilities
    /// </summary>
    public async Task<PromptResult> ReadLineAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _prompt.ReadLineAsync().ConfigureAwait(false);
            
            // Debug: Log what was entered (you can set breakpoints here)
            if (!string.IsNullOrEmpty(result.Text))
            {
                Console.WriteLine($"[DEBUG] Entered: '{result.Text}'");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] PrettyPrompt error: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _prompt.DisposeAsync();
            _disposed = true;
        }
    }
}
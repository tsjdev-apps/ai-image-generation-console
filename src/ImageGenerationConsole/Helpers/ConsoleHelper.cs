using Spectre.Console;

namespace ImageGenerationConsole.Helpers;

/// <summary>
/// Provides utility methods for building interactive console UIs using Spectre.Console.
/// </summary>
internal static class ConsoleHelper
{
    /// <summary>
    /// Clears the console and displays a stylized header with title and author information.
    /// </summary>
    public static void ShowHeader()
    {
        AnsiConsole.Clear();

        Grid grid = new();
        grid.AddColumn();

        grid.AddRow(new FigletText("Image Generation").Centered().Color(Color.Red));
        grid.AddRow(
            Align.Center(
                new Panel("[red]Sample by Thomas Sebastian Jensen " +
                "([link]https://www.tsjdev-apps.de[/])[/]")));

        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Prompts the user to select a single option from a list.
    /// </summary>
    /// <param name="options">The list of selectable options.</param>
    /// <param name="prompt">The prompt message shown to the user.</param>
    /// <param name="showHeader">Whether to display the header before prompting.</param>
    /// <returns>The selected option.</returns>
    public static string SelectFromOptions(
        List<string> options,
        string prompt,
        bool showHeader = true)
    {
        if (showHeader)
        {
            ShowHeader();
        }

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(prompt)
                .AddChoices(options));
    }

    /// <summary>
    /// Prompts the user to select multiple options from a list.
    /// At least one option must be selected.
    /// </summary>
    /// <param name="options">The list of selectable options.</param>
    /// <param name="prompt">The prompt message shown to the user.</param>
    /// <param name="showHeader">Whether to display the header before prompting.</param>
    /// <returns>The list of selected options.</returns>
    public static List<string> SelectMultipleFromOptions(
        List<string> options,
        string prompt,
        bool showHeader = true)
    {
        if (showHeader)
        {
            ShowHeader();
        }

        return AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title(prompt)
                .Required()
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]")
                .AddChoices(options));
    }

    /// <summary>
    /// Prompts the user for a valid HTTPS URL with validation.
    /// </summary>
    /// <param name="prompt">The prompt message shown to the user.</param>
    /// <param name="showHeader">Whether to display the header before prompting.</param>
    /// <returns>A validated HTTPS URL as a string.</returns>
    public static string GetUrlFromConsole(
        string prompt,
        bool showHeader = true)
    {
        if (showHeader)
        {
            ShowHeader();
        }

        return AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .PromptStyle("white")
                .ValidationErrorMessage("[red]Please enter a valid HTTPS URL[/]")
                .Validate(input =>
                {
                    if (input.Length < 3)
                        return ValidationResult.Error("[red]URL too short[/]");

                    if (input.Length > 250)
                        return ValidationResult.Error("[red]URL too long[/]");

                    if (Uri.TryCreate(input, UriKind.Absolute, out Uri? uri)
                        && uri.Scheme == Uri.UriSchemeHttps)
                        return ValidationResult.Success();

                    return ValidationResult.Error("[red]URL must start with https://[/]");
                }));
    }

    /// <summary>
    /// Prompts the user to enter a string with optional length validation.
    /// </summary>
    /// <param name="prompt">The message shown to the user.</param>
    /// <param name="validateLength">Whether to enforce a max length of 200 characters.</param>
    /// <param name="showHeader">Whether to display the header before prompting.</param>
    /// <returns>The validated input string.</returns>
    public static string GetStringFromConsole(
        string prompt,
        bool validateLength = true,
        bool showHeader = true)
    {
        if (showHeader)
        {
            ShowHeader();
        }

        // Allow empty prompt for cases where prompt is displayed separately
        if (string.IsNullOrEmpty(prompt))
        {
            return AnsiConsole.Ask<string>("> ");
        }

        return AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .PromptStyle("white")
                .ValidationErrorMessage("[red]Please enter a valid input[/]")
                .Validate(input =>
                {
                    if (input.Length < 3)
                        return ValidationResult.Error("[red]Input too short[/]");

                    if (validateLength && input.Length > 200)
                        return ValidationResult.Error("[red]Input too long[/]");

                    return ValidationResult.Success();
                }));
    }

    /// <summary>
    /// Prompts the user to enter a secret (e.g., API key) with masked input.
    /// The input is displayed as asterisks for security.
    /// </summary>
    /// <param name="prompt">The message shown to the user.</param>
    /// <param name="showHeader">Whether to display the header before prompting.</param>
    /// <returns>The secret input string.</returns>
    public static string GetSecretFromConsole(
        string prompt,
        bool showHeader = true)
    {
        if (showHeader)
        {
            ShowHeader();
        }

        return AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .PromptStyle("white")
                .Secret()
                .ValidationErrorMessage("[red]Please enter a valid API key[/]")
                .Validate(input =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                        return ValidationResult.Error("[red]API key cannot be empty[/]");

                    if (input.Length < 10)
                        return ValidationResult.Error("[red]API key too short[/]");

                    return ValidationResult.Success();
                }));
    }

    /// <summary>
    /// Displays an error message in red color.
    /// </summary>
    /// <param name="errorMessage">The error message to display.</param>
    public static void WriteError(string errorMessage)
    {
        AnsiConsole.MarkupLine($"[red]{errorMessage}[/]");
    }

    /// <summary>
    /// Displays a general message in white color.
    /// </summary>
    /// <param name="message">The message to display.</param>
    public static void WriteMessage(string message)
    {
        AnsiConsole.MarkupLine($"[white]{message}[/]");
    }
}

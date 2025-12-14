using Azure.AI.OpenAI;
using ImageGenerationConsole.Helpers;
using OpenAI;
using OpenAI.Images;
using Spectre.Console;
using System.ClientModel;
using System.Diagnostics;
using static ImageGenerationConsole.Utils.Statics;

#pragma warning disable OPENAI001

// Display application header
ConsoleHelper.ShowHeader();

// Prompt user to select AI provider (Azure OpenAI or OpenAI)
string provider = ConsoleHelper.SelectFromOptions(
    [Providers.AzureOpenAI, Providers.OpenAI],
    Prompts.SelectProvider);

// Initialize client variables
OpenAIClient? openAiClient = null;
Dictionary<string, (string deploymentName, ImageGenerationOptions options)> modelConfigurations = [];

// Configure Azure OpenAI provider
if (provider == Providers.AzureOpenAI)
{
    try
    {
        // Collect Azure OpenAI credentials
        string azureAiEndpoint = ConsoleHelper.GetUrlFromConsole(
            Prompts.EnterAzureEndpoint);
        string azureAiKey = ConsoleHelper.GetSecretFromConsole(
            Prompts.EnterAzureApiKey);

        // Initialize Azure OpenAI client
        openAiClient = new AzureOpenAIClient(
            new Uri(azureAiEndpoint),
            new ApiKeyCredential(azureAiKey));
    }
    catch (Exception ex)
    {
        ExitWithError(string.Format(ErrorMessages.ErrorInitializingAzureClient, ex.Message));
        return;
    }

    // Collect deployment names and model types
    ConsoleHelper.ShowHeader();
    AnsiConsole.MarkupLine(Prompts.EnterDeployments);
    AnsiConsole.WriteLine();

    string deploymentInput = ConsoleHelper.GetStringFromConsole(
        string.Empty,
        validateLength: false,
        showHeader: false);

    var deployments = deploymentInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    // Parse and configure each deployment
    foreach (var deployment in deployments)
    {
        var parts = deployment.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        // Validate deployment format
        if (parts.Length != 2)
        {
            ConsoleHelper.WriteError(string.Format(ErrorMessages.InvalidDeploymentFormat, deployment));
            continue;
        }

        string deploymentName = parts[0];
        string modelType = parts[1].ToLowerInvariant();

        // Get model-specific image generation options
        ImageGenerationOptions? options = GetImageGenerationOptions(modelType);

        // Validate model type
        if (options == null)
        {
            ConsoleHelper.WriteError(string.Format(ErrorMessages.UnknownModelType, modelType, deploymentName));
            continue;
        }

        modelConfigurations[deploymentName] = (deploymentName, options);
    }

    // Ensure at least one valid deployment was configured
    if (modelConfigurations.Count == 0)
    {
        ExitWithError(ErrorMessages.NoValidDeployments);
        return;
    }
}
// Configure OpenAI provider
else
{
    try
    {
        // Collect OpenAI API key
        string openAiKey = ConsoleHelper.GetSecretFromConsole(
            Prompts.EnterOpenAIApiKey,
            showHeader: true);

        // Initialize OpenAI client
        openAiClient = new OpenAIClient(openAiKey);
    }
    catch (Exception ex)
    {
        ExitWithError(string.Format(ErrorMessages.ErrorInitializingOpenAIClient, ex.Message));
        return;
    }

    // Allow user to select which models to use
    List<string> availableModels = [ModelTypes.DallE3, ModelTypes.GptImage1, ModelTypes.GptImage1Mini];
    List<string> selectedModels = ConsoleHelper.SelectMultipleFromOptions(
        availableModels,
        Prompts.SelectModels,
        showHeader: true);

    // Ensure at least one model is selected
    if (selectedModels.Count == 0)
    {
        ExitWithError(ErrorMessages.NoModelsSelected);
        return;
    }

    // Configure options for each selected model
    foreach (var model in selectedModels)
    {
        ImageGenerationOptions? options = GetImageGenerationOptions(model);

        if (options != null)
        {
            modelConfigurations[model] = (model, options);
        }
    }
}

// Collect image generation prompt from user
string imagePrompt = ConsoleHelper.GetStringFromConsole(
    Prompts.EnterImagePrompt,
    validateLength: false);

// Clear console and show generation status header
ConsoleHelper.ShowHeader();
ConsoleHelper.WriteMessage(Messages.StartingGeneration);
ConsoleHelper.WriteMessage(string.Empty);

// Track success and failure counts
int successCount = 0;
int failureCount = 0;

// Generate images for each configured model
foreach (var (modelKey, (deploymentName, options)) in modelConfigurations)
{
    try
    {
        // Display generation status with animated spinner
        await AnsiConsole.Status()
            .StartAsync(string.Format(Messages.GeneratingImage, modelKey), async ctx =>
            {
                try
                {
                    // Start timer for generation duration
                    var stopwatch = Stopwatch.StartNew();

                    // Get appropriate image client
                    ImageClient imageClient = openAiClient!.GetImageClient(deploymentName);

                    // Generate image using the API
                    ClientResult<GeneratedImage> imageResult =
                        await imageClient.GenerateImageAsync(imagePrompt, options);

                    stopwatch.Stop();

                    // Display generation time
                    ConsoleHelper.WriteMessage(string.Format(Messages.GenerationCompleted, stopwatch.ElapsedMilliseconds, stopwatch.Elapsed.TotalSeconds));

                    // Save the generated image to a temporary file
                    await SaveImageToTempFileAsync(imageResult.Value, modelKey);

                    successCount++;
                }
                catch (ClientResultException ex)
                {
                    // Handle API-specific errors
                    failureCount++;
                    HandleApiError(ex, modelKey, deploymentName);
                }
                catch (Exception ex)
                {
                    // Handle unexpected errors
                    failureCount++;
                    ConsoleHelper.WriteError(string.Format(ErrorMessages.UnexpectedError, modelKey, ex.Message));
                }
            });
    }
    catch (Exception ex)
    {
        // Handle errors during status display initialization
        failureCount++;
        ConsoleHelper.WriteError(string.Format(ErrorMessages.FailedToStartGeneration, modelKey, ex.Message));
    }

    ConsoleHelper.WriteMessage(string.Empty);
}

// Display generation summary
ConsoleHelper.WriteMessage(string.Empty);

if (successCount > 0)
{
    ConsoleHelper.WriteMessage(string.Format(Messages.ImagesGeneratedSuccessfully, successCount));
}

if (failureCount > 0)
{
    ConsoleHelper.WriteError(string.Format(Messages.ImagesFailedToGenerate, failureCount));
}

ConsoleHelper.WriteMessage(string.Empty);
ConsoleHelper.WriteMessage(Messages.PressKeyToExit);
Console.ReadKey();

/// <summary>
/// Gets the image generation options for a specific model type.
/// </summary>
/// <param name="modelType">The model type identifier.</param>
/// <returns>The configured ImageGenerationOptions, or null if the model type is unknown.</returns>
static ImageGenerationOptions? GetImageGenerationOptions(string modelType)
{
    return modelType switch
    {
        ModelTypes.DallE3 => new()
        {
            Size = GeneratedImageSize.W1792xH1024,
            Quality = GeneratedImageQuality.Standard,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Uri
        },
        ModelTypes.GptImage1 => new()
        {
            Size = GeneratedImageSize.W1536xH1024
        },
        ModelTypes.GptImage1Mini => new()
        {
            Size = GeneratedImageSize.W1536xH1024,
            Quality = GeneratedImageQuality.Medium
        },
        _ => null
    };
}

/// <summary>
/// Handles API-specific errors with detailed messages for common HTTP status codes.
/// </summary>
/// <param name="ex">The ClientResultException to handle.</param>
/// <param name="modelKey">The model key for error reporting.</param>
/// <param name="deploymentName">The deployment name for error reporting.</param>
static void HandleApiError(ClientResultException ex, string modelKey, string deploymentName)
{
    ConsoleHelper.WriteError(string.Format(ErrorMessages.ApiError, modelKey, ex.Message));

    switch (ex.Status)
    {
        case 401:
            ConsoleHelper.WriteError(ErrorMessages.AuthenticationFailed);
            break;
        case 429:
            ConsoleHelper.WriteError(ErrorMessages.RateLimitExceeded);
            break;
        case 404:
            ConsoleHelper.WriteError(string.Format(ErrorMessages.ModelNotFound, deploymentName));
            break;
    }
}

/// <summary>
/// Displays an error message and waits for user input before exiting.
/// </summary>
/// <param name="errorMessage">The error message to display.</param>
static void ExitWithError(string errorMessage)
{
    ConsoleHelper.WriteError(errorMessage);
    ConsoleHelper.WriteMessage(Messages.PressKeyToExit);
    Console.ReadKey();
}

/// <summary>
/// Saves a generated image to a temporary file with timestamp-based naming.
/// Handles both URL-based and byte-based image data.
/// </summary>
/// <param name="image">The generated image to save.</param>
/// <param name="modelName">The model name used for file naming.</param>
static async Task SaveImageToTempFileAsync(GeneratedImage image, string modelName)
{
    try
    {
        // Create unique filename with timestamp
        var tempPath = Path.Combine(Path.GetTempPath(), $"{modelName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        byte[] imageBytes;

        // Download image from URL if provided
        if (image.ImageUri != null)
        {
            try
            {
                using HttpClient httpClient = new();
                imageBytes = await httpClient.GetByteArrayAsync(image.ImageUri);
            }
            catch (HttpRequestException ex)
            {
                ConsoleHelper.WriteError(string.Format(ErrorMessages.ErrorDownloadingImage, ex.Message));
                return;
            }
        }
        // Use byte data if provided
        else if (image.ImageBytes != null)
        {
            imageBytes = image.ImageBytes.ToArray();
        }
        // No image data available
        else
        {
            ConsoleHelper.WriteError(string.Format(ErrorMessages.NoImageData, modelName));
            return;
        }

        // Save image to file system
        await File.WriteAllBytesAsync(tempPath, imageBytes);

        // Display clickable file path
        AnsiConsole.MarkupLine(string.Format(Messages.ImageSaved, tempPath));
    }
    catch (UnauthorizedAccessException ex)
    {
        // Handle file permission errors
        ConsoleHelper.WriteError(string.Format(ErrorMessages.AccessDenied, modelName, ex.Message));
    }
    catch (IOException ex)
    {
        // Handle file I/O errors
        ConsoleHelper.WriteError(string.Format(ErrorMessages.FailedToSaveImage, modelName, ex.Message));
    }
    catch (Exception ex)
    {
        // Handle unexpected errors during file save
        ConsoleHelper.WriteError(string.Format(ErrorMessages.UnexpectedSaveError, modelName, ex.Message));
    }
}

#pragma warning restore OPENAI001

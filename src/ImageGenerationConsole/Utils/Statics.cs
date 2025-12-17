namespace ImageGenerationConsole.Utils;

/// <summary>
/// Contains static string constants used throughout the application.
/// Centralizes all user-facing messages for consistency and easy localization.
/// </summary>
internal static class Statics
{
    /// <summary>
    /// Provider names for AI services.
    /// </summary>
    public static class Providers
    {
        /// <summary>
        /// Azure OpenAI provider name.
        /// </summary>
        public const string AzureOpenAI
            = "Azure OpenAI";

        /// <summary>
        /// OpenAI provider name.
        /// </summary>
        public const string OpenAI
            = "OpenAI";
    }

    /// <summary>
    /// Model type identifiers for image generation models.
    /// </summary>
    public static class ModelTypes
    {
        /// <summary>
        /// DALL·E 3 model type identifier.
        /// </summary>
        public const string DallE3
            = "dall-e-3";

        /// <summary>
        /// GPT-Image-1 model type identifier.
        /// </summary>
        public const string GptImage1
            = "gpt-image-1";

        /// <summary>
        /// GPT-Image-1-Mini model type identifier.
        /// </summary>
        public const string GptImage1Mini
            = "gpt-image-1-mini";

        /// <summary>
        /// GPT-Image-1.5 model type identifier.
        /// </summary>
        public const string GptImage15
            = "gpt-image-1.5";
    }

    /// <summary>
    /// User prompt messages for input collection.
    /// </summary>
    public static class Prompts
    {
        /// <summary>
        /// Prompt to select the AI service provider.
        /// </summary>
        public const string SelectProvider
            = "[yellow]Select your provider:[/]";

        /// <summary>
        /// Prompt to enter Azure OpenAI endpoint.
        /// </summary>
        public const string EnterAzureEndpoint
            = "[yellow]Enter your Azure OpenAI endpoint:[/]";

        /// <summary>
        /// Prompt to enter API key for the Azure OpenAI provider.
        /// </summary>
        public const string EnterAzureApiKey
            = "[yellow]Enter your Azure OpenAI API key:[/]";

        /// <summary>
        /// Prompt to enter OpenAI API key.
        /// </summary>
        public const string EnterOpenAIApiKey
            = "[yellow]Enter your OpenAI API key:[/]";

        /// <summary>
        /// Prompt to enter the image generation prompt.
        /// </summary>
        public const string EnterImagePrompt
            = "[yellow]Enter your image prompt:[/]";

        /// <summary>
        /// Prompt to select models for image generation.
        /// </summary>
        public const string SelectModels
            = "[yellow]Select the models you want to use (Space to select, Enter to confirm):[/]";

        /// <summary>
        /// Prompt to enter Azure OpenAI deployment names and model types.
        /// </summary>
        public const string EnterDeployments
            = "[yellow]Enter your Azure OpenAI deployment names " +
              "(format: 'deploymentName:modelType', comma-separated):[/]\n" +
              "[dim]Example: myDallE:dall-e-3,myGptImage:gpt-image-1[/]\n" +
              "[dim]Supported model types: dall-e-3, gpt-image-1, gpt-image-1-mini, gpt-image-1.5[/]";
    }

    /// <summary>
    /// Success and informational messages.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// Message indicating the start of image generation.
        /// </summary>
        public const string StartingGeneration
            = "[green]Starting image generation...[/]";

        /// <summary>
        /// Prompt to press any key to exit the application.
        /// </summary>
        public const string PressKeyToExit
            = "Press any key to exit...";

        /// <summary>
        /// Message indicating completion of image generation with timing details.
        /// </summary>
        public const string GenerationCompleted
            = "[green]✓[/] Generation completed in {0} ms ({1:F2} seconds)";

        /// <summary>
        /// Message indicating that an image has been saved successfully.
        /// </summary>
        public const string ImageSaved
            = "[green]✓[/] Image saved: [link]{0}[/]";

        /// <summary>
        /// Message indicating the number of images generated successfully.
        /// </summary>
        public const string ImagesGeneratedSuccessfully
            = "[green]✓ {0} image(s) generated successfully![/]";

        /// <summary>
        /// Message indicating the number of images that failed to generate.
        /// </summary>
        public const string ImagesFailedToGenerate
            = "[red]✗ {0} image(s) failed to generate.[/]";

        /// <summary>
        /// Message indicating the image generation process for a specific model.
        /// </summary>
        public const string GeneratingImage
            = "[yellow]Generating image using {0}...[/]";
    }

    /// <summary>
    /// Error messages for various failure scenarios.
    /// </summary>
    public static class ErrorMessages
    {
        /// <summary>
        /// Message indicating an error initializing the Azure OpenAI client.
        /// </summary>
        public const string ErrorInitializingAzureClient
            = "Error initializing Azure OpenAI client: {0}";

        /// <summary>
        /// Message indicating an error initializing the OpenAI client.
        /// </summary>
        public const string ErrorInitializingOpenAIClient
            = "Error initializing OpenAI client: {0}";

        /// <summary>
        /// Message indicating an invalid deployment format.
        /// </summary>
        public const string InvalidDeploymentFormat
            = "Invalid format for deployment: '{0}'. " +
              "Expected format: 'deploymentName:modelType'. Skipping...";

        /// <summary>
        /// Message indicating an unknown model type.
        /// </summary>
        public const string UnknownModelType
            = "Unknown model type: '{0}'. " +
              "Supported types: dall-e-3, gpt-image-1, " +
              "gpt-image-1-mini. Skipping deployment '{1}'...";

        /// <summary>
        /// Message indicating no valid deployments were configured.
        /// </summary>
        public const string NoValidDeployments
            = "No valid deployments configured. " +
              "Please check your input format and try again.";

        /// <summary>
        /// Message indicating no models were selected.
        /// </summary>
        public const string NoModelsSelected
            = "No models selected. " +
              "Please select at least one model.";

        /// <summary>
        /// Message indicating an API error during image generation.
        /// </summary>
        public const string ApiError
            = "[red]✗[/] API error for {0}: {1}";

        /// <summary>
        /// Message indicating an unexpected error during image generation.
        /// </summary>
        public const string UnexpectedError
            = "[red]✗[/] Unexpected error for {0}: {1}";

        /// <summary>
        /// Message indicating failure to start image generation.
        /// </summary>
        public const string FailedToStartGeneration
            = "[red]✗[/] Failed to start generation for {0}: {1}";

        /// <summary>
        /// Message indicating authentication failure.
        /// </summary>
        public const string AuthenticationFailed
            = "Authentication failed. Please check your API key.";

        /// <summary>
        /// Message indicating rate limit has been exceeded.
        /// </summary>
        public const string RateLimitExceeded
            = "Rate limit exceeded. Please try again later.";

        /// <summary>
        /// Message indicating the specified model or deployment was not found.
        /// </summary>
        public const string ModelNotFound
            = "Model or deployment '{0}' not found.";

        /// <summary>
        /// Message indicating an error downloading the image from a URL.
        /// </summary>
        public const string ErrorDownloadingImage
            = "Error downloading image from URL: {0}";

        /// <summary>
        /// Message indicating no image data is available.
        /// </summary>
        public const string NoImageData
            = "Error: No image data available for {0}.";

        /// <summary>
        /// Message indicating access denied when saving an image.
        /// </summary>
        public const string AccessDenied
            = "Error: Access denied when saving image for {0}: {1}";

        /// <summary>
        /// Message indicating failure to save an image.
        /// </summary>
        public const string FailedToSaveImage
            = "Error: Failed to save image for {0}: {1}";

        /// <summary>
        /// Message indicating an unexpected error occurred while saving an image.
        /// </summary>
        public const string UnexpectedSaveError
            = "Error: Unexpected error saving image for {0}: {1}";
    }
}

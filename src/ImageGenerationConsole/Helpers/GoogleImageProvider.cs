using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using static ImageGenerationConsole.Utils.Statics;

namespace ImageGenerationConsole.Helpers;

internal static class GoogleImageProvider
{
    /// <summary>
    /// Generates an image using Google AI Gemini image models via REST.
    /// </summary>
    /// <param name="apiKey">The Google AI API key.</param>
    /// <param name="model">The Gemini model identifier.</param>
    /// <param name="prompt">The image prompt.</param>
    /// <returns>Image bytes for the generated image.</returns>
    public static async Task<byte[]> GenerateImageAsync(string apiKey, string model, string prompt)
    {
        using HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

        GoogleGenerateContentRequest request = new([
            new GoogleRequestContent([
                new GoogleRequestPart(prompt)
            ])
        ]);

        JsonSerializerOptions serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent",
            request,
            serializerOptions);

        response.EnsureSuccessStatusCode();

        GoogleGenerateContentResponse? responseContent = await response.Content.ReadFromJsonAsync<GoogleGenerateContentResponse>(serializerOptions);

        GoogleInlineData? inlineData = responseContent?.Candidates?
            .SelectMany(candidate => candidate.Content?.Parts ?? [])
            .Select(part => part.InlineData)
            .FirstOrDefault(data => data != null && !string.IsNullOrWhiteSpace(data.Data));

        if (inlineData?.Data == null)
        {
            throw new InvalidOperationException(ErrorMessages.GoogleNoImageData);
        }

        try
        {
            return Convert.FromBase64String(inlineData.Data);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(ErrorMessages.GoogleInvalidResponse, ex);
        }
    }

    /// <summary>
    /// Saves Google AI image bytes to a temporary file with timestamp-based naming.
    /// </summary>
    /// <param name="imageBytes">The image bytes.</param>
    /// <param name="modelName">The model name used for file naming.</param>
    public static async Task SaveImageToTempFileAsync(byte[] imageBytes, string modelName)
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"{modelName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            await File.WriteAllBytesAsync(tempPath, imageBytes);

            AnsiConsole.MarkupLine(string.Format(Messages.ImageSaved, tempPath));
        }
        catch (UnauthorizedAccessException ex)
        {
            ConsoleHelper.WriteError(string.Format(ErrorMessages.AccessDenied, modelName, ex.Message));
        }
        catch (IOException ex)
        {
            ConsoleHelper.WriteError(string.Format(ErrorMessages.FailedToSaveImage, modelName, ex.Message));
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError(string.Format(ErrorMessages.UnexpectedSaveError, modelName, ex.Message));
        }
    }

    private sealed record class GoogleGenerateContentRequest(
        [property: JsonPropertyName("contents")] List<GoogleRequestContent> Contents);

    private sealed record class GoogleRequestContent(
        [property: JsonPropertyName("parts")] List<GoogleRequestPart> Parts);

    private sealed record class GoogleRequestPart(
        [property: JsonPropertyName("text")] string Text);

    private sealed record class GoogleGenerateContentResponse(
        [property: JsonPropertyName("candidates")] List<GoogleCandidate>? Candidates);

    private sealed record class GoogleCandidate(
        [property: JsonPropertyName("content")] GoogleContent? Content);

    private sealed record class GoogleContent(
        [property: JsonPropertyName("parts")] List<GoogleContentPart>? Parts);

    private sealed record class GoogleContentPart(
        [property: JsonPropertyName("inlineData")] GoogleInlineData? InlineData);

    private sealed record class GoogleInlineData(
        [property: JsonPropertyName("mimeType")] string? MimeType,
        [property: JsonPropertyName("data")] string? Data);
}

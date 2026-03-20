namespace UrlShortener.Domain.Config;

public class GeminiSettings
{
    public const string SectionName = "Gemini";

    public string ApiKey { get; init; } = string.Empty;
}
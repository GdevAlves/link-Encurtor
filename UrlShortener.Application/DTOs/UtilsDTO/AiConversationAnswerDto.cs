namespace UrlShortener.Application.DTOs.UtilsDTO;

public sealed class AiConversationAnswerDto
{
    public required Guid ConversationId { get; init; }
    public required string Answer { get; init; }
}


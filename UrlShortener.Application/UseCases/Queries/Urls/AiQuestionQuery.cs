using UrlShortener.Application.Abstractions;

namespace UrlShortener.Application.UseCases.Queries.Urls;

public class AiQuestionQuery(string question, Guid? conversationId = null) : IQuery<IResult>
{
    public required string Question { get; init; } = question;
    public Guid? ConversationId { get; init; } = conversationId;
}
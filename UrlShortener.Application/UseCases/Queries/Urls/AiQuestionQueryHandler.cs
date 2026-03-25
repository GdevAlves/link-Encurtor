using System.Text.Json;
using Flunt.Notifications;
using Flunt.Validations;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.AI;
using UrlShortener.Application.DTOs.UtilsDTO;
using UrlShortener.Application.Enums;
using UrlShortener.Application.UseCases.Commands;
using UrlShortener.Domain.IRepositories;
using UrlShortener.Domain.IServices;
using Abstractions_IResult = UrlShortener.Application.Abstractions.IResult;

namespace UrlShortener.Application.UseCases.Queries.Urls;

public class AiQuestionQueryHandler(
    AgentFactory agentFactory,
    ICurrentUserService currentUserService,
    IAnalyticsConversationSessionRepository sessionRepository
) : IQueryHandler<AiQuestionQuery, Abstractions_IResult>
{
    public async ValueTask<Abstractions_IResult> Handle(AiQuestionQuery query, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotNullOrWhiteSpace(query.Question, "Question", "A pergunta é obrigatória.");

        if (!contract.IsValid)
            return new Result(ResultStatus.ValidationError, false, "Validação falhou", contract.Notifications);

        var userId = currentUserService.GetUserId();
        if (userId == Guid.Empty)
            return new Result(ResultStatus.Unauthorized, false, "Usuário não autenticado.");

        var conversationId = query.ConversationId ?? Guid.CreateVersion7();

        JsonElement? currentState = null;
        var serializedSession = await sessionRepository.GetSessionAsync(userId, conversationId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(serializedSession))
            currentState = JsonSerializer.Deserialize<JsonElement>(serializedSession);

        var agent = agentFactory.CreateUrlAnalyticsAgent();

        var response = await agent.GetInsightsAsync(userId, query.Question, currentState, cancellationToken);

        await sessionRepository.SaveSessionAsync(userId, conversationId, response.Session.GetRawText(), cancellationToken);

        return new Result(ResultStatus.Success, true, "success", new AiConversationAnswerDto
        {
            ConversationId = conversationId,
            Answer = response.Answer
        });
    }
}
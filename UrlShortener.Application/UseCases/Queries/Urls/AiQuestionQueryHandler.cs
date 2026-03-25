using Flunt.Notifications;
using Flunt.Validations;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.AI;
using UrlShortener.Application.Enums;
using UrlShortener.Application.UseCases.Commands;
using UrlShortener.Domain.IServices;
using Abstractions_IResult = UrlShortener.Application.Abstractions.IResult;
using IResult = UrlShortener.Application.Abstractions.IResult;

namespace UrlShortener.Application.UseCases.Queries.Urls;

public class AiQuestionQueryHandler(
    AgentFactory agentFactory,
    ICurrentUserService currentUserService
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

        var agent = agentFactory.CreateUrlAnalyticsAgent();

        var response = await agent.GetInsightsAsync(userId, query.Question, query.CurrentState, cancellationToken);
        
        return new Result(ResultStatus.Success, true, "success", response);
    }
}
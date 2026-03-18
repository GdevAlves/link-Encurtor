using System.Net;
using Flunt.Notifications;
using Flunt.Validations;
using URLapi.Application.Abstractions;
using URLapi.Application.AI;
using URLapi.Application.UseCases.Commands;
using URLapi.Domain.IServices;
using IResult = URLapi.Application.Abstractions.IResult;

namespace URLapi.Application.UseCases.Queries.Urls;

public class AiQuestionQueryHandler(
    AgentFactory agentFactory,
    ICurrentUserService currentUserService
) : IQueryHandler<AiQuestionQuery, IResult>
{
    public async ValueTask<IResult> Handle(AiQuestionQuery query, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notification>()
            .Requires()
            .IsNotNullOrWhiteSpace(query.Question, "Question", "A pergunta é obrigatória.");

        if (!contract.IsValid)
            return new Result(HttpStatusCode.BadRequest, false, "Validação falhou", contract.Notifications);

        var userId = currentUserService.GetUserId();
        if (userId == Guid.Empty)
            return new Result(HttpStatusCode.Unauthorized, false, "Usuário não autenticado.");

        var agent = agentFactory.CreateUrlAnalyticsAgent();

        var response = await agent.GetInsightsAsync(userId, query.Question, query.CurrentState, cancellationToken);
        
        return new Result(HttpStatusCode.OK, true, "success", response);
    }
}
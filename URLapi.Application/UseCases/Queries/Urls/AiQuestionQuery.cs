using System.Text.Json;
using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Queries.Urls;

public class AiQuestionQuery(string question, JsonElement? currentState) : IQuery<IResult>
{
    public required string Question { get; init; } = question;
    public JsonElement? CurrentState { get; init; } = currentState;
}
using URLapi.Application.Abstractions;

namespace URLapi.Application.UseCases.Queries.Urls;

public class AiQuestionQuery(string question) : IQuery<IResult>
{
    public required string Question { get; init; } = question;
}

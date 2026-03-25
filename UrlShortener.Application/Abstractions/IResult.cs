using UrlShortener.Application.Enums;
using UrlShortener.Application.UseCases.Commands;

namespace UrlShortener.Application.Abstractions;

public interface IResult
{
    Result GetResult();
    object? GetData();
    ResultStatus GetStatus();
    string GetMessage();
}
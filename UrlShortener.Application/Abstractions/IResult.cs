using System.Net;
using UrlShortener.Application.UseCases.Commands;

namespace UrlShortener.Application.Abstractions;

public interface IResult
{
    Result GetResult();
    object? GetData();
    HttpStatusCode GetStatusCode();
    string GetMessage();
}
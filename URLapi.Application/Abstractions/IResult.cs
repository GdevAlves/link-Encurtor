using System.Net;
using URLapi.Application.UseCases.Commands;

namespace URLapi.Application.Abstractions;

public interface IResult
{
    Result GetResult();
    object? GetData();
    HttpStatusCode GetStatusCode();
    string GetMessage();
}
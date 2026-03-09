using System.Net;
using URLapi.Domain.UseCases.Commands;

namespace URLapi.Domain.Abstractions;

public interface IResult
{
    Result GetResult();
    object? GetData();
    HttpStatusCode GetStatusCode();
    string GetMessage();
}
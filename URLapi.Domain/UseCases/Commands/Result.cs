using System.Net;
using URLapi.Domain.Abstractions;

namespace URLapi.Domain.UseCases.Commands;

public class Result(
    HttpStatusCode httpStatusCode,
    bool success,
    string? message = null,
    object? data = null)
    : IResult
{
    public HttpStatusCode HttpStatusCode { get; set; } = httpStatusCode;
    public bool Success { get; } = success;
    public string Message { get; } = message ?? string.Empty;
    public object? Data { get; } = data;

    public object? GetData()
    {
        return Data;
    }

    public string GetMessage()
    {
        return Message;
    }

    public Result GetResult()
    {
        return this;
    }

    public HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode;
    }
}
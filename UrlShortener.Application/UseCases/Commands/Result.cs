using UrlShortener.Application.Abstractions;
using UrlShortener.Application.Enums;

namespace UrlShortener.Application.UseCases.Commands;

public class Result(
    ResultStatus status,
    bool success,
    string? message = null,
    object? data = null)
    : IResult
{
    public ResultStatus Status { get; } = status;
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

    public ResultStatus GetStatus()
    {
        return Status;
    }
}
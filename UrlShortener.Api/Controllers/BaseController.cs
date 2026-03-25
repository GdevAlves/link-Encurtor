using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Enums;
using IResult = UrlShortener.Application.Abstractions.IResult;

namespace UrlShortener.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult(IResult result, bool isCreatedResource = false)
    {
        var res = result.GetResult();
        var statusCode = MapResultStatusToHttpStatusCode(res.Status, isCreatedResource);

        return StatusCode(statusCode,
            !res.Success ? new { message = res.Message, data = res.Data } : res.Data);
    }

    private static int MapResultStatusToHttpStatusCode(ResultStatus status, bool isCreatedResource = false)
    {
        return status switch
        {
            ResultStatus.Success => isCreatedResource ? StatusCodes.Status201Created : StatusCodes.Status200OK,
            ResultStatus.ValidationError => StatusCodes.Status400BadRequest,
            ResultStatus.NotFound => StatusCodes.Status404NotFound,
            ResultStatus.Unauthorized => StatusCodes.Status401Unauthorized,
            ResultStatus.Forbidden => StatusCodes.Status403Forbidden,
            ResultStatus.Conflict => StatusCodes.Status409Conflict,
            ResultStatus.InternalError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
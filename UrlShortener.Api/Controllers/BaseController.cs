using Microsoft.AspNetCore.Mvc;
using IResult = UrlShortener.Application.Abstractions.IResult;

namespace UrlShortener.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult(IResult result)
    {
        var res = result.GetResult();
        return StatusCode((int)res.HttpStatusCode,
            !res.Success ? new { message = res.Message, data = res.Data } : res.Data);
    }
}
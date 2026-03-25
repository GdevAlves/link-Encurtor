using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.UseCases.Commands.Urls;
using UrlShortener.Application.UseCases.Queries.Urls;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("v1/url")]
public class UrlController(IMediator mediator) : BaseController
{
    [Authorize]
    [HttpPost("")]
    public async Task<IActionResult> Post(
        [FromBody] CreateUrlCommand command
    )
    {
        var result = await mediator.Send(command);
        return HandleResult(result, isCreatedResource: true);
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> Get(string shortUrl)
    {
        var query = new GetBigUrlByShortUrlQuery(shortUrl)
        {
            ShortUrl = shortUrl
        };
        var result = await mediator.Send(query);

        if (!result.GetResult().Success)
            return NotFound(new { message = "URL not found", shortUrl });

        var longUrl = result.GetData()?.ToString();
        if (string.IsNullOrEmpty(longUrl))
            return NotFound(new { message = "URL not found", shortUrl });

        // Ensure URL has a scheme
        if (!longUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !longUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            longUrl = "https://" + longUrl;

        // HTTP 302 Redirect (standard for URL shorteners)
        return Redirect(longUrl);
    }

    [Authorize]
    [HttpGet("{shortUrl}/info")]
    public async Task<IActionResult> GetInfo(string shortUrl)
    {
        var query = new GetUrlInfoByShortUrlQuery(shortUrl)
        {
            ShortUrl = shortUrl
        };
        var result = await mediator.Send(query);
        return HandleResult(result);
    }
    
    [Authorize]
    [HttpGet("{userId:guid}/urls")]
    public async Task<IActionResult> GetById(
        Guid userId,
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var query = new GetUsersUrlsByUserIdQuery(userId, page, pageSize)
        {
            Id = userId,
            Page = page,
            PageSize = pageSize
        };
        var result = await mediator.Send(query);
        return HandleResult(result);
    }
    
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteUrlCommand
        {
            Id = id
        };
        var result = await mediator.Send(command);
        return HandleResult(result);
    }

    // TODO Rotas/Controller de analytcs 
    [Authorize]
    [HttpPost("ai")]
    public async Task<IActionResult> AiQuestion([FromBody] AiQuestionQuery query)
    {
        var result = await mediator.Send(query);
        return HandleResult(result);
    }
}
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.UseCases.Commands.Users;
using UrlShortener.Application.UseCases.Queries.Users;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("v1/user")]
public class UserController(IMediator mediator) : BaseController
{
    [HttpPost("")]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result, isCreatedResource: true);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { UserId = userId };
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Update(
        Guid userId,
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        command.UserId = userId;
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> Delete(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { UserId = userId };
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("verify-{token:guid}")]
    public async Task<IActionResult> VerifyToken(
        Guid token,
        CancellationToken cancellationToken
    )
    {
        var command = new VerifyUserCommand
        {
            Token = token
        };
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
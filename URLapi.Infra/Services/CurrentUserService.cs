using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using URLapi.Domain.IServices;

namespace URLapi.Infra.Services;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid GetUserId()
    {
        var principal = accessor.HttpContext?.User;
        var userId =
            principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }

    public string GetUserEmail()
    {
        return accessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public bool IsAuthenticated()
    {
        return accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
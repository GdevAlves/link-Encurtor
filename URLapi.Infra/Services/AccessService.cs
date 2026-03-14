using Microsoft.AspNetCore.Http;
using URLapi.Domain.IServices;

namespace URLapi.Infra.Services;

public class AccessService(IHttpContextAccessor accessor) : IAccessService
{
    public string? GetIpAddress()
    {
        return accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }

    public string GetReferer()
    {
        accessor.HttpContext.Request.Headers.TryGetValue("Referer", out var referrer);
        if (string.IsNullOrEmpty(referrer.ToString()))
            referrer = accessor.HttpContext.Request.Headers["Referer"].ToString();
        return referrer;
    }

    public string? GetUserAgent()
    {
        accessor.HttpContext.Request.Headers.TryGetValue("User-Agent", out var userAgent);
        if (string.IsNullOrEmpty(userAgent.ToString())) userAgent = accessor.HttpContext.Request.Headers.UserAgent;
        return userAgent;
    }

    public string GetCountry()
    {
        // é necessário uma API externa para geoloc
        throw new NotImplementedException();
    }

    public string GetCity()
    {
        throw new NotImplementedException();
    }
}
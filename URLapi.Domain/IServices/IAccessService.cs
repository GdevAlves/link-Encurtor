namespace URLapi.Domain.IServices;

public interface IAccessService
{
    string? GetIpAddress();
    string GetReferer();
    string? GetUserAgent();
    string GetCountry();
    string GetCity();
}
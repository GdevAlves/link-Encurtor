namespace UrlShortener.Domain.IServices;

public interface ICurrentUserService
{
    Guid GetUserId();
    string GetUserEmail();
    bool IsAuthenticated();
}
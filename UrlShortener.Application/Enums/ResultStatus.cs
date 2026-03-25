namespace UrlShortener.Application.Enums;

public enum ResultStatus
{
    Success,
    ValidationError,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    InternalError
}

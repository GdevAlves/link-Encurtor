namespace UrlShortener.Domain.Entities;

public class UrlAccessLog : Entity
{
    private UrlAccessLog(Url url)
    {
    }

    public UrlAccessLog()
    {
    }

    public Guid UrlId { get; set; }
    public Url Url { get; set; }
    public DateTime AccessedAt { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    public string? Referer { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
}
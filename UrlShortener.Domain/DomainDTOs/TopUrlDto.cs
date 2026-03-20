namespace UrlShortener.Domain.DomainDTOs;

public class TopUrlDto
{
    public Guid UrlId { get; set; }
    public string ShortUrl { get; set; }
    public string LongUrl { get; set; }
    public int AccessCount { get; set; }
    public DateTime LastAccessed { get; set; }
}
namespace UrlShortener.Domain.Entities;

public class Url : Entity
{
    private Url()
    {
    }

    public Url(User creator, string longUrl, string shortUrl)
    {
        Creator = creator;
        LongUrl = longUrl;
        ShortUrl = shortUrl;
    }

    public User Creator { get; set; }
    public string LongUrl { get; set; }
    public string ShortUrl { get; set; }
    public int AccessCount { get; private set; }

    public void IncrementAccessCount()
    {
        AccessCount++;
    }
}
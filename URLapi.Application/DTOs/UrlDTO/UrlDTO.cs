namespace URLapi.Application.DTOs.UrlDTO;

public class UrlDTO
{
    public Guid Id { get; set; }
    public string LongUrl { get; set; }
    public string ShortUrl { get; set; }
    public int AccessCount { get; set; }
}
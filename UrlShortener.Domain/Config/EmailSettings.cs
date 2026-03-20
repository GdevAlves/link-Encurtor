namespace UrlShortener.Domain.Config;

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; init; }
    public string SmtpPassword { get; init; }
}
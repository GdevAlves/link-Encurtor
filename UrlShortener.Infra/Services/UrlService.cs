using System.Text;
using UrlShortener.Domain.IServices;

namespace UrlShortener.Infra.Services;

public class UrlService : IUrlService
{
    private readonly int _size = 16;

    public string GenerateShortUrl()
    {
        const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < _size; i++)
        {
            var index = random.Next(caracteres.Length); // Escolhe caractere aleatório [8]
            stringBuilder.Append(caracteres[index]);
        }


        return stringBuilder.ToString();
    }
}
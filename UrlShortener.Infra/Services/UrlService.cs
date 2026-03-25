using System.Security.Cryptography;
using System.Text;
using UrlShortener.Domain.IServices;

namespace UrlShortener.Infra.Services;

public class UrlService : IUrlService
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const int ShortUrlLength = 7;

    public string GenerateShortUrl()
    {
        // Combina timestamp com bytes aleatórios criptograficamente seguros para reduzir colisões
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var randomBytes = RandomNumberGenerator.GetBytes(4);

        // Combina timestamp com random bytes para criar um valor único
        var combinedValue = timestamp ^ BitConverter.ToUInt32(randomBytes, 0);

        return EncodeBase62(combinedValue, ShortUrlLength);
    }

    private static string EncodeBase62(long value, int minLength)
    {
        if (value == 0)
            return new string(Base62Chars[0], minLength);

        var result = new StringBuilder();
        var absValue = Math.Abs(value);

        while (absValue > 0)
        {
            var remainder = (int)(absValue % 62);
            result.Insert(0, Base62Chars[remainder]);
            absValue /= 62;
        }

        // Preenche com caracteres aleatórios criptograficamente seguros se necessário
        while (result.Length < minLength)
        {
            var randomIndex = RandomNumberGenerator.GetInt32(Base62Chars.Length);
            result.Insert(0, Base62Chars[randomIndex]);
        }

        return result.ToString();
    }
}
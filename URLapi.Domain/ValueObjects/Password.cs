using URLapi.Domain.IServices;

namespace URLapi.Domain.ValueObjects;

public class Password : ValueObject
{
    private Password()
    {
    }

    private Password(string hash)
    {
        Hash = hash;
    }

    public string Hash { get; } = string.Empty;

    public static Password FromPlainText(string text, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Password cannot be empty.", nameof(text));

        if (hasher is null) throw new ArgumentNullException(nameof(hasher));

        return new Password(hasher.Hash(text));
    }

    public bool Verify(string text, IPasswordHasher hasher)
    {
        if (hasher is null) throw new ArgumentNullException(nameof(hasher));

        return hasher.Verify(text, Hash);
    }
}
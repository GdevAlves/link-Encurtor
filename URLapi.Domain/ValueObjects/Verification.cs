namespace URLapi.Domain.ValueObjects;

public class Verification : ValueObject
{
    // Construtor privado para o EF Core usar ao reconstruir do banco
    private Verification()
    {
    }

    // Construtor público para criar nova instância
    public Verification(bool createNew = true)
    {
        if (!createNew) return;
        Verified = false;
        VerifyHashCode = Guid.NewGuid();
        VerifyHashCodeExpiration = DateTime.UtcNow.AddHours(5);
    }

    public bool Verified { get; set; }
    public Guid VerifyHashCode { get; private set; }
    public DateTime VerifyHashCodeExpiration { get; set; }
}
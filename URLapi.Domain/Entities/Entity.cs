namespace URLapi.Domain.Entities;

public abstract class Entity
{
    public Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime ModifiedAt { get; protected set; }

    public bool Equals(Guid id)
    {
        return Id == id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
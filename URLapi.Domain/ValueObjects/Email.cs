namespace URLapi.Domain.ValueObjects;

public class Email : ValueObject
{
    private Email()
    {
    }

    public Email(string address)
    {
        Address = address;
        Verification = new Verification();
    }

    public string Address { get; }

    public Verification Verification { get; private set; }

    public override string ToString()
    {
        return Address;
    }
}
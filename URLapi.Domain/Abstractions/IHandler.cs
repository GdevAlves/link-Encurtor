namespace URLapi.Domain.Abstractions;

public interface IHandler<T> where T : ICommand
{
    Task<IResult> Handle(T command, CancellationToken cancellationToken);
}
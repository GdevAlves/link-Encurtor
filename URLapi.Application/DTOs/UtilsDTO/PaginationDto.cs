namespace URLapi.Application.DTOs.UtilsDTO;

public class PaginationDto<T>
{
    public T? Data { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
}
namespace MovieSearchCase.Domain.Entities;

public class PagedResult<T>
{

    public int Page { get; init; }
    public int TotalPages { get; init; }
    public int TotalResults { get; init; }
    public IReadOnlyList<T> Results { get; init; } = [];
}


namespace MovieSearchCase.WebApi.Models.Movies;

/// <summary>
/// API response shape for a movie. Deliberately separate from
/// <see cref="Domain.Entities.Movie"/> so the wire format can evolve independently
/// of the domain model.
/// </summary>
public record Movie
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public string? Overview { get; init; }

    public string? PosterPath { get; init; }

    public string? BackdropPath { get; init; }

    public double VoteAverage { get; init; }

    public DateOnly? ReleaseDate { get; init; }
}
//forp2
public record MovieDetails : Movie
{
    public int? Runtime { get; init; }
    public string? Tagline { get; init; }
    public IReadOnlyList<string> Genres { get; init; } = new List<string>();
}



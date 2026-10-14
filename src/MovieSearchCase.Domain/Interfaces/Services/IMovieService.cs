using MovieSearchCase.Domain.Entities;

namespace MovieSearchCase.Domain.Interfaces.Services;

/// <summary>
/// TMDB-backed movie lookups used by the API.
/// </summary>
public interface IMovieService
{
    Task<IReadOnlyList<Movie>> GetTrendingAsync(CancellationToken cancellationToken);

    // TODO(candidate): add a search method here (query + page) and implement it on
    // MovieService by calling ITmdbClient's new search method.
    Task<PagedResult<Movie>> GetSearchAsync(string query, int page, CancellationToken cancellationToken);
    Task<Movie> GetMovieDetailsAsync(int movieId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MovieVideo>> GetMovieVideosAsync(
    int movieId,
    CancellationToken cancellationToken);

}

using Microsoft.Extensions.Caching.Memory;
using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Interfaces.Clients;
using MovieSearchCase.Domain.Interfaces.Services;

namespace MovieSearchCase.Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly ITmdbClient _tmdbClient;
    private readonly IMemoryCache _cache;

    public MovieService(ITmdbClient tmdbClient, IMemoryCache cache)
    {
        _tmdbClient = tmdbClient;
        _cache = cache;
    }


    public Task<IReadOnlyList<Movie>> GetTrendingAsync(CancellationToken cancellationToken) =>
        _tmdbClient.GetTrendingMoviesAsync(cancellationToken);

    // TODO(candidate): add a search method here that calls ITmdbClient's new search method.
    //public Task<PagedResult<Movie>> GetSearchAsync(string query, int page, CancellationToken cancellationToken) =>
    //    _tmdbClient.GetSearchMoviesAsync(query, page, cancellationToken);

    public async Task<PagedResult<Movie>> GetSearchAsync(
    string query,
    int page,
    CancellationToken cancellationToken)
    {
        var trimmedQuery = query.Trim();
        var cacheKey = $"movies:search:{page}:{trimmedQuery}";


        // Return  saved one if this search is cached.
        if (_cache.TryGetValue<PagedResult<Movie>>(cacheKey, out var cached)
            && cached is not null)
        {
            return cached;
        }

        // else,  results from TMDB.
        var result = await _tmdbClient.GetSearchMoviesAsync(
            trimmedQuery,
            page,
            cancellationToken);

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
    public Task<Movie> GetMovieDetailsAsync(int movieId, CancellationToken cancellationToken) =>
        _tmdbClient.GetMovieDetailsAsync(movieId, cancellationToken);


    public Task<IReadOnlyList<MovieVideo>> GetMovieVideosAsync(int movieId, CancellationToken cancellationToken) =>
    _tmdbClient.GetMovieVideosAsync(movieId, cancellationToken);
}

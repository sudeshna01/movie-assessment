using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Exceptions;
using MovieSearchCase.Domain.Interfaces.Clients;
using System.Net.Http.Json;

namespace MovieSearchCase.Infrastructure.Clients;

public class TmdbClient : ITmdbClient
{
    private readonly HttpClient _httpClient;

    public TmdbClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }



    public async Task<IReadOnlyList<Movie>> GetTrendingMoviesAsync(CancellationToken cancellationToken)
    {
        TmdbPagedResponse<TmdbMovie>? response;

        try
        {
            response = await _httpClient.GetFromJsonAsync<TmdbPagedResponse<TmdbMovie>>(
                "trending/movie/week",
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB did not return trending movies.");
        }

        if (response is null)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB returned an empty trending movies response.");
        }

        return response.Results.Select(TmdbMovieMapper.ToDomainModel).ToList();
    }

    // TODO(candidate): implement TMDB's /search/movie?query={query}&page={page} here,
    // following the same try/catch + mapping pattern as GetTrendingMoviesAsync above.
    public async Task<PagedResult<Movie>> GetSearchMoviesAsync(string query, int page, CancellationToken cancellationToken)
    {
        TmdbPagedResponse<TmdbMovie>? response;
        var SearchUrl = $"search/movie?query={Uri.EscapeDataString(query.Trim())}&page={page}";
        try
        {
            response = (await _httpClient.GetFromJsonAsync<TmdbPagedResponse<TmdbMovie>>(SearchUrl, cancellationToken));

        }
        catch (OperationCanceledException)
              when (!cancellationToken.IsCancellationRequested)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB did not respond in time.");
        }
        catch (HttpRequestException)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB did not return any search movies.");
        }

        if (response is null)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB returned an empty search response.");
        }
        return new PagedResult<Movie>
        {
            Page = response.Page,
            TotalPages = response.TotalPages,
            TotalResults = response.TotalResults,
            Results = response.Results.Select(TmdbMovieMapper.ToDomainModel).ToList()
        };

    }
    //movie details endpoint: https://api.themoviedb.org/3/movie/{movie_id}?api_key=<<api_key>>&language=en-US
    public async Task<Movie> GetMovieDetailsAsync(int movieId, CancellationToken cancellationToken)
    {
        TmdbMovieDetails? response;
        var movieDetailsUrl = $"movie/{movieId}";

        try
        {
            response = await _httpClient.GetFromJsonAsync<TmdbMovieDetails>(
                movieDetailsUrl,
                cancellationToken);
        }
        catch (HttpRequestException exception)
    when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.EntityNotFound,
                "The requested movie does not exist.");
        }
        catch (HttpRequestException)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB is unavailable.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB did not respond in time.");
        }


        if (response is null)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB returned an empty movie details response.");
        }

        // If ToDetailsDomainModel() returns Movie:
        return response.ToDetailsDomainModel();

        // If ToDetailsDomainModel() returns Task<Movie>, instead use:
        // return await response.ToDetailsDomainModel();
    }

    public async Task<IReadOnlyList<MovieVideo>> GetMovieVideosAsync(int movieId, CancellationToken cancellationToken)
    {
        TmdbVideosResponse? response;
        var movieVideosUrl = $"movie/{movieId}/videos";
        try
        {
            response = await _httpClient
         .GetFromJsonAsync<TmdbVideosResponse>(
             $"movie/{movieId}/videos",
             cancellationToken);
        }
        catch (HttpRequestException exception)
    when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.EntityNotFound,
                "The requested movie does not exist.");
        }
        catch (HttpRequestException)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB is unavailable.");
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB did not respond in time.");
        }

        if (response is null)
        {
            throw new MovieException(
                MovieException.ExceptionTitle,
                ErrorType.UpstreamServiceUnavailable,
                "TMDB returned an empty videos response.");
        }
        return response.Results
        .Select(video => video.ToVideoDomainModel())
        .ToList();
    }
}

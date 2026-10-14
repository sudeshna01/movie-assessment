using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.Shared;

namespace MovieSearchCase.WebApi.Handlers.Movies;

public class MoviesRequestHandlerFactory : IMoviesRequestHandlerFactory
{
    private readonly IMovieService _movieService;

    public MoviesRequestHandlerFactory(IMovieService movieService)
    {
        _movieService = movieService;
    }


    public IRequestHandlerAsync GetTrendingMovies() => new GetTrendingMoviesHandler(_movieService);
    public IRequestHandlerAsync SearchMovies(string query, int page, CancellationToken cancellationToken) => new SearchMovieHandler(_movieService, query, page);

    public IRequestHandlerAsync GetMovies(int id, CancellationToken cancellationToken, bool includeVideo = false) =>
     new GetMovieDetailsHandler(_movieService, id, includeVideo);
}

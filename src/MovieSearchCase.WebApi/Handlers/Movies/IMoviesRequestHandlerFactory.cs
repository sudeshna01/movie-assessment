using MovieSearchCase.Shared;

namespace MovieSearchCase.WebApi.Handlers.Movies;

public interface IMoviesRequestHandlerFactory
{
    IRequestHandlerAsync GetTrendingMovies();

    // TODO(candidate): add a SearchMovies(string query, int page) method here once you've
    // added the search handler, then wire it up in MoviesRequestHandlerFactory below.
    IRequestHandlerAsync SearchMovies(string query, int page, CancellationToken cancellationToken);
    IRequestHandlerAsync GetMovies(int Id, CancellationToken cancellationToken, bool includeVideo = false);

}

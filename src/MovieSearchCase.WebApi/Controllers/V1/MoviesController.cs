using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MovieSearchCase.WebApi.Application.Versioning;
using MovieSearchCase.WebApi.Handlers.Movies;
using Swashbuckle.AspNetCore.Annotations;

namespace MovieSearchCase.WebApi.Controllers.V1;

[ApiVersion("1.0")]
[VersionedRoute]
public class MoviesController : ApiControllerBase
{
    private readonly IMoviesRequestHandlerFactory _requestHandlerFactory;

    public MoviesController(IMoviesRequestHandlerFactory requestHandlerFactory)
    {
        _requestHandlerFactory = requestHandlerFactory;
    }

    /// <summary>
    /// Returns this week's TMDB-backed trending movies.
    /// </summary>
    [HttpGet("trending")]
    [SwaggerOperation(Summary = "Get trending movies", OperationId = "GetTrendingMovies")]
    [SwaggerResponse(StatusCodes.Status200OK, "This week's trending movies", typeof(IEnumerable<Models.Movies.Movie>))]
    public async Task<IActionResult> GetTrending() =>
        await _requestHandlerFactory.GetTrendingMovies().HandleAsync(Request);

    // TODO(candidate): add a GET "search" action here (query + page querystring params),
    // following the same one-liner pattern as GetTrending — resolve a handler from the
    // factory and call HandleAsync(Request). See Handlers/Movies/GetTrendingMoviesHandler.cs
    // for what the handler itself should look like.

    /// </summary>
    [HttpGet("search")]
    [SwaggerOperation(Summary = "Search a Movie", OperationId = "SearchMovie")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paginated search results",
    typeof(MovieSearchCase.Domain.Entities.PagedResult<Models.Movies.Movie>))]
    public async Task<IActionResult> GetSearch([FromQuery] string query, CancellationToken cancellationToken, [FromQuery] int page = 1) =>
        await _requestHandlerFactory.SearchMovies(query, page, cancellationToken).HandleAsync(Request);

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get movie details", OperationId = "GetMovieDetails")]
    [SwaggerResponse(StatusCodes.Status200OK, "Movie details", typeof(Models.Movies.MovieDetails))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Movie not found")]

    public async Task<IActionResult> GetMovieDetails([FromRoute] int id, CancellationToken cancellationToken,
        [FromQuery] bool includeVideos = false) =>
    await _requestHandlerFactory
        .GetMovies(id, cancellationToken, includeVideos)
        .HandleAsync(Request);
}



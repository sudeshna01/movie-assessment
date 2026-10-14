using Microsoft.AspNetCore.Mvc;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.Shared;
using MovieSearchCase.WebApi.Mappers;

namespace MovieSearchCase.WebApi.Handlers.Movies;

public class GetMovieDetailsHandler : IRequestHandlerAsync
{
    private readonly IMovieService _movieService;
    private readonly int _movieId;
    private readonly bool _includeVideo;

    public GetMovieDetailsHandler(
        IMovieService movieService,
        int movieId,
        bool includeVideo = false)
    {
        _movieService = movieService;
        _movieId = movieId;
        _includeVideo = includeVideo;
    }

    public async Task<IActionResult> HandleAsync(HttpRequest request)
    {
        var movie = await _movieService.GetMovieDetailsAsync(
            _movieId,
            request.HttpContext.RequestAborted);

        var details = movie.ToDetailsApiModel();

        if (!_includeVideo)
        {
            return new OkObjectResult(details);
        }

        var videos = await _movieService.GetMovieVideosAsync(
            _movieId,
            request.HttpContext.RequestAborted);

        return new OkObjectResult(
            new Models.Movies.MovieDetailsWithVideos
            {
                Id = details.Id,
                Title = details.Title,
                Overview = details.Overview,
                PosterPath = details.PosterPath,
                BackdropPath = details.BackdropPath,
                VoteAverage = details.VoteAverage,
                ReleaseDate = details.ReleaseDate,
                Runtime = details.Runtime,
                Tagline = details.Tagline,
                Genres = details.Genres,
                Videos = videos
            });
    }
}

using Microsoft.AspNetCore.Mvc;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.Shared;
using MovieSearchCase.WebApi.Mappers;

namespace MovieSearchCase.WebApi.Handlers.Movies;

public class SearchMovieHandler : IRequestHandlerAsync
{
    private readonly IMovieService _movieService;
    private readonly string _query;
    private readonly int _page;

    public SearchMovieHandler(IMovieService movieService, string query, int page)
    {
        _movieService = movieService;
        _query = query;
        _page = page;
    }

    public async Task<IActionResult> HandleAsync(HttpRequest request)
    {
        var error = RequestValidation.ValidateRequiredString(
           _query, "query");

        if (error is not null)
        {
            return error;
        }

        if (_page < 1 || _page > 500)
        {
            var result = new ObjectResult(new ProblemDetails
            {
                Type = "about:blank",
                Title = "Invalid request",
                Status = 400,
                Detail = "The 'page' parameter must be between 1 and 500."
            })
            {
                StatusCode = 400
            };

            result.ContentTypes.Add("application/problem+json");
            return result;
        }


        var SearchResults = await _movieService.GetSearchAsync(_query, _page, request.HttpContext.RequestAborted);
        return new OkObjectResult(new
        {
            page = SearchResults.Page,
            totalPages = SearchResults.TotalPages,
            totalResults = SearchResults.TotalResults,
            results = SearchResults.Results.Select(movie => movie.ToApiModel())
        });
    }
}

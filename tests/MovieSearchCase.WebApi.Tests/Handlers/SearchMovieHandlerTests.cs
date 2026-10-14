using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.Infrastructure.Clients;
using MovieSearchCase.WebApi.Handlers.Movies;
using System.Net;
using System.Text.Json;
using Xunit;

namespace MovieSearchCase.WebApi.Tests.Handlers;

public class SearchMovieHandlerTests
{
    [Fact]
    public async Task EmptyQueryShouldReturnBadRequest()
    {
        // Arrange: create a fake service and an empty query.
        var service = new Mock<IMovieService>();
        var handler = new SearchMovieHandler(
            service.Object, "", 1);

        // Act: execute the handler.
        var result = await handler.HandleAsync(
            new DefaultHttpContext().Request);

        // Assert: check the response and service calls.
        var response = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, response.StatusCode);

        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NoMatchesShouldReturnOkWithEmptyResults()
    {
        var service = new Mock<IMovieService>();

        service
            .Setup(s => s.GetSearchAsync(
                "unknownmovie", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<Movie>
            {
                Page = 1,
                TotalPages = 0,
                TotalResults = 0,
                Results = new List<Movie>()
            });

        var handler = new SearchMovieHandler(
            service.Object, "unknownmovie", 1);

        var result = await handler.HandleAsync(
            new DefaultHttpContext().Request);

        var response = Assert.IsType<OkObjectResult>(result);
        var body = JsonSerializer.SerializeToElement(response.Value);

        Assert.Equal(1, body.GetProperty("page").GetInt32());
        Assert.Equal(0, body.GetProperty("totalPages").GetInt32());
        Assert.Equal(0, body.GetProperty("totalResults").GetInt32());
        Assert.Equal(0, body.GetProperty("results").GetArrayLength());
    }

    [Fact]
    public async Task WhitespaceQueryShouldReturnBadRequest()
    {
        var service = new Mock<IMovieService>();
        var handler = new SearchMovieHandler(
            service.Object, "   ", 1);

        var result = await handler.HandleAsync(
            new DefaultHttpContext().Request);

        var response = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, response.StatusCode);

        service.VerifyNoOtherCalls();
    }


    [Fact]
    public async Task SearchShouldUseCorrectUrl()
    {
        var fakeHandler = new FakeHttpHandler();

        using var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/")
        };

        var client = new TmdbClient(httpClient);

        await client.GetSearchMoviesAsync(
            "dark knight", 2, CancellationToken.None);

        Assert.Equal(
            "https://api.themoviedb.org/3/search/movie?query=dark%20knight&page=2",
            fakeHandler.RequestUrl);
    }

    public class FakeHttpHandler : HttpMessageHandler
    {
        public string? RequestUrl { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUrl = request.RequestUri!.AbsoluteUri;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                      "page": 2,
                      "total_pages": 3,
                      "total_results": 41,
                      "results": [
                        {
                          "id": 268,
                          "title": "Batman",
                          "vote_average": 7.2,
                          "release_date": "1989-06-21"
                        }
                      ]
                    }
                    """,
                    System.Text.Encoding.UTF8,
                    "application/json")
            });
        }


        [Fact]
        public async Task SearchShouldSendCorrectUrlAndReturnPagination()
        {
            // Arrange
            var fakeHandler = new FakeHttpHandler();

            using var httpClient = new HttpClient(fakeHandler)
            {
                BaseAddress = new Uri("https://api.themoviedb.org/3/")
            };

            var client = new TmdbClient(httpClient);

            // Act
            var result = await client.GetSearchMoviesAsync(
                "dark knight", 2, CancellationToken.None);

            // Assert
            Assert.Equal(
                "https://api.themoviedb.org/3/search/movie?query=dark%20knight&page=2",
                fakeHandler.RequestUrl);

            Assert.Equal(2, result.Page);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(41, result.TotalResults);

            var movie = Assert.Single(result.Results);
            Assert.Equal(268, movie.Id);
            Assert.Equal("Batman", movie.Title);
        }
    }
}


using Microsoft.Extensions.Caching.Memory;
using Moq;
using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Interfaces.Clients;
using MovieSearchCase.Infrastructure.Services;
using Xunit;

namespace MovieSearchCase.Infrastructure.Tests.Services;

public class MovieServiceTests
{
    [Fact]
    public async Task GetTrendingShouldReturnMoviesFromClient()
    {
        var expected = new List<Movie>
        {
            new Movie { Id = 268, Title = "Batman" }
        };

        var client = new Mock<ITmdbClient>();

        client
            .Setup(c => c.GetTrendingMoviesAsync(
                CancellationToken.None))
            .ReturnsAsync(expected);

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MovieService(client.Object, cache);

        var result = await service.GetTrendingAsync(
            CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task SearchShouldReturnResultsFromClient()
    {
        var expected = new PagedResult<Movie>
        {
            Page = 2,
            TotalPages = 3,
            TotalResults = 41,
            Results = new List<Movie>
            {
                new Movie { Id = 268, Title = "Batman" }
            }
        };

        var client = new Mock<ITmdbClient>();

        client
            .Setup(c => c.GetSearchMoviesAsync(
                "batman", 2, CancellationToken.None))
            .ReturnsAsync(expected);

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MovieService(client.Object, cache);

        var result = await service.GetSearchAsync(
            "batman", 2, CancellationToken.None);

        Assert.Same(expected, result);

        client.Verify(c => c.GetSearchMoviesAsync(
            "batman", 2, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task RepeatedSearchShouldUseCache()
    {
        var expected = new PagedResult<Movie>
        {
            Page = 1,
            TotalPages = 1,
            TotalResults = 1,
            Results = new List<Movie>
            {
                new Movie { Id = 268, Title = "Batman" }
            }
        };

        var client = new Mock<ITmdbClient>();

        client
            .Setup(c => c.GetSearchMoviesAsync(
                "batman", 1, CancellationToken.None))
            .ReturnsAsync(expected);

        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MovieService(client.Object, cache);

        // First search fetches from the client.
        var first = await service.GetSearchAsync(
            "batman", 1, CancellationToken.None);

        // Second search should reuse the cached result.
        var second = await service.GetSearchAsync(
            "batman", 1, CancellationToken.None);

        Assert.Same(expected, first);
        Assert.Same(first, second);

        // Two searches, but only one client call.
        client.Verify(c => c.GetSearchMoviesAsync(
            "batman", 1, CancellationToken.None), Times.Once);
    }
}

using MovieSearchCase.Domain.Entities;

namespace MovieSearchCase.WebApi.Models.Movies;

public record MovieDetailsWithVideos : MovieDetails
{
    public IReadOnlyList<MovieVideo>
        Videos
    { get; init; } = [];
}

using MovieSearchCase.Domain.Entities;
using System.Globalization;

namespace MovieSearchCase.Infrastructure.Clients;

internal static class TmdbMovieMapper
{
    public static Movie ToDomainModel(this TmdbMovie tmdbMovie) => new()
    {
        Id = tmdbMovie.Id,
        Title = tmdbMovie.Title,
        Overview = tmdbMovie.Overview,
        PosterPath = tmdbMovie.PosterPath,
        BackdropPath = tmdbMovie.BackdropPath,
        VoteAverage = tmdbMovie.VoteAverage,
        ReleaseDate = DateOnly.TryParseExact(
            tmdbMovie.ReleaseDate,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var releaseDate)
            ? releaseDate
            : null,
    };

    //for details
    public static Movie ToDetailsDomainModel(
    this TmdbMovieDetails tmdbMovie) => new()
    {
        Id = tmdbMovie.Id,
        Title = tmdbMovie.Title,
        Overview = tmdbMovie.Overview,
        PosterPath = tmdbMovie.PosterPath,
        BackdropPath = tmdbMovie.BackdropPath,
        VoteAverage = tmdbMovie.VoteAverage,
        ReleaseDate = DateOnly.TryParseExact(
            tmdbMovie.ReleaseDate,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var releaseDate)
        ? releaseDate
        : null,

        Runtime = tmdbMovie.Runtime,
        Tagline = tmdbMovie.Tagline,
        Genres = tmdbMovie.Genres
        .Select(genre => genre.Name)
        .ToList()
    };
    //for video
    public static MovieVideo ToVideoDomainModel(this TmdbVideo video) => new()
    {
        Key = video.Key,
        Name = video.Name,
        Site = video.Site,
        Type = video.Type
    };
}

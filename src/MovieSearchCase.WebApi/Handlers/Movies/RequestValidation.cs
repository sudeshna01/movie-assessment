using Microsoft.AspNetCore.Mvc;

namespace MovieSearchCase.WebApi.Handlers;

internal static class RequestValidation
{
    public static ObjectResult? ValidateRequiredString(
        string? value,
        string parameterName)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var result = new ObjectResult(new ProblemDetails
        {
            Type = "about:blank",
            Title = "Invalid request",
            Status = 400,
            Detail = $"The '{parameterName}' parameter is required."
        })
        {
            StatusCode = 400
        };

        result.ContentTypes.Add("application/problem+json");

        return result;
    }
}

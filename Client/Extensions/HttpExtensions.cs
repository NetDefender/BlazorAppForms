using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorAppForms.Client.Extensions;

public static class HttpExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };

    public static Task<TValue?> GetFromJsonWithOptionsAsync<TValue>(this HttpClient client, [StringSyntax("Uri")] string? requestUri, CancellationToken cancellationToken = default)
    {
        return client.GetFromJsonAsync<TValue>(requestUri, _jsonOptions, cancellationToken);
    }
}
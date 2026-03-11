using System.Text.Json;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Domain.Services;

public interface IProgressDataSource
{
    Task<ProgressProjection?> ComputeProjectionAsync(CancellationToken ct = default);
}

public static class RouteProjectionService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static IReadOnlyList<double[]> ParsePolyline(string polylineJson)
    {
        if (string.IsNullOrWhiteSpace(polylineJson))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<double[]>>(polylineJson, JsonOptions)
                ?? (IReadOnlyList<double[]>)[];
        }
        catch
        {
            return [];
        }
    }
}

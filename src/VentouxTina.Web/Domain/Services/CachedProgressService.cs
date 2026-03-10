using Microsoft.Extensions.Caching.Memory;
using VentouxTina.Web.Domain.Models;

namespace VentouxTina.Web.Domain.Services;

public interface IProgressService
{
    Task<ProgressProjection?> GetProjectionAsync(CancellationToken ct = default);
}

public class CachedProgressService : IProgressService
{
    private const string CacheKey = "progress_projection";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    private readonly IMemoryCache _cache;
    private readonly IProgressDataSource _dataSource;

    public CachedProgressService(IMemoryCache cache, IProgressDataSource dataSource)
    {
        _cache = cache;
        _dataSource = dataSource;
    }

    public async Task<ProgressProjection?> GetProjectionAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKey, out ProgressProjection? cached))
            return cached;

        var projection = await _dataSource.ComputeProjectionAsync(ct);

        if (projection is not null)
            _cache.Set(CacheKey, projection, CacheDuration);

        return projection;
    }
}

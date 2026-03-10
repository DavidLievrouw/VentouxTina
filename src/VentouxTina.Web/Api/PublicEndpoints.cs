using Microsoft.AspNetCore.Mvc;
using VentouxTina.Web.Api.Contracts;
using VentouxTina.Web.Domain.Services;
using VentouxTina.Web.Infrastructure.DataSources;

namespace VentouxTina.Web.Api;

public static class PublicEndpoints
{
    public static IEndpointRouteBuilder MapPublicEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").RequireRateLimiting("fixed");

        // GET /api/progress
        group.MapGet(
            "/progress",
            async (IProgressService progress, CancellationToken ct) =>
            {
                var projection = await progress.GetProjectionAsync(ct);
                if (projection is null)
                    return Results.Problem(
                        detail: "Geen routedata beschikbaar.",
                        statusCode: 503
                    );

                return Results.Ok(
                    new
                    {
                        asOfDate = projection.AsOfDate,
                        totalDistanceKm = projection.TotalDistanceKm,
                        traveledDistanceKm = projection.TraveledDistanceKm,
                        remainingDistanceKm = projection.RemainingDistanceKm,
                        progressPercent = projection.ProgressPercent,
                        status = projection.Status,
                    }
                );
            }
        );

        // GET /api/logs
        group.MapGet(
            "/logs",
            async (
                ITripLogDataSource logs,
                [FromQuery] int? limit,
                CancellationToken ct
            ) =>
            {
                if (limit.HasValue && (limit.Value < 1 || limit.Value > 1000))
                    return Results.BadRequest(
                        new ErrorResponse("INVALID_LIMIT", "Limit moet tussen 1 en 1000 zijn.")
                    );

                var entries = await logs.GetEntriesAsync(limit, ct);
                return Results.Ok(
                    entries.Select(e => new
                    {
                        entryId = e.EntryId,
                        timestamp = e.Timestamp,
                        kilometers = e.Kilometers,
                        activity = e.Activity,
                        isCorrection = e.IsCorrection,
                    })
                );
            }
        );

        // GET /api/context
        group.MapGet(
            "/context",
            async (IProjectContextDataSource ctx, CancellationToken ct) =>
            {
                var (context, goal) = await ctx.GetContextAsync(ct);
                if (context is null || goal is null)
                    return Results.Problem(
                        detail: "Contextdata niet beschikbaar.",
                        statusCode: 503
                    );

                return Results.Ok(
                    new
                    {
                        locale = context.Locale,
                        organizationName = goal.OrganizationName,
                        goalAmountEur = goal.GoalAmountEur,
                        fundraiserText = context.FundraisingGoalText,
                        audience = goal.Audience,
                    }
                );
            }
        );

        return app;
    }
}

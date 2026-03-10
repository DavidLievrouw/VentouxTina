using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using VentouxTina.Web.Domain.Services;

namespace VentouxTina.Web.Api;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        // POST /account/login — receives form fields, sets cookie, redirects
        app.MapPost(
                "/account/login",
                async (HttpContext ctx, IAuthService authService) =>
                {
                    var form = ctx.Request.Form;
                    var username = form["username"].ToString().Trim();
                    var password = form["password"].ToString();
                    var returnUrl = form["returnUrl"].ToString();

                    var success = await authService.LoginAsync(ctx, username, password);

                    if (!success)
                    {
                        var encodedReturn = Uri.EscapeDataString(returnUrl);
                        return Results.Redirect($"/login?error=1&returnUrl={encodedReturn}");
                    }

                    var target = string.IsNullOrWhiteSpace(returnUrl)
                        ? "/beheer/logboek"
                        : returnUrl;
                    return Results.Redirect(target);
                }
            )
            .DisableAntiforgery();

        // GET /account/logout — clears cookie, redirects home
        app.MapGet(
            "/account/logout",
            async (HttpContext ctx) =>
            {
                await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/");
            }
        );

        return app;
    }
}

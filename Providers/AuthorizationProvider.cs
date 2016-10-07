using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Firefly.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Firefly.Providers{
    public sealed class AuthorizationProvider:OpenIdConnectServerProvider{

        public override Task ValidateTokenRequest(ValidateTokenRequestContext context) {
            // Reject the token requests that don't use grant_type=password or grant_type=refresh_token.
            if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType()) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only the resource owner password credentials and refresh token " +
                                "grants are accepted by this authorization server");

                return Task.FromResult(0);
            }

            // Since there's only one application and since it's a public client
            // (i.e a client that cannot keep its credentials private), call Skip()
            // to inform the server the request should be accepted without 
            // enforcing client authentication.
            context.Skip();

            return Task.FromResult(0);
        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context) {
    // Resolve ASP.NET Core Identity's user manager from the DI container.
    var manager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
    if (context.Request.IsPasswordGrantType()) {
        var user = await manager.FindByNameAsync(context.Request.Username);
        if (user == null) {
            context.Reject(
                error: OpenIdConnectConstants.Errors.InvalidGrant,
                description: "Invalid credentials.");

            return;
        }

        // Ensure the user is not already locked out.
        if (manager.SupportsUserLockout && await manager.IsLockedOutAsync(user)) {
            context.Reject(
                error: OpenIdConnectConstants.Errors.InvalidGrant,
                description: "Invalid credentials.");

            return;
        }

        // Ensure the password is valid.
        if (!await manager.CheckPasswordAsync(user, context.Request.Password)) {
            if (manager.SupportsUserLockout) {
                await manager.AccessFailedAsync(user);
            }

            context.Reject(
                error: OpenIdConnectConstants.Errors.InvalidGrant,
                description: "Invalid credentials.");

            return;
        }

        if (manager.SupportsUserLockout) { 
            await manager.ResetAccessFailedCountAsync(user);
        }

        // Reject the token request if two-factor authentication has been enabled by the user.
        if (manager.SupportsUserTwoFactor && await manager.GetTwoFactorEnabledAsync(user)) {
            context.Reject(
                error: OpenIdConnectConstants.Errors.InvalidGrant,
                description: "Two-factor authentication is required for this account.");

            return;
        }

        var identity = new ClaimsIdentity(context.Options.AuthenticationScheme);

        // Note: the name identifier is always included in both identity and
        // access tokens, even if an explicit destination is not specified.
        identity.AddClaim(ClaimTypes.NameIdentifier, user.UserName);

        // When adding custom claims, you MUST specify one or more destinations.
        // Read "part 7" for more information about custom claims and scopes.
        identity.AddClaim("username", await manager.GetUserNameAsync(user),
            OpenIdConnectConstants.Destinations.AccessToken,
            OpenIdConnectConstants.Destinations.IdentityToken);

        // Create a new authentication ticket holding the user identity.
        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(identity),
            new AuthenticationProperties(),
            context.Options.AuthenticationScheme);

        // Set the list of scopes granted to the client application.
        ticket.SetScopes(
            /* openid: */ OpenIdConnectConstants.Scopes.OpenId,
            /* email: */ OpenIdConnectConstants.Scopes.Email,
            /* profile: */ OpenIdConnectConstants.Scopes.Profile);

        // Set the resource servers the access token should be issued for.
        ticket.SetResources("resource_server");

        context.Validate(ticket);
    }
}

    }

}
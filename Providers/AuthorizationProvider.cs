using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Server;
using Firefly.Extensions;
using Firefly.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Firefly.Providers{
    public sealed class AuthorizationProvider:OpenIdConnectServerProvider{

        private readonly ILogger<AuthorizationProvider> logger;
        public AuthorizationProvider(ILogger<AuthorizationProvider> logger){
            this.logger = logger;
        }
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

        public override async Task HandleTokenRequest(HandleTokenRequestContext context) 
        {
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

                var roles = await manager.GetRolesAsync(user);
                logger.LogDebug("Found roles for user " + user.UserName + ":");
                logger.LogDebugJson(roles);

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

            

                // Redefining Name and Role types manually - see fucking why https://leastprivilege.com/2016/08/21/why-does-my-authorize-attribute-not-work/
                var identity = new ClaimsIdentity(context.Options.AuthenticationScheme, "Name", "Role");
       
                identity.AddClaim(ClaimTypes.NameIdentifier, user.Id.ToString());
                
                identity.AddClaim("Name", user.UserName,
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);    

                // Add roles to the claims
                foreach (var role in roles){
                    identity.AddClaim("Role", role,
                    OpenIdConnectConstants.Destinations.AccessToken,
                    OpenIdConnectConstants.Destinations.IdentityToken);
                } 
                
                logger.LogDebugJson(identity);
                
                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties(),
                    context.Options.AuthenticationScheme);

                // Todo: Define scopes when useful ...

               /* ticket.SetScopes(
                     OpenIdConnectConstants.Scopes.OpenId,
                     OpenIdConnectConstants.Scopes.Email,
                    OpenIdConnectConstants.Scopes.Profile);
                */
                
                // Set the resource servers the access token should be issued for.
                ticket.SetResources("resource_server");

                context.Validate(ticket);
    }
}

    }

}
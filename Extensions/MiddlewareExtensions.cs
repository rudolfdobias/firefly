using Firefly.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Firefly.Extensions {
    public static class MiddlewareExtensions {

        public static IApplicationBuilder UseCurrentUserMiddleware(this IApplicationBuilder builder){
            
            return builder.UseMiddleware<CurrentUserMiddleware>();
        }
    }
}
using System;
using System.Threading.Tasks;
using Firefly.Models;
using Firefly.Providers;
using Microsoft.AspNetCore.Http;

namespace Firefly.Middleware{
    public class CurrentUserMiddleware{
        private readonly RequestDelegate _next;
        private readonly ICurrentUserProvider _provider;

        public CurrentUserMiddleware(RequestDelegate next, ICurrentUserProvider provider){
            _next = next;
            _provider = provider;
        }

        public async Task Invoke(HttpContext context)  
        {
            
                var user = await _provider.HandleFromHttpContextAsync(context);
                if (user is ApplicationUser){
                    await _next.Invoke(context);
                } else {
                    context.Response.StatusCode = 401;
                    return;// Task.FromResult(0);
                }
            
        }
    }
}
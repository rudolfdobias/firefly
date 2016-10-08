using System.Threading.Tasks;
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
            _provider.HandleFromHttpContextAsync(context);
            await _next.Invoke(context);
        }
    }
}
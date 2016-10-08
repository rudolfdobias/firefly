using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Firefly.Models;
using Firefly.Providers;
using Firefly.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Firefly.Middleware{
    public class DebugHeadersMiddleware{
        private readonly RequestDelegate _next;
        private readonly ICurrentUserProvider _provider;

        public DebugHeadersMiddleware(RequestDelegate next, ICurrentUserProvider provider){
            _next = next;
            _provider = provider;
        }

        public async Task Invoke(HttpContext context)  
        {
            var watch = new Stopwatch();
            watch.Start();
      
            context.Response.OnStarting(state => {
                var httpContext = (HttpContext) state;

                if (_provider.GetUser() is ApplicationUser){
                    var user = _provider.GetUser();
                    context.Response.Headers.Add("X-Debug-User-Id", user.Id.ToString());
                    context.Response.Headers.Add("X-Debug-User-UserName", user.UserName);
                } else {
                    context.Response.Headers.Add("X-Debug-UserId", "NULL");
                }

                watch.Stop();
                context.Response.Headers.Add("X-Debug-Processing-Time", new[] { FormatElapsedTime(watch.ElapsedMilliseconds) });
                return Task.FromResult(0);
            }, context);

            await _next.Invoke(context);
            watch.Stop();
        }

        private string FormatElapsedTime(long millis){
            decimal number;
            string suffix;
            if (millis >= 1000){
                number = Math.Round((decimal)millis / 1000, 1);
                suffix = "s";
            }
            else {
                number = (decimal)millis;
                suffix = "ms";
            }

            return number.ToString() + suffix;
        }
    }
}
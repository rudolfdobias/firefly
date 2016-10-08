using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Firefly.Extensions{
    public static class JsonLogger{
        public static void LogDebugJson(this ILogger logger, object logObject){
            var output = JsonConvert.SerializeObject(logObject, new JsonSerializerSettings { 
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            logger.LogDebug(output);
        }
    }
}
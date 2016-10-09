using Microsoft.AspNetCore.Mvc;


namespace Firefly.Controllers{
    
    public class StatusController : Controller{
        
        [Route("/")]
        public IActionResult Status(){
            return StatusCode(200, new {
                Status = "OK"
            });
        }

        [Route("/loaderio-32b325992f3d08dbb7d4d02c87fb6465")]
        public IActionResult Benchmark(){
            return StatusCode(200, "loaderio-32b325992f3d08dbb7d4d02c87fb6465");
        }

    }
}
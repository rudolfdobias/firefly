using Microsoft.AspNetCore.Mvc;


namespace Firefly.Controllers{
    
    public class StatusController : Controller{
        
        [Route("/")]
        public IActionResult Status(){
            return StatusCode(200, new {
                Status = "OK"
            });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firefly.Controllers{

    [Authorize]
    abstract public class ProtectedController : Controller{

    }
}
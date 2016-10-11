using System;
using Firefly.Models;

namespace Firefly.Controllers
{
    public class AuthorsController : GenericResourceController<Author>
    {
        public AuthorsController(IServiceProvider services)
            :base(services)
        {}
    }
}

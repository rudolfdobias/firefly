using Firefly.Models;

namespace Firefly.Controllers
{
    public class AuthorsController : GenericResourceController<Author>
    {
        public AuthorsController(ApplicationDbContext context)
            :base(context)
        {}
    }
}

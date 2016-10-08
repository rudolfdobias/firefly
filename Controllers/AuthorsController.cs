using Firefly.Models;

namespace Firefly.Controllers
{
    public class AuthorsController : GenericResourceController<Author>
    {
        private readonly ApplicationDbContext _context;
        public AuthorsController(ApplicationDbContext context)
            :base(context)
        {}
    }
}

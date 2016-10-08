using Firefly.Models;

namespace Firefly.Controllers
{
    public class ArticlesController : GenericResourceController<Article>
    {
        public ArticlesController(ApplicationDbContext context) 
            :base(context, new Article())
        {}
    }
}

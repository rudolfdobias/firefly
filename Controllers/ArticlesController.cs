using System;
using Firefly.Models;


namespace Firefly.Controllers
{
    public class ArticlesController : GenericResourceController<Article>
    {
        public ArticlesController(IServiceProvider services) 
            :base(services)
        {
        }
    }
}

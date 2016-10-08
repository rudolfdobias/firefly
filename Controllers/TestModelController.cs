using Firefly.Models;

namespace Firefly.Controllers {
    public class TestModelController : GenericResourceController<TestModel>{
        public TestModelController(ApplicationDbContext context) 
            : base(context, new TestModel())
        {}
    }
}
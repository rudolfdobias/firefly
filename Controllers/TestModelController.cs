using Firefly.Models;
using System;

namespace Firefly.Controllers {
    public class TestModelController : GenericResourceController<TestModel>{
        public TestModelController(IServiceProvider services) 
            : base(services)
        {}
    }
}
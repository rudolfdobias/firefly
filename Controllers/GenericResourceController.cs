using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Firefly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Controllers{

    [Route("api/[controller]")]
    public class GenericResourceController<TModel> : ProtectedController where TModel:BaseEntity  
    {
        
        private readonly ApplicationDbContext _context;
        protected readonly TModel model;
        protected readonly DbSet<TModel> modelDbSet;
        public GenericResourceController(ApplicationDbContext context, TModel Model)
        {
            _context = context;
            this.modelDbSet = _context.Set<TModel>();
            this.model = Model;
        }

        [HttpGet]
        public async Task<IEnumerable<TModel>> Get()
        {
            return await modelDbSet.ToListAsync();
        }

        [HttpGet("{id}")]
        public TModel Get(Guid id)
        {
            return modelDbSet.FirstOrDefault(x=>x.Id == id);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]TModel value)
        {
            modelDbSet.Add(value);
            _context.SaveChanges();
            return StatusCode(201, value);
        }
    }
    
}
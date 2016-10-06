using System;
using System.Collections.Generic;
using System.Linq;
using Firefly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Controllers
{
    [Route("api/[controller]")]
    public class AuthorsController : Controller
    {
        private readonly ArticleContext _context;
        public AuthorsController(ArticleContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IEnumerable<Author> Get()
        {
            return _context.Authors.Include(author => author.Articles).ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Author Get(Guid id)
        {
            return _context.Authors.FirstOrDefault(x => x.Id == id);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Author value)
        {
            _context.Authors.Add(value);
            _context.SaveChanges();
            return StatusCode(201, value);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Firefly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firefly.Controllers
{
    [Route("api/[controller]")]
    public class ArticlesController : Controller
    {
        private readonly ArticleContext _context;
        public ArticlesController(ArticleContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IEnumerable<Article> Get()
        {
            return _context.Articles.Include(article => article.Author).ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public Article Get(Guid id)
        {
            return _context.Articles.FirstOrDefault(x=>x.Id == id);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Article value)
        {
            _context.Articles.Add(value);
            _context.SaveChanges();
            return StatusCode(201, value);
        }
    }
}

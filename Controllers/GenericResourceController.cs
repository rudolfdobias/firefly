using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Firefly.Models;
using Firefly.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Firefly.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Firefly.Controllers{

    [Authorize]
    [Route("api/[controller]")]
    public class GenericResourceController<TEntity> : ProtectedController where TEntity:BaseEntity  
    {
        
        private readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _modelDbSet;
        protected readonly IServiceProvider _services;
        protected readonly IOptions<Config> _config;
        public GenericResourceController(IServiceProvider services)
        {
            _context = services.GetService<ApplicationDbContext>();
            _config = services.GetService<IOptions<Config>>();
            _modelDbSet = _context.Set<TEntity>();
        }

        [HttpGet]
        public async Task<ResultSet<TEntity>> Get([FromQuery] int limit, [FromQuery] int page)
        {
            if (limit <= 0){
                limit = _config.Value.Resources.DefaultLimit;
            }
            int skip = 0;
            if (page > 0){
                skip = limit * page;
            } 
            var result = new ResultSet<TEntity>();
            var query = _modelDbSet.Skip(skip).Take(limit);
            result.Data = await query.ToListAsync();

            int? total = null;
            if (_config.Value.Resources.ShowTotalCount){
                total = await _modelDbSet.CountAsync();
            }

            result.Meta = new MetaData {
                CurrentPage = page, 
                PerPage = limit, 
                Prev = createHateoasUrlPrev(limit, page), 
                Next = createHateoasUrlNext(limit, page, total), 
                Total = total
            };
            
            return result;
        }

        [HttpGet("{id}")]
        public TEntity Get(Guid id)
        {
            return _modelDbSet.FirstOrDefault(x=>x.Id == id);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]TEntity value)
        {
            _modelDbSet.Add(value);
            _context.SaveChanges();
            return StatusCode(201, value);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody]TEntity value){

            var entity = _modelDbSet.FirstOrDefault(x => x.Id == id);
            if (entity == null){
                return StatusCode(404);
            }
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (var propertyInfo in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string name = propertyInfo.Name;
                if (name == "Item" || name == "Id"){
                    continue;
                }
                entity[propertyInfo.Name] = propertyInfo.GetValue(value, null);
                values.Add(propertyInfo.Name, propertyInfo.GetValue(value, null));
            }
            _context.SaveChanges();
            return StatusCode(200, entity);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id){
            
            var entity = _modelDbSet.FirstOrDefault(x => x.Id == id);
            if (entity == null){
                return StatusCode(404);
            }
            _modelDbSet.Remove(entity);
            _context.SaveChanges();
            return StatusCode(204);
        }

        protected string GetBaseUrl(){
            return HttpContext.Request.Host + HttpContext.Request.Path.ToString();
        }

        protected string createHateoasUrlPrev(int limit, int page){
            if (page <= 0){
                return null;
            }   
            
            var qs = new QueryString(Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()));
            qs.Replace("limit", limit.ToString());
            qs.Replace("page", (page -1).ToString());
            var test2 = qs.ToString();
            return GetBaseUrl() + qs.ToString();
        }

        protected string createHateoasUrlNext(int limit, int page, int? total){
            if (total is int && total <= 0){
                return null;
            }
            if (total is int && total <= limit * (page+ 1)){
                return null;
            }
          
            var qs = new QueryString(Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()));
            qs.Replace("limit", limit.ToString());
            qs.Replace("page", (page +1).ToString());
            var test2 = qs.ToString();
            return GetBaseUrl() + qs.ToString();
        }
    }
    
}
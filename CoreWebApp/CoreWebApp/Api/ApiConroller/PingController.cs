using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebApp.MediatR;
using CoreWebApp.Models;
using CoreWebApp.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebApp.Api.ApiConroller
{
    [Route("api/[controller]")]
    public class PingController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _memoryCache;
        private readonly IPhotosService _photosService;
        public PingController(IMediator mediator, IMemoryCache memoryCache, IPhotosService photosService)
        {
            _memoryCache = memoryCache;
            _mediator = mediator;
            _photosService = photosService;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> TestMediatRSimpleNotification(EventModel model)
        {

            _mediator.Publish<EventModel>(model);

            return new string[] { "value1", "value2" };
        }
        [HttpGet("/testRequest")]
        public async Task<string> TestMediatRRequest(string message)
        {

            return await _mediator.Send(new RequestModel { Message = message });
           
        }
        [HttpGet("/InMemoryCacheDemo")]
        public async Task<Photo> InMemoryCacheDemo(int id)
        {
            var existsInCache =  _memoryCache.TryGetValue<Photo>(id.ToString(), out var photo);
            if(!existsInCache)
            {
                photo = await _photosService.GetPhotoItem(id);
                _memoryCache.Set<Photo>(id.ToString(), photo, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10)));

                photo.FromCacheOrService = "Service";
            }
            else
            {
                photo.FromCacheOrService = "Cache";
            }
            return photo;
        }
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

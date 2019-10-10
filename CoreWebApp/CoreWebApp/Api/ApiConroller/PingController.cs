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
            var source = "cache";

            var retrivedPhoto = await _memoryCache.GetOrCreateAsync<Photo>(id.ToString(), async (cache) =>
            {
                cache.SetSlidingExpiration(TimeSpan.FromSeconds(3));
                cache.SetAbsoluteExpiration(TimeSpan.FromSeconds(20));
                cache.SetSize(5000);
                cache.RegisterPostEvictionCallback(EvictionCallback, this);


                var photoFromRepo = await _photosService.GetPhotoItem(id);
                source = _memoryCache.Get<string>("source");

                return photoFromRepo;
            });

            retrivedPhoto.FromCacheOrService = source;

            return retrivedPhoto;

        }

        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            _memoryCache.Set("source", "Retrived from cache");
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

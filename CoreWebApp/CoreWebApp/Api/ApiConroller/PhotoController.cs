using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoreWebApp.Models;
using CoreWebApp.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebApp.Api.ApiConroller
{
    [Route("api/[controller]")]
    public class PhotoController : Controller
    {
        private readonly IPhotosService _photosService;

        public PhotoController(IPhotosService photosService)
        {
            _photosService = photosService;
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<Photo>> Get()
        {
            return await _photosService.GetPhotos();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Photo> Get(int id)
        {
            return await _photosService.GetPhotoItem(id);
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

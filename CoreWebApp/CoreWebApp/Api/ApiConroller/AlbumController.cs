using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebApp.Models;
using CoreWebApp.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebApp.Api.ApiConroller
{
    [Route("api/[controller]")]
    public class AlbumController : Controller
    {
        private readonly IAlbumService _albumService;
        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }
        // GET: api/<controller>
        [HttpGet]
        [EnableCors("corsPolicy")]
        public async Task<IEnumerable<Album>> Get()
        {
            return await _albumService.GetAlbums();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Album> Get(int id)
        {
            return await _albumService.GetAlbumItem(id);
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

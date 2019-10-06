using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebApp.Models;
using CoreWebApp.Repository;
using CoreWebApp.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebApp.Api.ApiConroller
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly IPostRepository _PostService;
        public PostController(IPostRepository PostService)
        {
            _PostService = PostService;
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<Post>> Get()
        {
            return await _PostService.GetPosts();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Post> Get(int id)
        {
            return await _PostService.GetPost(id);
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

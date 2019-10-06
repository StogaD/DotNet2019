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
    public class CommentController : Controller
    {
        private readonly ICommentService _CommentService;
        public CommentController(ICommentService CommentService)
        {
            _CommentService = CommentService;
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<Comment>> Get()
        {
            return await _CommentService.GetComments();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Comment> Get(int id)
        {
            return await _CommentService.GetCommentItem(id);
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

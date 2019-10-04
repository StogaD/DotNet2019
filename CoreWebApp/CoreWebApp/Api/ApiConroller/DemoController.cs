using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebApp.Api.Infrastructure;
using CoreWebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebApp.Api.ApiConroller
{
    [ApiController]
    [ApiConventionType(typeof(CustomApiConvention))]
    [Route("api/[controller]")]
    public class DemoController : Controller
    {
        private readonly Parameters _parameters;
        public DemoController(IOptions<Parameters> parameters)
        {
            _parameters = parameters.Value;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(CustomApiConvention), "Demo")]
        public string DemoGet(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        [ProducesResponseType(404)]
        [ProducesResponseType(201)]
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

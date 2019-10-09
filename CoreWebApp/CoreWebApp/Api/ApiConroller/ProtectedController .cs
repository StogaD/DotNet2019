﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebApp.Api.Infrastructure;
using CoreWebApp.Configuration;
using CoreWebApp.CookiePolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebApp.Api.ApiConroller
{
    [ApiController]
    [ApiConventionType(typeof(CustomApiConvention))]
    [Route("api/[controller]")]
    public class ProtectedController : Controller
    {
        private readonly Parameters _parameters;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authService;
        public ProtectedController(IOptions<Parameters> parameters, ILogger logger, IAuthorizationService authService)
        {
            _parameters = parameters.Value;
            _logger = logger;
            _authService = authService;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.Error("Protected");

            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [MinimumAgeAuthorize(20)]
        public string ProtectedGet(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public async Task Post([FromBody]string value)
        {
            var resouse = new Resource(value);
            var isAuthorized = await _authService.AuthorizeAsync(User, resouse, "Write");

            if (isAuthorized.Succeeded)
            {
                //perform some job
            }
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

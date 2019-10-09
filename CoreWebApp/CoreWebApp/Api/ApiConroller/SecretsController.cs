using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebApp.KeyVault;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApp.Api.ApiConroller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretsController : ControllerBase
    {
        private readonly ISecretAccess _secretAccess;
        public SecretsController(ISecretAccess secretAccess)
        {
            _secretAccess = secretAccess;
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<string> Get(string secretName = null)
        {
           return await _secretAccess.GetSecret(secretName);
        }
       

    }
}
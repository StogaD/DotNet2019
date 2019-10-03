using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CoreWebApp.Api.Infrastructure
{
    public static class CustomApiConvention
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status208AlreadyReported)]
        [ProducesResponseType(StatusCodes.Status508LoopDetected)]
        [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
        public static void Demo([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix)]int id)
        {
          
        }
        public static void Demo2([ApiConventionNameMatch(ApiConventionNameMatchBehavior.Suffix)]int id)
        {
            //todo : this doesn't work when applied to controller
        }

    }
}

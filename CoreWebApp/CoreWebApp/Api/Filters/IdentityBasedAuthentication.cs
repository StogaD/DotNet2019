using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreWebApp.Api.Filters
{
    public class CustomAuthFilter : Attribute, IAuthorizeData
    {
        public string Policy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Roles { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AuthenticationSchemes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
        }
    }
}

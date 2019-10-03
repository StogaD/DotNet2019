using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using CoreWebApp.Api.Filters;

namespace CoreWebApp.Api.Infrastructure
{
    public class AuthorizationHeaderOperationFilter : IOperationFilter
    {
        public AuthorizationHeaderOperationFilter(string onlyForParameterTestPurpose)
        {

        }
        public void Apply(Operation operation, OperationFilterContext context)
        {

            var allowAnonymous = context.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute),false).Any();
            var isAuthorized = context.ApiDescription.ActionDescriptor.FilterDescriptors.Any(x => x.Filter is IAuthorizationFilter);

            var isAuthorizedAttr =  context.MethodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false).Any();
            var isIdentityBasedAuthentication = context.ApiDescription.ActionDescriptor.FilterDescriptors.Any(x => x.Filter is CustomAuthFilter);

            if (!isAuthorized  && !isAuthorizedAttr)
            {
                return;
            }

           if(isIdentityBasedAuthentication )
            {
                operation.Parameters.Add(
                new BodyParameter
                {
                    Description = "X - Auth token",
                    @In = "header",
                    Name = "X-Auth",
                    Required = true,
                    Schema = new Schema { Type = "string" }
                });
            }
           else
            {
                operation.Parameters.Add(
                new BodyParameter
                {
                    Description = "Bearer token",
                    @In = "Header",
                    Name = "Authorization",
                    Required = false,
                    Schema = new Schema { Type = "string" }
                });

            }

            operation.Summary = "Usefull posibility of set the tokens when working with swagger UI. \n Check if there are some changes in OpenAPI 3.0";
            operation.ExternalDocs = new ExternalDocs { Description = "Tokens", Url = "https://swagger.io/docs/specification/authentication" };
      

        }
    }
}

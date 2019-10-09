using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using CoreWebApp.Api.Model;
using System.Security.Claims;

namespace CoreWebApp
{
    internal class AdditionalPolicyRequirement :  AuthorizationHandler<AdditionalPolicyRequirement>, IAuthorizationRequirement
    {
        private readonly string _domain;
        public AdditionalPolicyRequirement(string domain)
        {
            _domain = domain;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdditionalPolicyRequirement requirement)
        {
            if(false == context.User.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                return Task.FromResult(0);
            }

            var name = context.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;

            if (name.EndsWith($"@{_domain}"))
            {
                context.Succeed(requirement);
            }
            return Task.FromResult(0);

        }
    }
}
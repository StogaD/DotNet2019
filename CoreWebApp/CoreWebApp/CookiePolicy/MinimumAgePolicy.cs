using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;

namespace CoreWebApp
{
    internal class MinimumAgePolicy : AuthorizationHandler<MinimumAgePolicy>, IAuthorizationRequirement
    {
        private readonly int _age;
        public MinimumAgePolicy(int age)
        {
            _age = age;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgePolicy requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            {
                return Task.FromResult(0);
            }
            var dob = context.User.Claims.First(c => c.Type == ClaimTypes.DateOfBirth).Value;

            if( DateTime.TryParse(dob,out var date))
            {
                if(date.AddYears(_age) < DateTime.Now)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.FromResult(0);
        }
    }
}
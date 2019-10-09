using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CoreWebApp.CookiePolicy
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public CustomCookieAuthenticationEvents()
        {
        }

        public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            var lastChanged = (from c in userPrincipal.Claims
                               where c.Type == "LastChanged"
                               select c.Value).FirstOrDefault();


            //Only for test purpose -  it shoud check the last chanhe in DB
            if (DateTime.TryParse(lastChanged, out var _dateTime))
            {
                var timeSpan = TimeSpan.FromMinutes(2);

                if (DateTime.Now.CompareTo(_dateTime.Add(timeSpan))> 0)
                   {
                    context.RejectPrincipal();
                    context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }

            }
            return base.ValidatePrincipal(context);
        }
    }
}

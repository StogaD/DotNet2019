using CoreWebApp.Api.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreWebApp.Api.ApiConroller
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync( CookieAuthenticationDefaults.AuthenticationScheme);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApplicationUser user)
        {

            // omit Autentication for now
            if( user == null || string.IsNullOrWhiteSpace(user.FullName) || string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Incorrect credentials !");
            }


            _logger.LogInformation("User {Name} logged out at {Time}.",
                User.Identity.Name, DateTime.UtcNow);


            //for test purpose . should be retrived from Db
            var mockDbChanges = DateTime.Now;
          
            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.DateOfBirth, new DateTime(1984,02,20).ToString()),
                new Claim("FullName", user.FullName),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim("LastChanged", mockDbChanges.ToString())
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //todo
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(15)
                });

            return Ok();
        }
    }
}

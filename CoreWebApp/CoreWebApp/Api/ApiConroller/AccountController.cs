using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWebApp.Token;
using CoreWebApp.TokenOption;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebApp.Api.ApiConroller
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtTokenOptions _options;

        public AccountController(
            UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager,IOptions<JwtTokenOptions> optionsAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _options = optionsAccessor.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Resgister([FromBody] Credentials credentials)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = credentials.Email, Email = credentials.Email };
                var result = await _userManager.CreateAsync(user, credentials.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "access_token" , GetAccessToken(credentials.Email) },
                        { "id_token" , GetIdToken(user) }
                    });
                }
                return Errors(result);
            }
            return Errors("Unexpected error");
        }
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] Credentials credentials)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(credentials.Email);
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "access_token", GetAccessToken(credentials.Email) },
                        { "id_token", GetIdToken(user) }
                    });
                }
                return new JsonResult("Unable to sign in") { StatusCode = 401 };
            }
            return Errors("Unexpected error");
        }
        private IActionResult Errors(IdentityResult result)
        {
            var items = result.Errors
                .Select(x => x.Description)
                .ToArray();
            return new JsonResult(items) { StatusCode = 400 };
        }
        private IActionResult Errors(string message)
        {
            return new JsonResult(message) { StatusCode = 400 };
        }

        private object GetIdToken(IdentityUser user)
        {
            var payload = new Dictionary<string, object>
            {
                { "id" , user.Id },
                { "sub" , user.Email },
                { "email" , user.Email },
                { "emailConfirmed",user.EmailConfirmed }
            };

            return GetToken(payload);
        }
        private string GetToken(Dictionary<string, object> payload)
        {
            var secret = _options.SecretKey;

            payload.Add("iss", _options.Issuer);
            payload.Add("aud", _options.Audience);
            payload.Add("nbf", ConvertToUnixTimestamp(DateTime.Now));
            payload.Add("iat", ConvertToUnixTimestamp(DateTime.Now));
            payload.Add("exp", ConvertToUnixTimestamp(DateTime.Now.AddDays(7)));
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            return encoder.Encode(payload, secret);
        }
        private object GetAccessToken(string email)
        {
            var payload = new Dictionary<string, object>
              {
                { "sub", email },
                { "email", email }
              };
            return GetToken(payload);
        }
        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}

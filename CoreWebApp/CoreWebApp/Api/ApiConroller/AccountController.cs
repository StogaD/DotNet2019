using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CoreWebApp.Token;
using CoreWebApp.TokenOption;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using JWTAPI.Core.Security.Tokens;
using JWTAPI.Security.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebApp.Api.ApiConroller
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtTokenOptions _options;
        private readonly SigningConfigurations _signingConfigurations;
        private IdentityUser _user;

        public AccountController(SigningConfigurations signingConfigurations,
            UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager,IOptions<JwtTokenOptions> optionsAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _options = optionsAccessor.Value;
            _signingConfigurations = signingConfigurations;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Resgister([FromBody] Credentials credentials)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = credentials.Email, Email = credentials.Email };
                _user = user;
                var result = await _userManager.CreateAsync(user, credentials.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    var claims = await _userManager.GetClaimsAsync(user);
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "access_token" , CreateAccessToken(credentials.Email, claims) },
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
                    _user = user;
                    var claims = await _userManager.GetClaimsAsync(user);

                    return Ok(CreateAccessToken(credentials.Email, claims));

                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "access_token", CreateAccessToken(credentials.Email, claims) },
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

            return GetToken(payload, null);
        }
        private string GetToken(Dictionary<string, object> payload, IList<Claim> userClaims)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(5);

            var secret = _options.SecretKey;

            var securityToken = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: userClaims,
            expires: accessTokenExpiration,
            notBefore: DateTime.UtcNow,
            signingCredentials: SigningConfigurations()
            );
            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(securityToken);

            return accessToken;

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
        private object CreateAccessToken(string email, IList<Claim> claims)
        {
            var ph = new PasswordHasher<IdentityUser>();
            var passwrd = ph.HashPassword(_user, _options.SecretKey);

            var refreshToken = new RefreshToken(
                token: new PasswordHasher<IdentityUser>().HashPassword(_user, Guid.NewGuid().ToString()),
                expiration: DateTime.UtcNow.AddSeconds(_options.RefreshTokenExpiration).Ticks
            );

            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiration);

            var securityToken = new JwtSecurityToken
            (
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: GetClaims(),
                expires: accessTokenExpiration,
                notBefore: DateTime.UtcNow,
                signingCredentials: _signingConfigurations.SigningCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(securityToken);

            return new AccessToken(accessToken, accessTokenExpiration.Ticks, refreshToken);


        }

        private IEnumerable<Claim> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email)
            };

            return claims;
        }
        private static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
        private SigningCredentials SigningConfigurations()
        {
            SecurityKey Key;
            using (var provider = new RSACryptoServiceProvider(2048))
            {
                Key = new RsaSecurityKey(provider.ExportParameters(true));
            }

            return new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
        }


    }


}

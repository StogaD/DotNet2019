using System;

namespace JWTAPI.Core.Security.Tokens
{
    public abstract class JsonWebToken
    {
        public string Token { get; protected set; }
        public long Expiration { get; protected set; }

        public JsonWebToken(string token, long expiration)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Invalid token.");

            if (expiration <= 0)
                throw new ArgumentException("Invalid expiration.");

            Token = token;
            Expiration = expiration;
        }

        public bool IsExpired() => DateTime.UtcNow.Ticks > Expiration;
    }

    public class RefreshToken : JsonWebToken
    {
        public RefreshToken(string token, long expiration) : base(token, expiration)
        {
        }
    }
    public class AccessToken : JsonWebToken
    {
        public RefreshToken RefreshToken { get; private set; }

        public AccessToken(string token, long expiration, RefreshToken refreshToken) : base(token, expiration)
        {
            if (refreshToken == null)
                throw new ArgumentException("Specify a valid refresh token.");

            RefreshToken = refreshToken;
        }
    }
}
using System;
using Microsoft.IdentityModel.Tokens;

namespace MTracking.BLL.Models.Implementations
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string Audience { get; set; }

        public int AccessTokenLifetime { get; set; }

        public int RefreshTokenLifetime { get; set; }

        public string JtiGenerator => Guid.NewGuid().ToString();

        public SigningCredentials SigningCredentials { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MTracking.BLL.DTOs.Authentication.Responses;
using MTracking.BLL.DTOs.User.Responses;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Entities;

namespace MTracking.BLL.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IMapper _mapper;

        public JwtService(IOptions<JwtIssuerOptions> jwtOptions, IMapper mapper)
        {
            _jwtOptions = jwtOptions.Value;
            _mapper = mapper;
        }

        public IResult<(ClaimsPrincipal, JwtSecurityToken)> GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(
                        token,
                        new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = true,
                            IssuerSigningKey = _jwtOptions.SigningCredentials.Key,
                        },
                        out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(_jwtOptions.SigningCredentials.Algorithm, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException(ValidationFactory.InvalidToken);

                return Result<(ClaimsPrincipal, JwtSecurityToken)>.CreateSuccess((principal, jwtSecurityToken));
            }
            catch
            {
                return Result<(ClaimsPrincipal, JwtSecurityToken)>.CreateFailed(ValidationFactory.ExpiredTokenPrincipalIsNotGotten);
            }
        }

        public async Task<IResult<UserLoginResponseDto>> GenerateUserTokenAsync(User user)
        {
            var tokensResult = await GenerateSimpleTokensAsync(user);

            if (!tokensResult.Success)
                return Result<UserLoginResponseDto>.CreateFailed(tokensResult.ErrorInfo.Error).AddErrors(tokensResult.ErrorInfo.Messages);
            
            return Result<UserLoginResponseDto>.CreateSuccess(new UserLoginResponseDto()
            {
                Token = tokensResult.Data,
                User = _mapper.Map<UserResponseDto>(user)
            });
        }

        public async Task<IResult<TokenDto>> GenerateSimpleTokensAsync(User user)
        {
            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = GenerateRefreshToken(user);

            if (!accessToken.Success)
                return Result<TokenDto>.CreateFailed(ValidationFactory.TokenIsNotGenerated).AddErrors(accessToken.ErrorInfo.Messages);

            if (!refreshToken.Success)
                return Result<TokenDto>.CreateFailed(ValidationFactory.TokenIsNotGenerated).AddErrors(refreshToken.ErrorInfo.Messages);

            return Result<TokenDto>.CreateSuccess(new TokenDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken.Data),
                AccessTokenExpiredTime = accessToken.Data.ValidTo.ToLocalTime(),
                RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken.Data),
                RefreshTokenExpiredTime = refreshToken.Data.ValidTo.ToLocalTime(),
            });
        }

        private async Task<Result<JwtSecurityToken>> GenerateAccessTokenAsync(User user)
        {
            var claims = await GetClaimsIdentityAsync(user);

            if (!claims.Success)
                return Result<JwtSecurityToken>.CreateFailed(ValidationFactory.AccessTokenIsNotCreated).AddErrors(claims.ErrorInfo.Messages);

            try
            {
                var accessToken = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    notBefore: DateTime.UtcNow,
                    claims: claims.Data.Claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtOptions.AccessTokenLifetime)),
                    signingCredentials: _jwtOptions.SigningCredentials);

                return Result<JwtSecurityToken>.CreateSuccess(accessToken);
            }
            catch
            {
                return Result<JwtSecurityToken>.CreateFailed(ValidationFactory.AccessTokenIsNotCreated);
            }
        }

        private Result<JwtSecurityToken> GenerateRefreshToken(User user)
        {
            var claims = GetRefreshClaimsIdentity(user);

            if (!claims.Success)
                return Result<JwtSecurityToken>.CreateFailed(ValidationFactory.RefreshTokenIsNotCreated);

            try
            {
                var refreshToken = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    notBefore: DateTime.UtcNow,
                    claims: claims.Data.Claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtOptions.RefreshTokenLifetime)),
                    signingCredentials: _jwtOptions.SigningCredentials);

                return Result<JwtSecurityToken>.CreateSuccess(refreshToken);
            }
            catch
            {
                return Result<JwtSecurityToken>.CreateFailed(ValidationFactory.RefreshTokenIsNotCreated);
            }
        }

        private async Task<Result<ClaimsIdentity>> GetClaimsIdentityAsync(User user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                return Result<ClaimsIdentity>.CreateSuccess(new ClaimsIdentity(claims, "Token"));
            }
            catch
            {
                return Result<ClaimsIdentity>.CreateFailed(ValidationFactory.ClaimsIsNotCreated);
            }
        }

        private Result<ClaimsIdentity> GetRefreshClaimsIdentity(User user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, _jwtOptions.JtiGenerator),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Token");

                return Result<ClaimsIdentity>.CreateSuccess(claimsIdentity);
            }
            catch
            {
                return Result<ClaimsIdentity>.CreateFailed(ValidationFactory.ClaimsIsNotCreated);
            }
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Authentication.Responses;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.Core.Entities;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IJwtService
    {
        IResult<(ClaimsPrincipal principal, JwtSecurityToken jwt)> GetPrincipalFromExpiredToken(string token);

        Task<IResult<TokenDto>> GenerateSimpleTokensAsync(User user);

        Task<IResult<UserLoginResponseDto>> GenerateUserTokenAsync(User user);
    }
}

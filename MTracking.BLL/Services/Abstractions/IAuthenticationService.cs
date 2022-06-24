using System.Collections.Generic;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Authentication.Requests;
using MTracking.BLL.DTOs.Authentication.Responses;
using MTracking.BLL.DTOs.User.Requests;
using MTracking.BLL.Models.Abstractions;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.Core.Entities;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<IResult<List<User>>> SearchAsync(string name);
        Task<IResult<UserLoginResponseDto>> RegisterAsync(UserRegisterDto userDto);

        Task<IResult<UserLoginResponseDto>> LoginAsync(UserLoginDto userDto);

        Task<IResult<UserLoginResponseDto>> ChangePasswordAsync(ChangePasswordDto passwordDto);

        Task<IResult<TokenDto>> RefreshTokenAsync(string refreshToken);

        Task<IResult> SendVerificationCodeAsync(string email);

        Task<IResult> SetNewPasswordAsync(SetPasswordDto dto);

        Task<IResult> CheckVerificationCodeAsync(CheckCodeDto dto);
    }
}

using AutoMapper;
using MTracking.BLL.DTOs.User.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MTracking.Core.Extensions;
using MTracking.BLL.DTOs.Authentication.Requests;
using MTracking.BLL.DTOs.Authentication.Responses;
using MTracking.BLL.DTOs.User.Requests;
using MTracking.BLL.Models.Abstractions;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Entities;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.BLL.Services.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserInfoService _userInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IRepository<User> _userRepository;
        private readonly IEmailService _emailService;

        public AuthenticationService(IUnitOfWork unitOfWork, IMapper mapper, IJwtService jwtService, IEmailService emailService, IUserInfoService userInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtService = jwtService;
            _userRepository = unitOfWork.GetRepository<User>();
            _emailService = emailService;
            _userInfo = userInfo;
        }

        public async Task<IResult<List<User>>> SearchAsync(string name)
        {
            var users = await _userRepository.Table
                .Where(x => x.UserName.Contains(name) || x.HebrewName.Contains(name) || x.Email.Contains(name) || x.CommitId.ToString().Contains(name))
                .ToListAsync();

            return users.Any()
                ? Result<List<User>>.CreateSuccess(users)
                : Result<List<User>>.CreateFailed(ValidationFactory.UserIsNotFound);


        }

        public async Task<IResult<UserLoginResponseDto>> RegisterAsync(UserRegisterDto userDto)
        {
            var user = await _userRepository.Table.FirstOrDefaultAsync(x => x.UserName.Equals(userDto.UserName) || x.Email == userDto.Email);

            if (user != null)
                return Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.UserAlreadyExists);

            try
            {
                //const string defaultPassword = "12345678";

                CreatePasswordHash(userDto.Password, out var passwordHash, out var passwordSalt);
                var commitId = await _userRepository.Table.MinAsync(x => x.CommitId);

                var newUser = _mapper.Map<User>(userDto);
                newUser.Email = userDto.Email;
                newUser.CommitId = commitId > 0 ? -1 : commitId - 1;
                newUser.UserName = userDto.UserName;
                newUser.HebrewName = "iOS user";
                newUser.EnglishName = "iOS user";
                newUser.PasswordHash = passwordHash;
                newUser.PasswordSalt = passwordSalt;
                newUser.IsPasswordChanged = true;
                newUser.EmployeeRunCommit = true;
                newUser.EmployeeSoftwareInvention = true;

                var result = await _userRepository.InsertAsync(newUser);

                if (await _unitOfWork.SaveAsync() > 0)
                    return await _jwtService.GenerateUserTokenAsync(result);

                return Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.UserIsNotCreated);
            }
            catch
            {
                return Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.UserIsNotCreated);
            }
        }

        public async Task<IResult<UserLoginResponseDto>> LoginAsync(UserLoginDto userDto)
        {
            var userName = userDto.UserName.ReplaceToHebrewGershayim().ReplaceToHebrewGeresh();
            var user = await _userRepository.Table.FirstOrDefaultAsync(x => x.UserName.Equals(userName));

            if (user is null)
                return Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.UserIsNotFound);

            if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
                return Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.WrongPassword);

            if (!user.EmployeeRunCommit)
                return Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.TheAccessIsRestricted);

            return await _jwtService.GenerateUserTokenAsync(user);
        }

        public async Task<IResult<UserLoginResponseDto>> ChangePasswordAsync(ChangePasswordDto passwordDto)
        {
            var user = await _userRepository.GetByIdAsync(_userInfo.UserId);

            if (user is null)
                return Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.UserIsNotFound);

            if (!VerifyPasswordHash(passwordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
                return Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.WrongPassword);

            CreatePasswordHash(passwordDto.NewPassword, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.IsPasswordChanged = true;

            var updatedUser = await _userRepository.UpdateAsync(user);

            return await _unitOfWork.SaveAsync() > 0
                ? await _jwtService.GenerateUserTokenAsync(updatedUser)
                : Result<UserLoginResponseDto>.CreateFailed(ValidationFactory.WrongPassword);
        }

        public async Task<IResult<TokenDto>> RefreshTokenAsync(string refreshToken)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(refreshToken);

            if (!principal.Success)
                return Result<TokenDto>.CreateFailed(principal.ErrorInfo.Error);

            var user = await _userRepository.GetByIdAsync(int.Parse(principal.Data.jwt.Subject));

            if (user is null)
                return Result<TokenDto>.CreateFailed(ValidationFactory.AccessTokenIsNotCreated);

            var newToken = await _jwtService.GenerateSimpleTokensAsync(user);

            return newToken.Success
                ? newToken
                : Result<TokenDto>.CreateFailed(ValidationFactory.InvalidToken);
        }

        public async Task<IResult> SendVerificationCodeAsync(string email)
        {
            var user = await _userRepository.Table.FirstOrDefaultAsync(x => x.Email == email);

            if (user is null)
                return Result.CreateFailed(ValidationFactory.EmailDoesNotExist);

            var verificationCode = new Random().Next(99999, 999999).ToString();

            var message = new EmailMessage(new string[] { email }, "Verification code", $"Your code is: {verificationCode}");

            var sendEmailResult = await _emailService.SendEmailAsync(message);

            if (!sendEmailResult.Success)
                return Result.CreateFailed(sendEmailResult.ErrorInfo.Error).AddErrors(sendEmailResult.ErrorInfo.Messages);

            user.VerificationCode = verificationCode;
            await _userRepository.UpdateAsync(user);

            return await _unitOfWork.SaveAsync() > 0
                ? Result.CreateSuccess()
                : Result.CreateFailed(ValidationFactory.EmailIsNotSent);
        }

        public async Task<IResult> SetNewPasswordAsync(SetPasswordDto dto)
        {
            var user = await _userRepository.Table
                .FirstOrDefaultAsync(x => x.Email == dto.Email && x.VerificationCode == dto.VerificationCode);

            if (user is null)
                return Result.CreateFailed(ValidationFactory.AccessDenied);

            CreatePasswordHash(dto.Password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.VerificationCode = null;
            user.IsPasswordChanged = true;

            await _userRepository.UpdateAsync(user);

            return await _unitOfWork.SaveAsync() > 0
                ? Result.CreateSuccess()
                : Result.CreateFailed(ValidationFactory.PasswordIsNotChanged);
        }

        public async Task<IResult> CheckVerificationCodeAsync(CheckCodeDto dto)
        {
            var user = await _userRepository.Table
                .FirstOrDefaultAsync(x => x.Email == dto.Email && x.VerificationCode == dto.VerificationCode);

            return user is null
                ? Result.CreateFailed(ValidationFactory.WrongVerificationCode)
                : Result.CreateSuccess();
        }

        #region private

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                    return false;
            }
            return true;
        }

        #endregion
    }
}

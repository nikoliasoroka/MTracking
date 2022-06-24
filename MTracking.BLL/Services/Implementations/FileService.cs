using System;
using System.Collections.Generic;
using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MTracking.DAL.Migrations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MTracking.BLL.DTOs.File.Request;
using MTracking.BLL.DTOs.File.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Entities;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.BLL.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IUserInfoService _userInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<File> _fileRepository;
        private readonly IRepository<TimeLog> _timeLogRepository;
        private readonly IRepository<UserFilePin> _userFilePinRepository;

        public FileService(IUnitOfWork unitOfWork, IMapper mapper, IUserInfoService userInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userInfo = userInfo;
            _userRepository = unitOfWork.GetRepository<User>();
            _fileRepository = unitOfWork.GetRepository<File>();
            _timeLogRepository = unitOfWork.GetRepository<TimeLog>();
            _userFilePinRepository = unitOfWork.GetRepository<UserFilePin>();
        }

        public async Task<IResult<FileResponseDto>> GetByIdAsync(int fileId)
        {
            var file = await _fileRepository.GetByIdAsync(fileId);

            return file is null
                ? Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotFound)
                : Result<FileResponseDto>.CreateSuccess(_mapper.Map<FileResponseDto>(file));
        }

        public async Task<IResult<PagedResult<FileResponseDto>>> GetAllAsync(PaginationParams paginationParams)
        {
            var timeLogs = await _timeLogRepository.Table
                .Include(x => x.File)
                .Include(x => x.File.UserFilePins)
                .Where(x => x.UserId == _userInfo.UserId && x.FileId.HasValue)
                .OrderByDescending(x => x.File.UserFilePins.Any())
                .ThenByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .ToListAsync();

            var files = timeLogs
                .Select(x => _mapper.Map<FileResponseDto>(x.File));

            var lookup = files.ToLookup(x => x.Id, v => v).Select(x => x.First()).AsQueryable();

            var pagedResult = await new PagedResult<FileResponseDto>().CreateAsync(lookup, paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PagedResult<FileResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<PagedResult<FileResponseDto>>> GetFilesByUserAsync(PaginationParams paginationParams)
        {
            var user = await _userRepository.Table
                .Include(x => x.TimeLogs)
                .ThenInclude(x => x.File)
                .ThenInclude(x => x.UserFilePins)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<PagedResult<FileResponseDto>>.CreateFailed(ValidationFactory.AccessDenied);

            var files = user.TimeLogs
                .GroupBy(x => x.File)
                .Select(x => _mapper.Map<FileResponseDto>(x.Key))
                .AsQueryable();

            var pagedResult = await new PagedResult<FileResponseDto>().CreateAsync(files, paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PagedResult<FileResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<PagedResult<FileResponseDto>>> SearchFilesAsync(PaginationParams paginationParams,
            string query)
        {
            var files = _fileRepository.Table
                .Where(x => x.Name.StartsWith(query) || x.Name.Contains($" {query}")
                    && x.UserFilePins.Any(a => a.UserId == _userInfo.UserId))
                .Include(x => x.UserFilePins)
                .OrderByDescending(x => x.UserFilePins.Any())
                .ThenBy(x => x.Name)
                .Select(x => _mapper.Map<FileResponseDto>(x));

            var pagedResult = await new PagedResult<FileResponseDto>().CreateAsync(files, paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PagedResult<FileResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<FileResponseDto>> PinFileAsync(bool status, int fileId)
        {
            var userExists = await _userRepository.Table.AnyAsync(x => x.Id == _userInfo.UserId);

            if (!userExists)
                return Result<FileResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            if (status)
            {
                await _userFilePinRepository.InsertAsync(new UserFilePin()
                {
                    UserId = _userInfo.UserId,
                    FileId = fileId
                });

                return await _unitOfWork.SaveAsync() > 0
                    ? Result<FileResponseDto>.CreateSuccess()
                    : Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotUpdated);
            }
            else
            {
                var userPin = await _userFilePinRepository.Table
                    .FirstOrDefaultAsync(x => x.UserId == _userInfo.UserId && x.FileId == fileId);

                if (userPin is null)
                    return Result<FileResponseDto>.CreateFailed(ValidationFactory.ErrorWhilePinningFile);

                await _userFilePinRepository.DeleteAsync(userPin.Id);

                return await _unitOfWork.SaveAsync() > 0
                    ? Result<FileResponseDto>.CreateSuccess()
                    : Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotDeleted);
            }
        }

        public async Task<IResult<FileResponseDto>> CreateAsync(FileCreateRequestDto dto)
        {
            return Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotCreated);
        }

        //public async Task<IResult<FileResponseDto>> CreateAsync(FileCreateRequestDto fileDto)
        //{
        //    using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //    var user = await _userRepository.GetByIdAsync(_userInfo.UserId);

        //    if (user is null)
        //        return Result<FileResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

        //    var newFile = await _fileRepository.InsertAsync(_mapper.Map<File>(fileDto));

        //    if (await _unitOfWork.SaveAsync() <= 0)
        //        return Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotCreated);

        //    var result = await _userFileRepository.InsertAsync(new UserFile()
        //    {
        //        UserId = user.Id,
        //        FileId = newFile.Id
        //    });

        //    var isSuccess = await _unitOfWork.SaveAsync() > 0;

        //    transactionScope.Complete();

        //    return isSuccess
        //        ? Result<FileResponseDto>.CreateSuccess(_mapper.Map<FileResponseDto>(newFile))
        //        : Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotCreated);
        //}

        public async Task<IResult<FileResponseDto>> CreateCommonFileAsync(FileCreateRequestDto fileDto)
        {
            var user = await _userRepository.GetByIdAsync(_userInfo.UserId);

            if (user is null)
                return Result<FileResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var newFile = await _fileRepository.InsertAsync(_mapper.Map<File>(fileDto));

            return await _unitOfWork.SaveAsync() > 0
                ? Result<FileResponseDto>.CreateSuccess(_mapper.Map<FileResponseDto>(newFile))
                : Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotCreated);
        }

        public async Task<IResult<FileResponseDto>> UpdateAsync(FileUpdateRequestDto fileDto)
        {
            var user = await _userRepository.GetByIdAsync(_userInfo.UserId);

            if (user is null)
                return Result<FileResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var updatedFile = await _fileRepository.UpdateAsync(_mapper.Map<File>(fileDto));

            return await _unitOfWork.SaveAsync() > 0
                ? Result<FileResponseDto>.CreateSuccess(_mapper.Map<FileResponseDto>(updatedFile))
                : Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotUpdated);
        }

        public async Task<IResult<FileResponseDto>> DeleteAsync(int fileId)
        {
            var user = await _userRepository.GetByIdAsync(_userInfo.UserId);

            if (user is null)
                return Result<FileResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var file = await _fileRepository.Table
                .Where(x => x.Id == fileId)
                .FirstOrDefaultAsync();

            if (file is null)
                return Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotDeleted)
                    .AddError(ValidationFactory.FileIsNotFound);

            await _fileRepository.DeleteAsync(file.Id);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<FileResponseDto>.CreateSuccess(_mapper.Map<FileResponseDto>(file))
                : Result<FileResponseDto>.CreateFailed(ValidationFactory.FileIsNotDeleted);
        }
    }
}
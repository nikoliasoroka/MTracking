using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.TimeLog.Request;
using MTracking.BLL.DTOs.TimeLog.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Entities;
using MTracking.Core.Enums;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.BLL.Services.Implementations
{
    public class TimeLogService : ITimeLogService
    {
        private IConfiguration _csvConfiguration;
        private readonly IServiceProvider _provider;
        private readonly IUserInfoService _userInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<File> _fileRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<Description> _descriptionRepository;
        private readonly IRepository<TimeLog> _timeLogRepository;
        private readonly IRepository<ExportTimeLog> _exportTimeLogRepository;

        public TimeLogService(IUnitOfWork unitOfWork, IMapper mapper, IUserInfoService userInfo,
            IServiceProvider provider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userInfo = userInfo;
            _provider = provider;
            _userRepository = unitOfWork.GetRepository<User>();
            _fileRepository = unitOfWork.GetRepository<File>();
            _topicRepository = unitOfWork.GetRepository<Topic>();
            _descriptionRepository = unitOfWork.GetRepository<Description>();
            _timeLogRepository = unitOfWork.GetRepository<TimeLog>();
            _exportTimeLogRepository = unitOfWork.GetRepository<ExportTimeLog>();
        }

        public async Task<IResult<PagedResult<TimeLogResponseDto>>> GetAllAsync(PaginationParams paginationParams)
        {
            var user = await _userRepository.Table
                .Include(x => x.TimeLogs)
                .ThenInclude(x => x.File)
                .Include(x => x.TimeLogs)
                .ThenInclude(x => x.Topic)
                .Include(x => x.TimeLogs)
                .ThenInclude(x => x.Description)
                .Where(x => x.TimeLogs.Any(t => t.Date >= DateTime.Now.AddMonths(-6)))
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<PagedResult<TimeLogResponseDto>>.CreateFailed(ValidationFactory.AccessDenied);

            var timeLogs = user.TimeLogs
                .Where(x => x.Date >= DateTime.Now.AddMonths(-6))
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => _mapper.Map<TimeLogResponseDto>(x))
                .AsQueryable();

            var pagedResult = await new PagedResult<TimeLogResponseDto>().CreateAsync(timeLogs,
                paginationParams.PageNumber, paginationParams.PageSize);

            return Result<PagedResult<TimeLogResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<PagedResult<TimeLogResponseDto>>> GetDailyAsync(PaginationParams paginationParams)
        {
            var timeLogs = await _timeLogRepository.Table
                .Include(x => x.File)
                .Include(x => x.Topic)
                .Include(x => x.Description)
                .Where(x => x.Date.Value.Date == _userInfo.UserTime.Value.Date && x.UserId == _userInfo.UserId)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => _mapper.Map<TimeLogResponseDto>(x))
                .ToListAsync();

            var pagedResult = await new PagedResult<TimeLogResponseDto>().CreateAsync(timeLogs.AsQueryable(),
                paginationParams.PageNumber, paginationParams.PageSize);

            return Result<PagedResult<TimeLogResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<PagedResult<TimeLogResponseDto>>> GetMonthlyAsync(PaginationParams paginationParams)
        {
            var timeLogs = await _timeLogRepository.Table
                .Include(x => x.File)
                .Include(x => x.Topic)
                .Include(x => x.Description)
                .Where(x => x.Date.Value.Year == _userInfo.UserTime.Value.Year
                            && x.Date.Value.Month == _userInfo.UserTime.Value.Month
                            && x.UserId == _userInfo.UserId)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => _mapper.Map<TimeLogResponseDto>(x))
                .ToListAsync();

            var pagedResult = await new PagedResult<TimeLogResponseDto>().CreateAsync(timeLogs.AsQueryable(),
                paginationParams.PageNumber, paginationParams.PageSize);

            return Result<PagedResult<TimeLogResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<TimeLogResponseDto>> GetByIdAsync(int id)
        {
            var user = await _userRepository.Table
                .Include(x => x.TimeLogs)
                .ThenInclude(x => x.File)
                .Include(x => x.TimeLogs)
                .ThenInclude(x => x.Topic)
                .Include(x => x.TimeLogs)
                .ThenInclude(x => x.Description)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var timeLog = user.TimeLogs
                .FirstOrDefault(x => x.Id == id);

            return timeLog is null
                ? Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.TimeLogIsNotFound)
                : Result<TimeLogResponseDto>.CreateSuccess(_mapper.Map<TimeLogResponseDto>(timeLog));
        }

        public async Task<IResult<PagedResult<TimeLogResponseDto>>> SearchAsync(PaginationParams paginationParams,
            string query,
            DateTime? DateFrom, DateTime? DateTo)
        {
            DateTime From = DateFrom ?? DateTime.Now.AddMonths(-6);
            DateTime To = DateTo ?? DateTime.Now;
            var timeLogs = await _timeLogRepository.Table
                .Include(x => x.File)
                .Include(x => x.Topic)
                .Include(x => x.Description)
                .Where(x => x.UserId == _userInfo.UserId &&
                            (x.File.Name.Contains(query) ||
                             x.Topic.Name.Contains(query) ||
                             x.Description.Name.Contains(query)))
                .Where(x => x.Date >= From && x.Date <= To)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => _mapper.Map<TimeLogResponseDto>(x))
                .ToListAsync();

            var pagedResult = await new PagedResult<TimeLogResponseDto>().CreateAsync(timeLogs.AsQueryable(),
                paginationParams.PageNumber, paginationParams.PageSize);

            return Result<PagedResult<TimeLogResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<PagedResult<TimeLogResponseDto>>> SearchMonthlyAsync(
            PaginationParams paginationParams, string query)
        {
            var timeLogs = await _timeLogRepository.Table
                .Include(x => x.File)
                .Include(x => x.Topic)
                .Include(x => x.Description)
                .Where(x => x.UserId == _userInfo.UserId
                            && x.Date.Value.Year == _userInfo.UserTime.Value.Year
                            && x.Date.Value.Month == _userInfo.UserTime.Value.Month
                            && (x.File.Name.Contains(query)
                                || x.Topic.Name.Contains(query)
                                || x.Description.Name.Contains(query)))
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => _mapper.Map<TimeLogResponseDto>(x))
                .ToListAsync();

            var pagedResult = await new PagedResult<TimeLogResponseDto>().CreateAsync(timeLogs.AsQueryable(),
                paginationParams.PageNumber, paginationParams.PageSize);

            return Result<PagedResult<TimeLogResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<TimeLogStatisticDto>> GetStatisticAsync()
        {
            using var scope = _provider.CreateScope();
            _csvConfiguration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var userDateTime = _userInfo.UserTime ?? DateTimeOffset.UtcNow;
            var minutesToNextSync = int.Parse(_csvConfiguration["ImportCSV:TimeDelay"]);

            var timeLogs = await _timeLogRepository.Table
                .Where(x => x.UserId == _userInfo.UserId && x.Date.Value.Year == userDateTime.Year &&
                            x.Date.Value.Month == userDateTime.Month)
                .Select(x => new {x.Date, x.WorkTime})
                .ToListAsync();

            var syncTime =
                await _exportTimeLogRepository.Table.OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync() ??
                new ExportTimeLog {CreatedOn = DateTime.UtcNow};

            DateTimeOffset nextSyncTime = DateTime.SpecifyKind(syncTime.CreatedOn.Value.AddMinutes(minutesToNextSync),
                DateTimeKind.Utc);
            DateTimeOffset previousSyncTime = DateTime.SpecifyKind(syncTime.CreatedOn.Value, DateTimeKind.Utc);

            var statistic = new TimeLogStatisticDto
            {
                Date = userDateTime.Date.ToString("dd-MM-yyyy"),
                WorkTimeToday = timeLogs.Where(x => x.Date.Value.Date == userDateTime.Date).Select(x => x.WorkTime)
                    .Sum(),
                WorkTimeMonthly = timeLogs.Select(x => x.WorkTime).Sum(),
                NextSyncTime = nextSyncTime.ToOffset(userDateTime.Offset).ToString("yyyy-MM-ddTHH:mm:ss zzz"),
                PreviousSyncTime = previousSyncTime.ToOffset(userDateTime.Offset).ToString("yyyy-MM-ddTHH:mm:ss zzz")
            };

            return Result<TimeLogStatisticDto>.CreateSuccess(statistic);
        }

        public async Task<IResult<TimeLogResponseDto>> CreateAsync(TimeLogCreateRequestDto dto)
        {
            var user = await _userRepository.Table
                .Include(x => x.TimeLogs)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            if (!user.EmployeeSoftwareInvention)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.NotEnoughPermissionToTrackTheTime)
                    .AddError(ValidationFactory.TimeLogIsNotCreated);

            var userDateTime = _userInfo.UserTime ?? DateTimeOffset.UtcNow;

            if (!IsNotFutureDate(dto.Date, userDateTime))
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.NotAllowedDate)
                    .AddError(ValidationFactory.TimeLogIsNotCreated);

            var usersTimeLogs = user.TimeLogs.Where(x => x.Date != null && x.Date.Value.Date == dto.Date.Date).ToList();

            if (!IsValidWorkTime(usersTimeLogs, dto.WorkTime))
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.NotAllowedAmountOfTime)
                    .AddError(ValidationFactory.TimeLogIsNotCreated);

            var file = await _fileRepository.GetByIdAsync(dto.FileId);

            if (file is null)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied)
                    .AddError(ValidationFactory.FileIsNotFound);

            if (file.PortfolioStatus == CommitFileStatus.Closed)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.FileIsClosed)
                    .AddError(ValidationFactory.TimeLogIsNotCreated);

            if (!file.IsChargedCase)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.NonBillableCase)
                    .AddError(ValidationFactory.TimeLogIsNotCreated);

            if (dto.TopicId != null && dto.TopicId != 0)
            {
                var topic = await _topicRepository.Table
                    .FirstOrDefaultAsync(x => x.Id == dto.TopicId && (x.UserId == user.Id || !x.IsCustom));

                if (topic is null)
                    return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied)
                        .AddError(ValidationFactory.TopicIsNotFound);
            }

            if (dto.DescriptionId != null && dto.DescriptionId != 0)
            {
                var description = await _descriptionRepository.Table
                    .FirstOrDefaultAsync(x => x.Id == dto.DescriptionId && (x.UserId == user.Id || !x.IsCustom));

                if (description is null)
                    return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied)
                        .AddError(ValidationFactory.DescriptionIsNotFound);
            }

            var timeLog = _mapper.Map<TimeLog>(dto);
            timeLog.UserId = user.Id;

            // Only for Apple reviewing: TimeLog.ForExport = false
            if (user.CommitId < 0)
                timeLog.ForExport = false;

            var newTimeLog = await _timeLogRepository.InsertAsync(timeLog);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TimeLogResponseDto>.CreateSuccess(_mapper.Map<TimeLogResponseDto>(newTimeLog))
                : Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.TimeLogIsNotCreated);
        }

        public async Task<IResult<TimeLogResponseDto>> UpdateAsync(TimeLogUpdateRequestDto dto)
        {
            var user = await _userRepository.Table
                .Include(x => x.TimeLogs)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var timeLog = user.TimeLogs
                .FirstOrDefault(x => x.Id == dto.Id);

            if (timeLog?.BillingStatus == CommitBillingStatus.Charged)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.RecordIsCharged)
                    .AddError(ValidationFactory.TimeLogIsNotUpdated);

            var userDateTime = _userInfo.UserTime ?? DateTimeOffset.UtcNow;

            if (!IsNotFutureDate(dto.Date, userDateTime))
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.NotAllowedDate)
                    .AddError(ValidationFactory.TimeLogIsNotCreated);

            var usersTimeLogs = user.TimeLogs.Where(x => x.Date != null && x.Date.Value.Date == dto.Date.Date).ToList();

            if (!IsValidWorkTimeForUpdate(usersTimeLogs, dto))
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.NotAllowedAmountOfTime)
                    .AddError(ValidationFactory.TimeLogIsNotUpdated);

            var file = await _fileRepository.GetByIdAsync(dto.FileId);

            if (file is null)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.FileIsNotFound)
                    .AddError(ValidationFactory.TimeLogIsNotUpdated);

            if (file.PortfolioStatus == CommitFileStatus.Closed)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.FileIsClosed)
                    .AddError(ValidationFactory.TimeLogIsNotUpdated);

            if (!file.IsChargedCase)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.NonBillableCase)
                    .AddError(ValidationFactory.TimeLogIsNotUpdated);

            if (dto.TopicId != null && dto.TopicId != 0)
            {
                var topic = await _topicRepository.Table
                    .FirstOrDefaultAsync(x => x.Id == dto.TopicId && (x.UserId == user.Id || !x.IsCustom));

                if (topic is null)
                    return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied)
                        .AddError(ValidationFactory.TopicIsNotFound);
            }

            if (dto.DescriptionId != null && dto.DescriptionId != 0)
            {
                var description = await _descriptionRepository.Table
                    .FirstOrDefaultAsync(x => x.Id == dto.DescriptionId && (x.UserId == user.Id || !x.IsCustom));

                if (description is null)
                    return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied)
                        .AddError(ValidationFactory.DescriptionIsNotFound);
            }

            var timeLogToUpdate = _mapper.Map(dto, timeLog);

            // Only for Apple reviewing: TimeLog.ForExport = false
            if (user.CommitId < 0)
                timeLogToUpdate.ForExport = false;

            var newTimeLog = await _timeLogRepository.UpdateAsync(timeLogToUpdate);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TimeLogResponseDto>.CreateSuccess(_mapper.Map<TimeLogResponseDto>(newTimeLog))
                : Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.TimeLogIsNotCreated);
        }

        public async Task<IResult<TimeLogResponseDto>> DeleteAsync(int id)
        {
            var user = await _userRepository.Table
                .Include(x => x.TimeLogs)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var timeLog = user.TimeLogs
                .FirstOrDefault(x => x.Id == id);

            if (timeLog is null)
                return Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.TimeLogIsNotDeleted)
                    .AddError(ValidationFactory.TimeLogIsNotFound);

            await _timeLogRepository.DeleteAsync(timeLog.Id);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TimeLogResponseDto>.CreateSuccess(_mapper.Map<TimeLogResponseDto>(timeLog))
                : Result<TimeLogResponseDto>.CreateFailed(ValidationFactory.TopicIsNotDeleted);
        }

        #region private

        private static bool IsValidWorkTime(IEnumerable<TimeLog> timeLogs, int workingTime)
        {
            const int maxWorkTime = 1435; // max working time per day (23h 55m)

            if (workingTime <= 0 || workingTime > maxWorkTime)
                return false;

            var time = timeLogs
                .Select(x => x.WorkTime)
                .Sum() + workingTime;

            return time <= maxWorkTime;
        }

        private static bool IsValidWorkTimeForUpdate(IEnumerable<TimeLog> timeLogs, TimeLogUpdateRequestDto dto)
        {
            const int maxWorkTime = 1435; // max working time per day (23h 55m)

            if (dto.WorkTime <= 0 || dto.WorkTime > maxWorkTime)
                return false;

            var time = timeLogs
                .Where(x => x.Id != dto.Id)
                .Select(x => x.WorkTime)
                .Sum() + dto.WorkTime;

            return time <= maxWorkTime;
        }

        private static bool IsNotFutureDate(DateTime date, DateTimeOffset userDateTime) =>
            date.Date <= userDateTime.Date;

        #endregion
    }
}
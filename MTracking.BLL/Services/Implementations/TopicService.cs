using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Topic.Request;
using MTracking.BLL.DTOs.Topic.Response;
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
    public class TopicService : ITopicService
    {
        private readonly IUserInfoService _userInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<TimeLog> _timeLogRepository;

        public TopicService(IUnitOfWork unitOfWork, IMapper mapper, IUserInfoService userInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userInfo = userInfo;
            _userRepository = unitOfWork.GetRepository<User>();
            _topicRepository = unitOfWork.GetRepository<Topic>();
            _timeLogRepository = unitOfWork.GetRepository<TimeLog>();
        }

        public async Task<IResult<TopicResponseDto>> GetByIdAsync(int topicId)
        {
            var user = await _userRepository.GetByIdAsync(_userInfo.UserId);

            if (user is null)
                return Result<TopicResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var topic = await _topicRepository.Table
                .FirstOrDefaultAsync(x => x.Id == topicId && (x.UserId == user.Id || !x.IsCustom));

            return topic is null
                ? Result<TopicResponseDto>.CreateFailed(ValidationFactory.TopicIsNotFound)
                : Result<TopicResponseDto>.CreateSuccess(_mapper.Map<TopicResponseDto>(topic));
        }

        public async Task<IResult<PagedResult<TopicResponseDto>>> GetAllAsync(PaginationParams paginationParams)
        {
            var timeLogs = await _timeLogRepository.Table
                .Include(x => x.Topic)
                .Where(x => x.UserId == _userInfo.UserId && x.TopicId.HasValue)
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .ToListAsync();

            var dbTopics = await _topicRepository.Table
                .Where(x => (x.UnitType == BillingCodeUnitType.OwedVat || x.UnitType == null) &&
                            (x.UserId == null || x.UserId == _userInfo.UserId))
                .Select(x => _mapper.Map<TopicResponseDto>(x))
                .ToListAsync();

            var topics = timeLogs
                .Select(x => x.Topic)
                .Where(x => (x.UnitType == BillingCodeUnitType.OwedVat || x.UnitType == null) &&
                            (x.UserId == null || x.UserId == _userInfo.UserId))
                .GroupBy(x => x.Id)
                .Select(x => _mapper.Map<TopicResponseDto>(x.Select(p => p).First()))
                .ToList();

            topics.AddRange(dbTopics.Where(x => topics.Select(z => z.Id).All(c => c != x.Id)));

            var pagedResult = await new PagedResult<TopicResponseDto>().CreateAsync(topics.AsQueryable(),
                paginationParams.PageNumber, paginationParams.PageSize);

            return Result<PagedResult<TopicResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<PagedResult<TopicResponseDto>>> SearchTopicsAsync(PaginationParams paginationParams,
            string query)
        {
            var topics = _topicRepository.Table
                .Where(x =>
                    (x.Name.Contains(query)) &&
                    ((x.UnitType == BillingCodeUnitType.OwedVat || x.UnitType == null) &&
                     (x.UserId == null || x.UserId == _userInfo.UserId)))
                .OrderByDescending(x => x.TimeLogs.Count())
                .ThenByDescending(x => x.IsCustom)
                .ThenByDescending(x => x.CreatedOn)
                .Select(x => _mapper.Map<TopicResponseDto>(x));

            var pagedResult = await new PagedResult<TopicResponseDto>().CreateAsync(topics, paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PagedResult<TopicResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<TopicResponseDto>> CreateAsync(TopicCreateRequestDto dto)
        {
            var newTopic = await _topicRepository.InsertAsync(_mapper.Map<Topic>(dto));

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TopicResponseDto>.CreateSuccess(_mapper.Map<TopicResponseDto>(newTopic))
                : Result<TopicResponseDto>.CreateFailed(ValidationFactory.TopicIsNotCreated);
        }

        public async Task<IResult<TopicResponseDto>> CreateCustomAsync(TopicCreateRequestDto dto)
        {
            var user = await _userRepository.Table
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<TopicResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var topic = _mapper.Map<Topic>(dto);
            topic.UserId = user.Id;
            topic.IsCustom = true;

            var newTopic = await _topicRepository.InsertAsync(topic);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TopicResponseDto>.CreateSuccess(_mapper.Map<TopicResponseDto>(newTopic))
                : Result<TopicResponseDto>.CreateFailed(ValidationFactory.TopicIsNotCreated);
        }

        public async Task<IResult<TopicResponseDto>> UpdateAsync(TopicUpdateRequestDto dto)
        {
            var user = await _userRepository.Table
                .Include(x => x.Topics)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<TopicResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var topic = user.Topics
                .FirstOrDefault(x => x.Id == dto.Id);

            if (topic is null)
                return Result<TopicResponseDto>.CreateFailed(ValidationFactory.TopicIsNotUpdated)
                    .AddError(ValidationFactory.TopicIsNotFound);

            var updatedTopic = await _topicRepository.UpdateAsync(_mapper.Map(dto, topic));

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TopicResponseDto>.CreateSuccess(_mapper.Map<TopicResponseDto>(updatedTopic))
                : Result<TopicResponseDto>.CreateFailed(ValidationFactory.TopicIsNotUpdated);
        }

        public async Task<IResult<TopicResponseDto>> DeleteAsync(int topicId)
        {
            var user = await _userRepository.Table
                .Include(x => x.Topics)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<TopicResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var topic = user.Topics
                .FirstOrDefault(x => x.Id == topicId);

            if (topic is null)
                return Result<TopicResponseDto>.CreateFailed(ValidationFactory.TopicIsNotDeleted)
                    .AddError(ValidationFactory.TopicIsNotFound);

            await _topicRepository.DeleteAsync(topic.Id);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TopicResponseDto>.CreateSuccess(_mapper.Map<TopicResponseDto>(topic))
                : Result<TopicResponseDto>.CreateFailed(ValidationFactory.TopicIsNotDeleted);
        }
    }
}
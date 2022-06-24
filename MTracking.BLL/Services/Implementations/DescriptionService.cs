using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Description.Request;
using MTracking.BLL.DTOs.Description.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Entities;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.BLL.Services.Implementations
{
    public class DescriptionService : IDescriptionService
    {
        private readonly IUserInfoService _userInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Description> _descriptionRepository;
        private readonly IRepository<TimeLog> _timeLogRepository;

        public DescriptionService(IUnitOfWork unitOfWork, IMapper mapper, IUserInfoService userInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = unitOfWork.GetRepository<User>();
            _timeLogRepository = unitOfWork.GetRepository<TimeLog>();
            _descriptionRepository = unitOfWork.GetRepository<Description>();
            _userInfo = userInfo;
        }

        public async Task<IResult<DescriptionResponseDto>> GetByIdAsync(int descriptionId)
        {
            var user = await _userRepository.Table
                .Include(x => x.Descriptions)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var description = user.Descriptions
                .FirstOrDefault(x => x.Id == descriptionId);

            return description is null
                ? Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.DescriptionIsNotFound)
                : Result<DescriptionResponseDto>.CreateSuccess(_mapper.Map<DescriptionResponseDto>(description));
        }

        public async Task<IResult<PagedResult<DescriptionResponseDto>>> GetAllAsync(PaginationParams paginationParams)
        {
            var timeLogs = await _timeLogRepository.Table
                .Include(x => x.Description)
                .Where(x => x.UserId == _userInfo.UserId && x.DescriptionId.HasValue)
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .ToListAsync();

            var descriptions = timeLogs
                .Select(x => _mapper.Map<DescriptionResponseDto>(x.Description));

            var lookup = descriptions.ToLookup(x => x.Id, v => v).Select(x => x.First()).AsQueryable();

            var pagedResult = await new PagedResult<DescriptionResponseDto>().CreateAsync(lookup, paginationParams.PageNumber, paginationParams.PageSize);

            return Result<PagedResult<DescriptionResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<PagedResult<DescriptionResponseDto>>> SearchDescriptionsAsync(PaginationParams paginationParams, string query)
        {
            var descriptions = _descriptionRepository.Table
                .Where(x => x.UserId == _userInfo.UserId && x.Name.Contains(query))
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => _mapper.Map<DescriptionResponseDto>(x));

            var pagedResult = await new PagedResult<DescriptionResponseDto>().CreateAsync(descriptions, paginationParams.PageNumber, paginationParams.PageSize);

            return Result<PagedResult<DescriptionResponseDto>>.CreateSuccess(pagedResult);
        }

        public async Task<IResult<DescriptionResponseDto>> CreateAsync(DescriptionCreateRequestDto dto)
        {
            var newDescription = await _descriptionRepository.InsertAsync(_mapper.Map<Description>(dto));

            return await _unitOfWork.SaveAsync() > 0
                ? Result<DescriptionResponseDto>.CreateSuccess(_mapper.Map<DescriptionResponseDto>(newDescription))
                : Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.DescriptionIsNotCreated);
        }

        public async Task<IResult<DescriptionResponseDto>> CreateCustomAsync(DescriptionCreateRequestDto dto)
        {
            var user = await _userRepository.Table
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var description = _mapper.Map<Description>(dto);
            description.UserId = user.Id;
            description.IsCustom = true;

            var newDescription = await _descriptionRepository.InsertAsync(description);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<DescriptionResponseDto>.CreateSuccess(_mapper.Map<DescriptionResponseDto>(newDescription))
                : Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.DescriptionIsNotCreated);
        }

        public async Task<IResult<DescriptionResponseDto>> UpdateAsync(DescriptionUpdateRequestDto dto)
        {
            var user = await _userRepository.Table
                .Include(x => x.Descriptions)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var description = user.Descriptions
                .FirstOrDefault(x => x.Id == dto.Id);

            if (description is null)
                return Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.DescriptionIsNotUpdated).AddError(ValidationFactory.DescriptionIsNotFound);

            var updatedDescription = await _descriptionRepository.UpdateAsync(_mapper.Map(dto, description));

            return await _unitOfWork.SaveAsync() > 0
                ? Result<DescriptionResponseDto>.CreateSuccess(_mapper.Map<DescriptionResponseDto>(updatedDescription))
                : Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.DescriptionIsNotUpdated);
        }

        public async Task<IResult<DescriptionResponseDto>> DeleteAsync(int descriptionId)
        {
            var user = await _userRepository.Table
                .Include(x => x.Descriptions)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var description = user.Descriptions
                .FirstOrDefault(x => x.Id == descriptionId);

            if (description is null)
                return Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.DescriptionIsNotDeleted).AddError(ValidationFactory.DescriptionIsNotFound);

            await _descriptionRepository.DeleteAsync(description.Id);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<DescriptionResponseDto>.CreateSuccess(_mapper.Map<DescriptionResponseDto>(description))
                : Result<DescriptionResponseDto>.CreateFailed(ValidationFactory.DescriptionIsNotDeleted);
        }
    }
}

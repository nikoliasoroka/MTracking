using System.Threading.Tasks;
using MTracking.BLL.Models.Abstractions.Generics;

namespace MTracking.BLL.Services.Abstractions
{
    public interface ICrudOperation<TEntityDto, in TEntityCreateDto, in TEntityUpdateDto>
        where TEntityDto : class, new()
        where TEntityCreateDto : class, new()
        where TEntityUpdateDto : class, new()
    {
        Task<IResult<TEntityDto>> GetByIdAsync(int id);

        Task<IResult<TEntityDto>> CreateAsync(TEntityCreateDto dto);

        Task<IResult<TEntityDto>> UpdateAsync(TEntityUpdateDto dto);

        Task<IResult<TEntityDto>> DeleteAsync(int id);
    }
}

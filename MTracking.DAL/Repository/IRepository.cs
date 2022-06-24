using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTracking.Core.Entities.Abstractions;

namespace MTracking.DAL.Repository
{
    public interface IRepository<TEntity> where TEntity : class, ITrackedEntity, IEntity<int>
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<TEntity> InsertAsync(TEntity entity);

        Task InsertRangeAsync(IEnumerable<TEntity> entities);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task UpdateRangeAsync(IEnumerable<TEntity> entities);

        Task DeleteAsync(int id);

        IQueryable<TEntity> Table { get; }
    }
}

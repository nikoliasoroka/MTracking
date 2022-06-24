using System;
using System.Threading.Tasks;
using MTracking.Core.Entities.Abstractions;
using MTracking.DAL.Repository;

namespace MTracking.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, ITrackedEntity, IEntity<int>;

        Task<int> SaveAsync();
    }
}

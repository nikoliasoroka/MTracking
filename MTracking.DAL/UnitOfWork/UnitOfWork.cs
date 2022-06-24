using System;
using System.Threading.Tasks;
using MTracking.Core.Entities.Abstractions;
using MTracking.Core.Logger;
using MTracking.DAL.Repository;

namespace MTracking.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MTrackingDbContext _context;
        private bool _disposed;

        public UnitOfWork(MTrackingDbContext context)
        {
            _context = context;
            _disposed = false;
        }

        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class, ITrackedEntity, IEntity<int>
        {
            var repository = new Repository<TEntity>(_context);

            return repository;
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch(Exception exception)
            {
                Logger.Error($"Can't save data to DB", exception);
                return -1;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _context.Dispose();

                _disposed = true;
            }
        }
    }
}

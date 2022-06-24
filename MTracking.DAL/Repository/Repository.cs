using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTracking.Core.Entities.Abstractions;

namespace MTracking.DAL.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, ITrackedEntity, IEntity<int>
    {
        private readonly MTrackingDbContext _context;

        protected virtual DbSet<TEntity> Entities => _context.Set<TEntity>();

        public IQueryable<TEntity> Table => Entities;

        public Repository(MTrackingDbContext context)
        {
            _context = context;
        }
        
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await Entities.FindAsync(id);
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
            var addResult = await Entities.AddAsync(entity);

            return addResult.Entity;
        }

        public async Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() =>
            {
                Entities.AddRangeAsync(entities);
            });
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                entity.UpdatedOn = DateTime.UtcNow;
                var updatedResult = Entities.Update(entity);

                return updatedResult.Entity;
            });
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() =>
            {
                Entities.UpdateRange(entities);
            });
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await Entities.FindAsync(id);

            Entities.Remove(entity);
        }
    }
}

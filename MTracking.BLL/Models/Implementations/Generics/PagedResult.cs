using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTracking.BLL.Models.Implementations.Generics
{
    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalNumberOfPages { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public IEnumerable<T> Results { get; set; }

        public async Task<PagedResult<T>> CreateAsync(IQueryable<T> queryable, int page, int pageSize)
        {
            var skipAmount = pageSize * (page - 1);

            var projection = queryable.Skip(skipAmount).Take(pageSize);

            int totalNumberOfRecords;
            List<T> results;

            try
            {
                totalNumberOfRecords = await queryable.CountAsync().ConfigureAwait(false);
                results = await projection.ToListAsync().ConfigureAwait(false);
            }
            catch
            {
                totalNumberOfRecords = queryable.Count();
                results = projection.ToList();
            }

            var mod = totalNumberOfRecords % pageSize;
            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);

            return new PagedResult<T>
            {
                Results = results,
                PageNumber = page,
                PageSize = results.Count,
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords
            };
        }
    }
}

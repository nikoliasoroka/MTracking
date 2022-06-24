using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.Core.Entities;
using MTracking.Core.Enums;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.API.Controllers
{
    /// <summary>
    /// [System]
    /// </summary>
    [Route("api/imp-exp")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IRepository<Import> _importRepository;
        private readonly IRepository<ExportTimeLog> _exportRepository;

        public ImportController(IUnitOfWork unitOfWork)
        {
            _importRepository = unitOfWork.GetRepository<Import>();
            _exportRepository = unitOfWork.GetRepository<ExportTimeLog>();
        }

        /// <summary>
        /// [System] Get import records. 
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        [HttpGet("import")]
        [ProducesResponseType(typeof(Import), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetImport([FromQuery] PaginationParams paginationParams, [FromQuery]ImportFileType fileType)
        {
            List<Import> import;

            if (fileType == ImportFileType.All)
            {
                import = _importRepository.Table
                    .OrderByDescending(x => x.Id)
                    .ToList();
            }
            else
            {
                import = _importRepository.Table
                    .Where(x => x.FileType == fileType)
                    .OrderByDescending(x => x.Id)
                    .ToList();
            }
            
            var pagedResult = await new PagedResult<Import>().CreateAsync(import.AsQueryable(), paginationParams.PageNumber, paginationParams.PageSize);

            return Ok(pagedResult);
        }

        /// <summary>
        /// [System] Get inserted files.
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("export")]
        [ProducesResponseType(typeof(Import), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetExport([FromQuery] PaginationParams paginationParams)
        {
            var export = _exportRepository.Table
                .OrderByDescending(x => x.Id)
                .ToList()
                .AsQueryable();

            var pagedResult = await new PagedResult<ExportTimeLog>().CreateAsync(export, paginationParams.PageNumber, paginationParams.PageSize);

            return Ok(pagedResult);
        }
    }
}

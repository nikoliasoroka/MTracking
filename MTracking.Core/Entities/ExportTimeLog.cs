using System;
using MTracking.Core.Entities.Abstractions;

namespace MTracking.Core.Entities
{
    public class ExportTimeLog : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int Records { get; set; }
        public string FileName { get; set; }
    }
}

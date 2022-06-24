using System;
using MTracking.Core.Entities.Abstractions;
using MTracking.Core.Enums;

namespace MTracking.Core.Entities
{
    public class Import : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ImportFileType FileType { get; set; }
        public string FileName { get; set; }
        public int? InsertedRecords { get; set; }
        public int? UpdatedRecords { get; set; }
        public double? Performance { get; set; }
    }
}

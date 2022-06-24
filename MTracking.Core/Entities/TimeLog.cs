using System;
using MTracking.Core.Entities.Abstractions;
using MTracking.Core.Enums;

namespace MTracking.Core.Entities
{
    public class TimeLog : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int WorkTime { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? CommitRecordId { get; set; }
        public bool isSynchronize { get; set; }
        public DateTime? BillingDateCreation { get; set; }
        public bool ForExport { get; set; }
        public CommitBillingStatus? BillingStatus { get; set; }

        public int? FileId { get; set; }
        public File File { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int? TopicId { get; set; }
        public Topic Topic { get; set; }

        public int? DescriptionId { get; set; }
        public Description Description { get; set; }
    }
}
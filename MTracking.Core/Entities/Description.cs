using System;
using System.Collections.Generic;
using MTracking.Core.Entities.Abstractions;

namespace MTracking.Core.Entities
{
    public class Description : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCustom { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? CommitRecordId { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public ICollection<TimeLog> TimeLogs { get; set; }
    }
}

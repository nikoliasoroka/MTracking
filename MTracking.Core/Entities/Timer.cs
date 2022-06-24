using System;
using MTracking.Core.Entities.Abstractions;

namespace MTracking.Core.Entities
{
    public class Timer : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? StopTime { get; set; }
        public long Duration { get; set; }
        public bool IsTimerActive { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}

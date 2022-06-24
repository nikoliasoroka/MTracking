using System;

namespace MTracking.Core.Entities.Abstractions
{
    public interface ITrackedEntity
    {
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}

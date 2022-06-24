using System;
using System.Collections.Generic;
using MTracking.Core.Entities.Abstractions;
using MTracking.Core.Enums;

namespace MTracking.Core.Entities
{
    public class Topic : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? BillingCodeId { get; set; }
        public string Detail { get; set; }
        public BillingCodeUnitType? UnitType { get; set; }
        public bool? IsNotary { get; set; }
        public bool IsCustom { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }

        public ICollection<TimeLog> TimeLogs { get; set; }
    }
}
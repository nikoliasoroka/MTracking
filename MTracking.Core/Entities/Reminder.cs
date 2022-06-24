using System;
using MTracking.Core.Entities.Abstractions;

namespace MTracking.Core.Entities
{
    public class Reminder : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool IsActive { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}

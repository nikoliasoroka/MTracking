using System;
using MTracking.Core.Entities.Abstractions;
using MTracking.Core.Enums;

namespace MTracking.Core.Entities
{
    public class Device : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string FirebaseToken { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string FirebaseInstanceId { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}

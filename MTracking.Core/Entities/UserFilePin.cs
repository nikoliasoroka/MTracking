using System;
using System.Collections;
using System.Collections.Generic;
using MTracking.Core.Entities.Abstractions;

namespace MTracking.Core.Entities
{
    public class UserFilePin : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int FileId { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public File File { get; set; }
    }
}
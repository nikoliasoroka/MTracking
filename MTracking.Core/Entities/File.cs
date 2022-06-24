using System;
using System.Collections;
using System.Collections.Generic;
using MTracking.Core.Entities.Abstractions;
using MTracking.Core.Enums;

namespace MTracking.Core.Entities
{
    public class File : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string PortfolioNumber { get; set; }
        public CommitFileStatus PortfolioStatus { get; set; }
        public string CaseName { get; set; }
        public string EnglishCaseName { get; set; }
        public DateTime? OpeningOn { get; set; }
        public DateTime? ClosingOn { get; set; }
        public bool IsChargedCase { get; set; }

        public ICollection<UserFilePin> UserFilePins { get; set; }
        public ICollection<TimeLog> TimeLogs { get; set; }
    }
}
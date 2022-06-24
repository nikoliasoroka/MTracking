using System;
using System.Collections.Generic;
using MTracking.Core.Entities.Abstractions;

namespace MTracking.Core.Entities
{
    public class User : IEntity<int>, ITrackedEntity
    {
        public int Id { get; set; }
        public int CommitId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsPasswordChanged { get; set; }
        public string HebrewName { get; set; }
        public string EnglishName { get; set; }
        public string UserName { get; set; }
        public bool EmployeeSoftwareInvention { get; set; }
        public bool EmployeeRunCommit { get; set; }
        public string Email { get; set; }
        public string VerificationCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public ICollection<Topic> Topics { get; set; }
        public ICollection<Description> Descriptions { get; set; }
        public ICollection<TimeLog> TimeLogs { get; set; }
        public ICollection<Reminder> Reminders { get; set; }
        public ICollection<Device> Devices { get; set; }
    }
}

using System;

namespace MTracking.BLL.BackgroundServices.Models
{
    public class HoursBillingParsed
    {
        public int WorkTime { get; set; }
        public int? CommitRecordId { get; set; }
        public int? InternalId { get; set; }
        public int? BillingStatus { get; set; }
        public bool IsCharged { get; set; }
        public int? CommitEmployeeId { get; set; }
        public DateTime? BillingCreatedOn { get; set; }
        public string FilePortfolioNumber { get; set; }
        public string BillingCodeTopic { get; set; }
        public int BillingType { get; set; }
        public bool IsNotary { get; set; }
        public string BillingDetails { get; set; }
        public DateTime? BillingUpdatedOn { get; set; }
        public DateTime? BillingDate { get; set; }
    }
}

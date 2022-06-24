namespace MTracking.BLL.BackgroundServices.Models
{
    public class InsertHoursCsv
    {
        public string CommitPortfolioNumber { get; set; }
        public string TimeLogId { get; set; }
        public string BillingDateYear { get; set; }
        public string BillingDateMonth { get; set; }
        public string BillingDateDay { get; set; }
        public string BillingDetail { get; set; }
        public string BillingDetailSecond { get; set; }
        public string BillingType { get; set; }
        public string BillingCode { get; set; }
        public string WorkTime { get; set; }
        public string CommitEmployeeId { get; set; }
        public string CommitRecordId { get; set; }
        public string Notary { get; set; }
    }
}

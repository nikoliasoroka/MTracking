using CsvHelper.Configuration;
using MTracking.BLL.BackgroundServices.Models;

namespace MTracking.BLL.BackgroundServices.Mappings
{
    public sealed class HoursBillingMapping : ClassMap<HoursBillingCsv>
    {
        public HoursBillingMapping()
        {
            Map(m => m.CommitRecordId).Name("1");
            Map(m => m.BillingStatus).Name("2");
            Map(m => m.IsCharged).Name("3");
            Map(m => m.CommitEmployeeId).Name("5");
            Map(m => m.BillingCreatedOn).Name("6");
            Map(m => m.FilePortfolioNumber).Name("7");
            Map(m => m.BillingCodeTopic).Name("8");
            Map(m => m.BillingType).Name("9");
            Map(m => m.IsNotary).Name("10");
            Map(m => m.WorkingHours).Name("12");
            Map(m => m.BillingDetails).Name("13");
            Map(m => m.BillingUpdatedOn).Name("17");
            Map(m => m.InternalId).Name("19");
            Map(m => m.BillingDate).Name("21");
        }
    }
}

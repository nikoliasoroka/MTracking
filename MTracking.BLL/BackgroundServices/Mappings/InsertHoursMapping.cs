using CsvHelper.Configuration;
using MTracking.BLL.BackgroundServices.Models;

namespace MTracking.BLL.BackgroundServices.Mappings
{
    public sealed class InsertHoursMapping : ClassMap<InsertHoursCsv>
    {
        public InsertHoursMapping()
        {
            Map(m => m.CommitPortfolioNumber).Index(0).Name("A");
            Map(m => m.TimeLogId).Index(1).Name("B");
            Map(m => m.BillingDateYear).Index(2).Name("C");
            Map(m => m.BillingDateMonth).Index(3).Name("D");
            Map(m => m.BillingDateDay).Index(4).Name("E");
            Map(m => m.BillingDetail).Index(5).Name("F");
            Map(m => m.BillingDetailSecond).Index(6).Name("G");
            Map(m => m.BillingType).Index(7).Name("H");
            Map(m => m.BillingCode).Index(8).Name("I");
            Map(m => m.WorkTime).Index(9).Name("J");
            Map(m => m.CommitEmployeeId).Index(10).Name("P");
            Map(m => m.CommitRecordId).Index(11).Name("Q");
            Map(m => m.Notary).Index(12).Name("R");
        }
    }
}

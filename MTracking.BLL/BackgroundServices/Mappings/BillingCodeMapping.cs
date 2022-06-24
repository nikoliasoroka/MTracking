using CsvHelper.Configuration;
using MTracking.BLL.BackgroundServices.Models;

namespace MTracking.BLL.BackgroundServices.Mappings
{
    public sealed class BillingCodeMapping : ClassMap<BillingCodeCsv>
    {
        public BillingCodeMapping()
        {
            Map(m => m.BillingCodeId).Name("1");
            Map(m => m.Name).Name("2");
            Map(m => m.Detail).Name("3");
            Map(m => m.UnitType).Name("4");
            Map(m => m.IsNotary).Name("5");
        }
    }
}

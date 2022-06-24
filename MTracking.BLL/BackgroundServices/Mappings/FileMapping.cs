using CsvHelper.Configuration;
using MTracking.BLL.BackgroundServices.Models;

namespace MTracking.BLL.BackgroundServices.Mappings
{
    public sealed class FileMapping : ClassMap<FileCsv>
    {
        public FileMapping()
        {
            Map(m => m.PortfolioNumber).Name("1");
            Map(m => m.PortfolioStatus).Name("2");
            Map(m => m.CaseName).Name("4");
            Map(m => m.EnglishCaseName).Name("5");
            Map(m => m.OpeningOn).Name("8");
            Map(m => m.ClosingOn).Name("9");
            Map(m => m.IsChargedCase).Name("11");
        }
    }
}

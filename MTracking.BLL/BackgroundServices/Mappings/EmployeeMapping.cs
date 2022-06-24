using CsvHelper.Configuration;
using MTracking.BLL.BackgroundServices.Models;

namespace MTracking.BLL.BackgroundServices.Mappings
{
    public sealed class EmployeeMapping : ClassMap<EmployeeCsv>
    {
        public EmployeeMapping()
        {
            Map(m => m.CommitId).Name("1");
            Map(m => m.HebrewName).Name("2");
            Map(m => m.EnglishName).Name("3");
            Map(m => m.EmployeeSoftwareInvention).Name("4");
            Map(m => m.EmployeeRunCommit).Name("5");
            Map(m => m.UserName).Name("8");
            Map(m => m.Email).Name("9");
        }
    }
}

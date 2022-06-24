using MTracking.BLL.DTOs.Comparing.Enums;

namespace MTracking.BLL.DTOs.Comparing
{
    public class ChangedObjectDto
    {
        public int? Id { get; set; }

        public string PortfolioNumber { get; set; }

        public RecordAction Action { get; set; }
    }
}

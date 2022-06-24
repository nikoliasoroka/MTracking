using System;

namespace MTracking.BLL.DTOs.TimeLog.Response
{
    public class TimeLogStatisticDto
    {
        public string Date { get; set; }
        public int WorkTimeToday { get; set; }
        public int WorkTimeMonthly { get; set; }
        public string NextSyncTime { get; set; }
        public string PreviousSyncTime { get; set; }
    }
}

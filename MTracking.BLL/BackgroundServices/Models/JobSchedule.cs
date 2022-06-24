using System;

namespace MTracking.BLL.BackgroundServices.Models
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, int minutes)
        {
            JobType = jobType;
            Minutes = minutes;
        }

        public Type JobType { get; }
        public int Minutes { get; }
    }
}

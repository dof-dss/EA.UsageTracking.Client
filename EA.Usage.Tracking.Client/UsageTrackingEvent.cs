using System;
using System.Collections.Generic;
using System.Text;

namespace EA.Usage.Tracking.Client
{
    public class UsageTrackingEvent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

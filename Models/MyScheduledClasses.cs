using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
  public  class MyScheduledClasses
    {
        public string id { get; set; }
        public string title { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string scheduledClassid { get; set; }
        public string metacode { get; set; }
        public string expiresOn { get; set; }
        public string videoCount { get; set; }
        public string cluster_type { get; set; }
        public bool IsScheduledCLass { get; set; }
        public bool IsMonthlyClass { get; set; }
        

    }
}

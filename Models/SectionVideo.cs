using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
    public class SectionVideo
    {
        public int id { get; set; }
        public string title { get; set; }
        public string videoServiceTypeMetacode { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string videoLink { get; set; }
        public string link { get; set; }// this is for scheduedClass
        public string videoIndex { get; set; }
        public string videoIndex2 { get; set; }
        public bool IsScheduledMonthlyCLass { get; set; }

    }
}

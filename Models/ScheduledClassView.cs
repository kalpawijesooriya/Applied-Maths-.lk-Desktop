using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
    public class ScheduledClassView : IScheduledClassView
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string startTime { get; set; }
        public string endTine { get; set; }
        public string availableFrom { get; set; }
        public string availableTill { get; set; }
        public string scheduledClassTypeMetacode { get; set; }
        public List<SectionVideo> classVideos { get; set; }
        public List<SectionTutorial> classTutorials { get; set; }
    }
}

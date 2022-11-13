using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
    public class Section
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool IsScheduledMonthlyCLass { get; set; }
        public List<SectionVideo> sectionVideos { get; set; }
        public List<SectionTutorial> sectionTutorials { get; set; }

    }
}

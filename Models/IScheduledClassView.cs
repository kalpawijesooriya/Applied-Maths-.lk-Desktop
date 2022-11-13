using System.Collections.Generic;

namespace appliedmaths.Models
{
    public interface IScheduledClassView
    {
        string availableFrom { get; set; }
        string availableTill { get; set; }
        List<SectionTutorial> classTutorials { get; set; }
        List<SectionVideo> classVideos { get; set; }
        string description { get; set; }
        string endTine { get; set; }
        string id { get; set; }
        string scheduledClassTypeMetacode { get; set; }
        string startTime { get; set; }
        string title { get; set; }
    }
}
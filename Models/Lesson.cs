using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
    public class Lesson
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string duration { get; set; }
        public string course_cluster_type_metacode { get; set; }
        public List<Section> sections { get; set; }
        public List<Tutorial> tutorials { get; set; }
    }
}

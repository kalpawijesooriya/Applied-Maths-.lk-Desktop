using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
   public class Anotate
    {
        public string type { get; set; } = "rtext";
       public string text { get; set; }
        public string alpha { get; set; } = "0.8";
        public string color { get; set; } = "0xFFD302";
        public string size { get; set; } = "15";
        public string interval { get; set; } = "2000";
        public string skip { get; set; } = "10000";
    }
}

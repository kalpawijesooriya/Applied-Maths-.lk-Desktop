using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
    public class MyPapers
    {
        public string title { get; set; }
        public string paperLink { get; set; }
        public string answerLink { get; set; }
        public PHPDateObject expiresOn { get; set; }
        public string expiresOnString { get; set; }

    }
}

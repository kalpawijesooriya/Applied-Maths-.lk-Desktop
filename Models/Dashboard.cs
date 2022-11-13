using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
    public class Dashboard
    {
        public List<Mycourses> mycourses { get; set; }
        public List<MyPapers> myPapers { get; set; }
        public List<MyClasses> myClasses { get; set; }
        public List<MyMonthlyClasses> myMonthlyClasses { get; set; }
        public string no_of_purchased_courses { get; set; }
        public string no_of_completed_courses { get; set; }
        public string no_of_pending_courses { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
   

    public class LoggedInUser : ILoggedInUser
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string district { get; set; }
        public string image { get; set; }
        public string school { get; set; }
        public string imagefullPath { get; set; }
        public string badgesPath { get; set; }
        public string contact { get; set; }
        public string address { get; set; }
        public string alYear { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
        public string email {get; set; }
}
}

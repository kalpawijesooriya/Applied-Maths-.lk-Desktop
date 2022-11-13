using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
    public interface ILoggedInUser
    {
        string userId { get; set; }
        string userName { get; set; }
        string district { get; set; }
        string image { get; set; }
        string school { get; set; }
        string imagefullPath { get; set; }
        string badgesPath { get; set; }
        string contact { get; set; }
        string address { get; set; }
        string alYear { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
        public string email { get; set; }
    }
}

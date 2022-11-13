using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
   public class AuthonticatedUser
    {
     
      public  string token_type { get; set; }
      public int expires_in { get; set; }
      public string access_token  { get; set; }
      public string refresh_token { get; set; }
      public string error { get; set; }
      public string error_description { get; set; }
      public string message { get; set; }
    }
}

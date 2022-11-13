using System;
using System.Collections.Generic;
using System.Text;

namespace appliedmaths.Models
{
    public class VdoCyperOTP : IVdoCyperOTP
    {
      public string otp { get; set; }
      public string playbackInfo { get; set; }
    }
}

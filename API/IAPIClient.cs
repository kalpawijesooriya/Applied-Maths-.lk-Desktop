using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace appliedmaths.API
{
   public interface IAPIClient
    {
        public HttpClient InitializeClient();
        public HttpClient VdoCyperClient();
     
    }
}

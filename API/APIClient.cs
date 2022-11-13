using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace appliedmaths.API
{
    class APIClient : IAPIClient
    {
        private HttpClient apiClient;
        private HttpClient VdoCyperApiClient;
       
      
        public HttpClient InitializeClient()
        {
            string baseUrl = ConfigurationManager.AppSettings.Get("BASE_URL");
            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(baseUrl);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return apiClient;
        }

        public HttpClient VdoCyperClient()
        {

            VdoCyperApiClient = new HttpClient();
            VdoCyperApiClient.BaseAddress = new Uri("https://dev.vdocipher.com/api/videos/");
            VdoCyperApiClient.DefaultRequestHeaders.Accept.Clear();
            VdoCyperApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return VdoCyperApiClient;
        }
    }
}

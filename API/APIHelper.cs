using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using appliedmaths.Helpers;
using appliedmaths.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace appliedmaths.API
{
    public class APIHelper : IAPIHelper
    {
        private IAPIClient _apiClient;
        private HttpClient httpClient;
        private HttpClient vdoCyperhttpClient;
        private IScheduledClassView _scheduledClassView;
        private ILoggedInUser _loggedInUser;
        public APIHelper(IAPIClient apiClient ,ILoggedInUser loggedInUser,IScheduledClassView scheduledClassView)
        {
            _apiClient = apiClient;
            httpClient = _apiClient.InitializeClient();
            vdoCyperhttpClient = _apiClient.VdoCyperClient();
            _loggedInUser = loggedInUser;
            _scheduledClassView = scheduledClassView;
        }


        public async Task<AuthonticatedUser> Authenticate(string username, string password)
        {
            string client_id = ConfigurationManager.AppSettings.Get("client_id");
            string client_secret = ConfigurationManager.AppSettings.Get("client_secret");
            var data = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("grant_type","password"),
            new KeyValuePair<string,string>("client_id",client_id),
            new KeyValuePair<string,string>("client_secret",client_secret),
            new KeyValuePair<string,string>("username",username),
            new KeyValuePair<string,string>("password",password),
            });
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using (HttpResponseMessage response = await httpClient.PostAsync("token", data))
            {

                if (response.IsSuccessStatusCode)
                {


                    var result = await response.Content.ReadAsAsync<AuthonticatedUser>();
                    _loggedInUser.token = result.access_token;
                    _loggedInUser.refreshToken = result.refresh_token;
                    return result;
                }
                else
                {
                    var error = await response.Content.ReadAsAsync<AuthonticatedUser>();
                    throw new Exception(error.error);
                }
            }
        }

        public async Task<AuthonticatedUser> RefreshAuthenticate()
        {
            string client_id = ConfigurationManager.AppSettings.Get("client_id");
            string client_secret = ConfigurationManager.AppSettings.Get("client_secret");
            var data = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("grant_type","refresh_token"),
            new KeyValuePair<string,string>("client_id",client_id),
            new KeyValuePair<string,string>("client_secret",client_secret),
            new KeyValuePair<string,string>("refresh_token",_loggedInUser.refreshToken),
            new KeyValuePair<string,string>("scope",""),
            });
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            using (HttpResponseMessage response = await httpClient.PostAsync("token", data))
            {

                if (response.IsSuccessStatusCode)
                {


                    var result = await response.Content.ReadAsAsync<AuthonticatedUser>();
                    _loggedInUser.token = result.access_token;
                    _loggedInUser.refreshToken = result.refresh_token;
                    return result;
                }
                else
                {
                    var error = await response.Content.ReadAsAsync<AuthonticatedUser>();
                    throw new Exception(error.error);
                }
            }
        }

        public async Task GetCurrentUser(string token)
        {
           
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
         
            using (HttpResponseMessage response = await httpClient.GetAsync("api/dashboard/profile"))
            {

                if (response.IsSuccessStatusCode)
                {


                    var result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);
                    var payload = json["payload"];
                    if (payload != null)
                    {
                        LoggedInUser loggedInUser = JsonConvert.DeserializeObject<LoggedInUser>(payload.ToString());
                        _loggedInUser.userId = loggedInUser.userId;
                        _loggedInUser.address = loggedInUser.address;
                        _loggedInUser.alYear = loggedInUser.alYear;
                        _loggedInUser.contact = loggedInUser.contact;
                        _loggedInUser.school = loggedInUser.school;
                        _loggedInUser.district = loggedInUser.district;
                        _loggedInUser.userName = loggedInUser.userName;
                        _loggedInUser.image = loggedInUser.image;
                        _loggedInUser.imagefullPath = loggedInUser.imagefullPath;
                        _loggedInUser.badgesPath = loggedInUser.badgesPath;
                        _loggedInUser.email = loggedInUser.email;



                    }

                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<Dashboard> GetDashboard()
        {

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_loggedInUser.token}");

            using (HttpResponseMessage response = await httpClient.GetAsync("api/dashboard"))
            {

                if (response.IsSuccessStatusCode)
                {


                    var result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);
                    var payload = json["payload"];
                    Dashboard dashboardParameters=null;
                    var objDashboardParameters = payload?["dashboardParameters"];
                    if (objDashboardParameters != null)
                        dashboardParameters = JsonConvert.DeserializeObject<Dashboard>(objDashboardParameters.ToString());

                    return dashboardParameters;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<LessonView> GetLessonSections(string Id)
        {
            
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_loggedInUser.token}");

            using (HttpResponseMessage response = await httpClient.GetAsync("api/lesson/view/"+Id))
            {

                if (response.IsSuccessStatusCode)
                {


                    var result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);
                    var payload = json["payload"];
                    LessonView lessonView = null;
                  
                    if (payload != null)
                        lessonView = JsonConvert.DeserializeObject<LessonView>(payload.ToString());

                    return lessonView;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }


        public async Task<VdoCyperOTP> GetVdoCyperOTP(string link)
        {

            vdoCyperhttpClient.DefaultRequestHeaders.Clear();

            vdoCyperhttpClient.DefaultRequestHeaders.Accept.Clear();
            vdoCyperhttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            vdoCyperhttpClient.DefaultRequestHeaders.Add("Authorization", "your Apisecret");
            List<Anotate> anotations = new List<Anotate>();

            Anotate anotate1 = new Anotate();
            anotate1.text = _loggedInUser.userName;
            anotations.Add(anotate1);
            Anotate anotate2 = new Anotate();
            anotate2.text = _loggedInUser.userName;
            anotations.Add(anotate2);
            Anotate anotate3 = new Anotate();
            anotate3.text = _loggedInUser.email;
            anotations.Add(anotate3);

            var jSonData = JsonConvert.SerializeObject(anotations);
            jSonData = jSonData.Replace("\"", "'");
            string contentCorrected = "\"annotate\":\"" + jSonData +"\"";
            string contentCorrected2 = "{\"ttl\":300," + contentCorrected + "}";
            using (HttpResponseMessage response = await vdoCyperhttpClient.PostAsync(link + "/otp", new StringContent(contentCorrected2, Encoding.UTF8, "application/json")))
            {

                if (response.IsSuccessStatusCode)
                {


                    var result = await response.Content.ReadAsAsync<VdoCyperOTP>();

                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<TopFiveUser>> GetTopFiveUSers()
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_loggedInUser.token}");

            using (HttpResponseMessage response = await httpClient.GetAsync("api/dashboard/top_five_users"))
            {

                if (response.IsSuccessStatusCode)
                {


                    var result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);
                    var payload = json["payload"];
                    List<TopFiveUser> topfiveUSers = null;

                    if (payload != null)
                        topfiveUSers = JsonConvert.DeserializeObject<List<TopFiveUser>>(payload.ToString());

                    return topfiveUSers;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<ScheduledClassView> GetScheduledClass(string Id)
        {

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_loggedInUser.token}");

            using (HttpResponseMessage response = await httpClient.GetAsync("api/scheduled/classes/view/" + Id))
            {

                if (response.IsSuccessStatusCode)
                {


                    var result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);
                    var payload = json["payload"];
                    ScheduledClassView lessonView = null;

                    if (payload != null)
                        lessonView = JsonConvert.DeserializeObject<ScheduledClassView>(payload["scheduled_class"].ToString());
                    _scheduledClassView.availableFrom = lessonView.availableFrom;
                    _scheduledClassView.availableTill = lessonView.availableTill;
                    _scheduledClassView.id = lessonView.id;
                    _scheduledClassView.title = lessonView.title;
                    _scheduledClassView.description = lessonView.description;
                    _scheduledClassView.startTime = lessonView.startTime;
                    _scheduledClassView.endTine = lessonView.endTine;
                    _scheduledClassView.scheduledClassTypeMetacode = lessonView.scheduledClassTypeMetacode;
                    _scheduledClassView.classVideos = lessonView.classVideos;
                    _scheduledClassView.classTutorials = lessonView.classTutorials;

                    return lessonView;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}

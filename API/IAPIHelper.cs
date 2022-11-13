using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appliedmaths.Models;

namespace appliedmaths.API
{
  public  interface IAPIHelper
    {
        Task<AuthonticatedUser> Authenticate(string username, string password);
        Task GetCurrentUser(string token);
        Task<Dashboard> GetDashboard();
        Task<LessonView> GetLessonSections(string Id);
        Task<VdoCyperOTP> GetVdoCyperOTP(string Id);
        Task< List<TopFiveUser>> GetTopFiveUSers();
        Task<ScheduledClassView> GetScheduledClass(string Id);
        Task<AuthonticatedUser> RefreshAuthenticate();
    }
}

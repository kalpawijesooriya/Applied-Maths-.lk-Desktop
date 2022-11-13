
using appliedmaths.EventModels;
using appliedmaths.Models;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using System.Threading.Tasks;
using appliedmaths.API;
using System.Threading;
using appliedmaths.Helpers;

namespace appliedmaths.ViewModels
{
   public class NotStartedClassViewModel : Screen
    {
        private IEventAggregator _events;

        private bool _isMenuClicked;
        private string _profileImg;
        private string _profileName;
        private IVdoCyperOTP _vdoCyperOTP;
        private IAPIHelper _apiHelper;
        private SectionVideo _video;
        private ILoggedInUser _loggedInUser;
        private string _url;
        private string _time;
        System.Windows.Forms.Timer timmer = new System.Windows.Forms.Timer();
        DateTime classStartTime;
        private IScheduledClassView _scheduledClassView;
        public NotStartedClassViewModel(IEventAggregator events, ILoggedInUser loggedInUser , IVdoCyperOTP vdoCyperOTP, IAPIHelper apiHelper, IScheduledClassView scheduledClassView)
        {
            _apiHelper = apiHelper;
            _events = events;
            _vdoCyperOTP = vdoCyperOTP;
            _loggedInUser = loggedInUser;
            _scheduledClassView = scheduledClassView;

            ShowMenuCommand = new DelegateCommand(ViewMenu);
            ProfileCommand = new DelegateCommand(ViewProfile);
            LogoutCommand = new DelegateCommand(Logout);
            BackCommand = new DelegateCommand(Back);

        }
        protected override void OnViewLoaded(object view)
        {

            ProfileImage = _loggedInUser.image;
            ProfileName = _loggedInUser.userName;
           
             _video = (SectionVideo)Application.Current.Resources["_section_video"];
             classStartTime = Convert.ToDateTime(_video.startTime);
          
            startCountDown();
            base.OnViewLoaded(view);
        }


        public string ProfileImage
        {
            get { return _profileImg; }
            set
            {
                _profileImg = value;
                NotifyOfPropertyChange(() => ProfileImage);
            }
        }
        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                NotifyOfPropertyChange(() => ProfileName);
            }
        }
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }
        public bool IsMenuClicked
        {
            get
            {
                return _isMenuClicked;
            }
            set
            {
                _isMenuClicked = value;
                NotifyOfPropertyChange(() => IsMenuVisible);


            }

        }
        public bool IsMenuVisible
        {
            get
            {
                bool output = false;
                if (IsMenuClicked)
                {
                    output = true;
                }
                return output;
            }

        }

        private void ViewMenu(object parameter)
        {
            if (IsMenuClicked)
                IsMenuClicked = false;
            else
            {
                IsMenuClicked = true;
            }
        }
        public ICommand ShowMenuCommand { get; private set; }
        public ICommand ProfileCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        private void ViewProfile(object parameter)
        {

        }
        private void Logout(object parameter)
        {
            timmer.Stop();
            _loggedInUser = null;
            _events.PublishOnUIThread(new LogoutEvent());
        }
        private void Back(object parameter)
        {
            timmer.Stop();
            Section _section = (Section)Application.Current.Resources["_section"];
           

            if ((string)Application.Current.Resources["currentClass_type"] != "SCHEDULEDCLASS")
            {
                _events.PublishOnUIThread(new SectionViewEvent(_section));
            }
            else
            {
                _events.PublishOnUIThread(new ScheduledClassViewEvent());
            }
        }

        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                NotifyOfPropertyChange(() => Time);
            }
        }
        
        private void startCountDown()
        {

            timmer.Interval = 500;
            timmer.Tick += new EventHandler(t_Tick);
            TimeSpan ts = classStartTime.Subtract(DateTime.Now);
            Time = ts.ToString("d' Days 'h' Hours 'm' Minutes 's' Seconds'");
            timmer.Start();
        }
        void t_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = classStartTime.Subtract(DateTime.Now);
            int x = (int)ts.TotalSeconds;
            if (x<=0) {
                timmer.Stop();
                _ = ShowVideoAsync();
               
                
            }

            Time = ts.ToString("d' Days 'h' Hours 'm' Minutes 's' Seconds'");
        }

        private async Task ShowVideoAsync()
        {
            var otp = await _apiHelper.GetVdoCyperOTP(_video.videoLink);
            _vdoCyperOTP.otp = otp.otp;
            _vdoCyperOTP.playbackInfo = otp.playbackInfo;
            Thread thread = new Thread(new ThreadStart(VdeoCypherThreadFunction));
            thread.Start();
            _events.PublishOnUIThread(new PlayVideoEvent());

        }

        public void VdeoCypherThreadFunction()
        {
            try
            {
                HttpServer httpServer = new HttpServer(_vdoCyperOTP);
                httpServer.Start();
            }
            catch (Exception ex)
            {

            }
        }
    }

}

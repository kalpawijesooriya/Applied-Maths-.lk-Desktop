using appliedmaths.API;
using appliedmaths.EventModels;
using appliedmaths.Helpers;
using appliedmaths.Models;
using Caliburn.Micro;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace appliedmaths.ViewModels
{
   public class SectionVideosViewModel : Screen
    {
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        private BindingList<SectionVideo> _videos;
        private BindingList<SectionTutorial> _tutorials;
        private Section _section;
        private IVdoCyperOTP _vdoCyperOTP;
        private bool _isMenuClicked;
        private string _profileImg;
        private string _profileName;
        private ILoggedInUser _loggedInUser;
        private string _discription;
        private string _duration;
        private string _title;
        private bool _isScheduledMonthly;
        private IScheduledClassView _scheduledClassView;
        string classType;
        private bool _isDownloading;
        private string _downloadStatus;
        public ICommand DownloadTutorialCommand { get; set; }

        public SectionVideosViewModel(IAPIHelper apiHelper, IEventAggregator events, IVdoCyperOTP vdoCyperOTP, ILoggedInUser loggedInUser,IScheduledClassView scheduledClassView)
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
            DownloadTutorialCommand = new RelayCommand<SectionTutorial>(DownloadTutorial);

        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            classType = (string)Application.Current.Resources["currentClass_type"];
            if (classType != "SCHEDULEDCLASS")
            {
                _section = (Section)Application.Current.Resources["_section"];
                Discriptiption = _section.description;
                Title = _section.title;
                Videos = new BindingList<SectionVideo>(_section.sectionVideos);
                Tutorials = new BindingList<SectionTutorial>(_section.sectionTutorials);
                if (_section.IsScheduledMonthlyCLass)
                {

                    foreach (SectionVideo video in _section.sectionVideos)
                    {
                        video.IsScheduledMonthlyCLass = true;


                    }
                }
            }
            else {

                Title = _scheduledClassView.title;
                Discriptiption = _scheduledClassView.description;
                foreach (SectionVideo video in _scheduledClassView.classVideos)
                {
                    video.IsScheduledMonthlyCLass = true;
                    video.startTime = _scheduledClassView.startTime;
                    video.endTime = _scheduledClassView.endTine;
                    video.title = _scheduledClassView.title;
                    video.videoLink = video.link;
                }
                Videos = new BindingList<SectionVideo>(_scheduledClassView.classVideos);
                Tutorials = new BindingList<SectionTutorial>(_scheduledClassView.classTutorials);
            }
           
            ProfileImage = _loggedInUser.image;
            ProfileName = _loggedInUser.userName;
            OTPRequestCommand = new RelayCommand<SectionVideo>(OTPRequest);
        }

        public bool IsScheduledMonthly
        {
            get
            {
                bool output = false;
                if (_isScheduledMonthly)
                {
                    output = true;
                }
                return output;
            }

        }

        public BindingList<SectionVideo> Videos
        {
            get { return _videos; }
            set
            {
                _videos = value;
                NotifyOfPropertyChange(() => Videos);
            }
        }

        public BindingList<SectionTutorial> Tutorials
        {
            get { return _tutorials; }
            set
            {
                _tutorials = value;
                NotifyOfPropertyChange(() => Tutorials);
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public string Discriptiption
        {
            get { return _discription; }
            set
            {
                _discription = value;
                NotifyOfPropertyChange(() => Discriptiption);
            }
        }

        public string Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                NotifyOfPropertyChange(() => Duration);
            }
        }

        public ICommand OTPRequestCommand { get; set; }
        private void OTPRequest(SectionVideo item)
        {
            if (classType != "SCHEDULEDCLASS")
            {
                if (_section.IsScheduledMonthlyCLass)
                {
                    DateTime startTime = Convert.ToDateTime(item.startTime);
                    DateTime EndTime = Convert.ToDateTime(item.endTime);
                    DateTime now = DateTime.Now;

                    int startDateCom = DateTime.Compare(startTime, now);
                    int endDateCom = DateTime.Compare(EndTime, now);
                    if (endDateCom < 0)
                    {
                        _events.PublishOnUIThread(new EndCLassEvent());
                    }
                    else if (startDateCom <= 0)
                    {
                        _ = ShowVideoAsync(item);
                    }
                    else if (startDateCom >= 0)
                    {
                        _events.PublishOnUIThread(new NotStartedEvent(item));
                    }

                }
                else
                {

                    _ = ShowVideoAsync(item);
                }
            }
            else {
               
                    DateTime startTime = Convert.ToDateTime(_scheduledClassView.startTime);
                    DateTime EndTime = Convert.ToDateTime(_scheduledClassView.endTine);
                    DateTime now = DateTime.Now;

                    int startDateCom = DateTime.Compare(startTime, now);
                    int endDateCom = DateTime.Compare(EndTime, now);
                    if (endDateCom < 0)
                    {
                        _events.PublishOnUIThread(new EndCLassEvent());
                    }
                    else if (startDateCom <= 0)
                    {
                        _ = ShowVideoAsync(item);
                    }
                    else if (startDateCom >= 0)
                    {
                        _events.PublishOnUIThread(new NotStartedEvent(item));
                    }

                
            }


        }

        private async Task ShowVideoAsync(SectionVideo item) {
            try {
                var otp = await _apiHelper.GetVdoCyperOTP(item.videoLink);
                _vdoCyperOTP.otp = otp.otp;
                _vdoCyperOTP.playbackInfo = otp.playbackInfo;
                Thread thread = new Thread(new ThreadStart(VdeoCypherThreadFunction));
                thread.Start();
                _events.PublishOnUIThread(new PlayVideoEvent());
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
           

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
                MessageBox.Show(ex.Message);
            }
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
            _loggedInUser = null;
            _events.PublishOnUIThread(new LogoutEvent());
        }
        private void Back(object parameter)
        {
           

            if ((string)Application.Current.Resources["currentClass_type"] != "SCHEDULEDCLASS")
            {
                _events.PublishOnUIThread(new CourseViewEvent(Application.Current.Resources["_course_id"].ToString()));
            }
            else
            {
                _events.PublishOnUIThread(new LogOnEvent());
            }
           
        }

        public bool IsDownloading
        {

            get
            {
                bool output = false;
                if (_isDownloading)
                {
                    output = true;
                }
                return output;
            }

        }
        private void DownloadTutorial(SectionTutorial item)
        {
            using (WebClient myWebClient = new WebClient())
            {
                try
                {
                    _isDownloading = true;
                    NotifyOfPropertyChange(() => IsDownloading);
                    DownloadStatus = "Downloading";
                    Thread thread = new Thread(() => {
                        createDir();
                        myWebClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                        myWebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                        myWebClient.DownloadFileAsync(new Uri(item.link), "C:\\AppliedMathsDocuments\\" + item.title + ".pdf");
                    });
                    thread.Start();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }


        private void createDir()
        {
            if (Directory.Exists("C:\\AppliedMathsDocuments"))
            {

                return;
            }
            else
            {
                Directory.CreateDirectory("C:\\AppliedMathsDocuments");
                return;
            }

        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            ProgressValue = int.Parse(Math.Truncate(percentage).ToString());
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadStatus = "Download";
            Thread.Sleep(2000);
            _isDownloading = false;
            NotifyOfPropertyChange(() => IsDownloading);
        }
        private double _progressValue;

        public double ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                NotifyOfPropertyChange(() => ProgressValue);
            }
        }
        public string DownloadStatus
        {
            get
            {
                return _downloadStatus;
            }
            set
            {
                _downloadStatus = value;
                NotifyOfPropertyChange(() => DownloadStatus);
            }
        }


    }
}

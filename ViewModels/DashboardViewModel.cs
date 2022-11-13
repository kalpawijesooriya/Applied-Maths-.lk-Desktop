using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using appliedmaths.API;
using appliedmaths.EventModels;
using appliedmaths.Models;
using Caliburn.Micro;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.PlatformUI;

namespace appliedmaths.ViewModels
{
    class DashboardViewModel : Screen
    {

        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        private BindingList<Mycourses> _mycourses;
        private BindingList<MyPapers> _myPapers;
        private BindingList<MyScheduledClasses> _myScheduledClasses;
        private BindingList<MyClasses> _myClasses;
        private BindingList<TopFiveUser> _topFiveUSers;
        private ILoggedInUser _loggedInUser;
        private Dashboard _dashboard;
        private string _purchased;
        private string _pending;
        private string _profileImg;
        private string _profileName;
        private bool _isBussy;
        private bool _isMenuClicked;
        private string _downloadStatus;
        private bool _isDownloading;
        public DashboardViewModel(IAPIHelper apiHelper, IEventAggregator events, ILoggedInUser loggedInUser)
        {
            _apiHelper = apiHelper;
            _events = events;
            _loggedInUser = loggedInUser;
            ShowMenuCommand = new DelegateCommand(ViewMenu);
            ProfileCommand = new DelegateCommand(ViewProfile);
            LogoutCommand = new DelegateCommand(Logout);
        }
        public ICommand ShowMenuCommand { get; private set; }
        public ICommand ProfileCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            IsBussy = true;
            await loadDashboardParameters();
            IsBussy = false;
        }

        private async Task loadDashboardParameters()
        {
            ProfileImage = _loggedInUser.image;
            ProfileName = _loggedInUser.userName;
           
            try {

                _dashboard = await _apiHelper.GetDashboard();
                List<TopFiveUser> temp_topfive = await _apiHelper.GetTopFiveUSers();
                TopFiveUsers = new BindingList<TopFiveUser>(temp_topfive);
                MyCourses = new BindingList<Mycourses>(_dashboard.mycourses);
                Pendding = _dashboard.no_of_pending_courses;
                Purchased = _dashboard.no_of_purchased_courses;

                setMyScheduledClasses();
                setMyPapers();

                ViewCoursesCommand = new RelayCommand<Mycourses>(ViewCourses);
                MarkingSchemeCommand = new RelayCommand<MyPapers>(DownloadMarkingScheme);
                PaperCommand = new RelayCommand<MyPapers>(DownloadPaper);
            } catch (Exception ex) {
                if (ex.Message == "Unauthorized")
                {
                    await _apiHelper.RefreshAuthenticate();
                    await loadDashboardParameters();

                }
            }
           



        }
        public BindingList<TopFiveUser> TopFiveUsers
        {
            get { return _topFiveUSers; }
            set
            {
                _topFiveUSers = value;
                NotifyOfPropertyChange(() => TopFiveUsers);
            }
        }
        public BindingList<Mycourses> MyCourses
        {
            get { return _mycourses; }
            set
            {
                _mycourses = value;
                NotifyOfPropertyChange(() => MyCourses);
            }
        }

        public BindingList<MyPapers> MyPapers
        {
            get { return _myPapers; }
            set
            {
                _myPapers = value;
                NotifyOfPropertyChange(() => MyPapers);
            }
        }
        public BindingList<MyClasses> MyClasses
        {
            get { return _myClasses; }
            set
            {
                _myClasses = value;
                NotifyOfPropertyChange(() => MyClasses);
            }
        }

        public string Purchased
        {
            get { return _purchased; }
            set
            {
                _purchased = value;
                NotifyOfPropertyChange(() => Purchased);
            }
        }
        public string Pendding
        {
            get { return _pending; }
            set
            {
                _pending = value;
                NotifyOfPropertyChange(() => Pendding);
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
        
        public BindingList<MyScheduledClasses> MyScheduledClasses
        {
            get { return _myScheduledClasses; }
            set
            {
                _myScheduledClasses = value;
                NotifyOfPropertyChange(() => MyScheduledClasses);
            }
        }
        public bool IsBussy
        {
            get
            {
                return _isBussy;
            }
            set
            {
                _isBussy = value;
                NotifyOfPropertyChange(() => IsLoadingVisible);
              

            }

        }

        public bool IsLoadingVisible
        {
            get
            {
                bool output = false;
                if (IsBussy)
                {
                    output = true;
                }
                return output;
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
        
        private void setMyScheduledClasses()
        {
            List<MyClasses> newMyClasses = new List<MyClasses>();
            foreach (var myClass in _dashboard.myClasses)
            {
                string[] startDateList = myClass.startTime.date.Split(".");
                myClass.startTimeString = startDateList[0];
                string[] endDateList = myClass.endTime.date.Split(".");
                myClass.endTimeString = endDateList[0];
                newMyClasses.Add(myClass);

            }
            _dashboard.myClasses = newMyClasses;
            MyClasses = new BindingList<MyClasses>(_dashboard.myClasses);

            var myClasses = _dashboard.myClasses;
            var myMonthlyClasses = _dashboard.myMonthlyClasses;

            List<MyScheduledClasses> myScheduledClassList = new List<MyScheduledClasses>();
            foreach (var myclass in myClasses)
            {
                MyScheduledClasses newMyScheduledClass = new MyScheduledClasses();
                newMyScheduledClass.id = myclass.id;
                newMyScheduledClass.startTime = myclass.startTimeString;
                newMyScheduledClass.endTime = myclass.endTimeString;
                newMyScheduledClass.title = myclass.title;
                newMyScheduledClass.metacode = myclass.metacode;
                newMyScheduledClass.IsScheduledCLass = true;
                newMyScheduledClass.scheduledClassid = myclass.scheduledClassid;
                myScheduledClassList.Add(newMyScheduledClass);


            };
            foreach (var myMonthlyClass in myMonthlyClasses)
            {
                MyScheduledClasses newMyScheduledClass = new MyScheduledClasses();
                newMyScheduledClass.id = myMonthlyClass.id;
                newMyScheduledClass.expiresOn = myMonthlyClass.expiresOn;
                newMyScheduledClass.videoCount = myMonthlyClass.videoCount;
                newMyScheduledClass.title = myMonthlyClass.title;
                newMyScheduledClass.cluster_type = myMonthlyClass.cluster_type;
                newMyScheduledClass.IsMonthlyClass = true;
                myScheduledClassList.Add(newMyScheduledClass);

            };
            MyScheduledClasses = new BindingList<MyScheduledClasses>(myScheduledClassList);
            ViewSheduledClassCommand = new RelayCommand<MyScheduledClasses>(ViewSheduledClass);
        }
        private void setMyPapers()
        {
            List<MyPapers> newMyPapers = new List<MyPapers>();
            foreach (var paper in _dashboard.myPapers)
            {
                string[] dateList = paper.expiresOn.date.Split(".");
                paper.expiresOnString = dateList[0];
                newMyPapers.Add(paper);
            }
            _dashboard.myPapers = newMyPapers;
            MyPapers = new BindingList<MyPapers>(_dashboard.myPapers);
        }

        public ICommand ViewSheduledClassCommand { get; set; }
        private async void ViewSheduledClass(MyScheduledClasses item)
        {
            if (item.IsMonthlyClass)
            {
                if (item.cluster_type== "SCHEDULED_MONTHLY_CLASS") {
                    _events.PublishOnUIThread(new CourseViewEvent(item.id));
                }
                else if (item.cluster_type == "SCHEDULED_MONTHLY_MCQ")
                {
                    string baseUrl = ConfigurationManager.AppSettings.Get("BASE_URL");
                    Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", baseUrl + @"lesson/view/" + item.id);
                }
                else if (item.cluster_type == "MONTHLY_CLASS") {
                    _events.PublishOnUIThread(new CourseViewEvent(item.id));
                }


            }
            if (item.IsScheduledCLass)
            {
                if (item.metacode == "PAPER")
                {
                    string baseUrl = ConfigurationManager.AppSettings.Get("BASE_URL");
                    Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", baseUrl + @"scheduled/classes/view/" + item.scheduledClassid);
                }
                else if (item.metacode == "VIDEO")
                {
                    try {
                        _ = await _apiHelper.GetScheduledClass(item.scheduledClassid);
                        _events.PublishOnUIThread(new ScheduledClassViewEvent());
                    }
                    catch (Exception ex) {
                        if (ex.Message == "Unauthorized")
                        {
                            await _apiHelper.RefreshAuthenticate();
                            _ = await _apiHelper.GetScheduledClass(item.scheduledClassid);
                            _events.PublishOnUIThread(new ScheduledClassViewEvent());

                        }
                       
                    }
                   
                   
                }
                
            }
        }

        public ICommand ViewCoursesCommand { get; set; }
        public ICommand MarkingSchemeCommand { get; set; }
        public ICommand PaperCommand { get; set; }
        private void ViewCourses(Mycourses item)
        {
            _events.PublishOnUIThread(new CourseViewEvent(item.id));
        }
        private void DownloadPaper(MyPapers item)
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
                        myWebClient.DownloadFileAsync(new Uri(item.paperLink), "C:\\AppliedMathsDocuments\\" + item.title + "_paper" + ".pdf");
                    });
                    thread.Start();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
        private void DownloadMarkingScheme(MyPapers item)
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
                        myWebClient.DownloadFileAsync(new Uri(item.answerLink), "C:\\AppliedMathsDocuments\\" + item.title + "_markingScheme" + ".pdf");
                    });
                    thread.Start();

                } catch(Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void createDir() {
            if (Directory.Exists("C:\\AppliedMathsDocuments"))
            {

                return;
            }
            else {
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
        private void ViewMenu(object parameter)
        {
            if (IsMenuClicked)
                IsMenuClicked = false;
            else {
                IsMenuClicked = true;
            }
        }
        //Show profile Function
        private void ViewProfile(object parameter)
        {
           
        }
        //Logout Fuction
        private void Logout(object parameter)
        {
            _loggedInUser = null;
            _events.PublishOnUIThread(new LogoutEvent());
        }
    }
}

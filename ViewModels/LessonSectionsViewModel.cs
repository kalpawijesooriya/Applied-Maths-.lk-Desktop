using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    class LessonSectionsViewModel : Screen
    {
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;
        private LessonView lessonView;
        private BindingList<Section> _sections;
        private BindingList<Tutorial> _tutorials;
        private string _course_id;
        private string _discription;
        private string _duration;
        private string _title;
        private bool _isMenuClicked;
        private string _profileImg;
        private string _profileName;
        private ILoggedInUser _loggedInUser;
        private bool _isBussy;
        private bool _isDownloading;
        private string _downloadStatus;
        public ICommand ShowMenuCommand { get; private set; }
        public ICommand ProfileCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand DownloadTutorialCommand { get; set; }

        public LessonSectionsViewModel(IAPIHelper apiHelper, IEventAggregator events, ILoggedInUser loggedInUser)
        {
            _apiHelper = apiHelper;
            _events = events;
            _loggedInUser = loggedInUser;
           
            ShowMenuCommand = new DelegateCommand(ViewMenu);
            ProfileCommand = new DelegateCommand(ViewProfile);
            LogoutCommand = new DelegateCommand(Logout);
            BackCommand=new DelegateCommand(Back);
            DownloadTutorialCommand = new RelayCommand<Tutorial>(DownloadTutorial);
        }

      
        protected override async void OnViewLoaded(object view)
        {
            IsBussy = true;
            ProfileImage = _loggedInUser.image;
            ProfileName = _loggedInUser.userName;
            base.OnViewLoaded(view);
            await loadCourseSection();
            IsBussy = false;
        }
      
        private async Task loadCourseSection()
        {
            try
            {
                _course_id = Application.Current.Resources["_course_id"].ToString();
                lessonView = await _apiHelper.GetLessonSections(_course_id);
                Sections = new BindingList<Section>(lessonView.lesson.sections);
                Tutorials = new BindingList<Tutorial>(lessonView.lesson.tutorials);
                Title = lessonView.lesson.title;
                Discriptiption = lessonView.lesson.description;
                Duration = lessonView.lesson.duration;
                ViewSectionVideosCommand = new RelayCommand<Section>(ViewSectionVideos);

            }
            catch (Exception ex){
                if (ex.Message == "Unauthorized") {
                    await _apiHelper.RefreshAuthenticate();
                    await loadCourseSection();

                }
            
            }
            
        }

        public BindingList<Section> Sections
        {
            get { return _sections; }
            set
            {
                _sections = value;
                NotifyOfPropertyChange(() => Sections);
            }
        }
        public BindingList<Tutorial> Tutorials
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
        public ICommand ViewSectionVideosCommand { get; set; }
        private void ViewSectionVideos(Section item)
        {
            if (lessonView.lesson.course_cluster_type_metacode == "SCHEDULED_MONTHLY_CLASS")
            {
                item.IsScheduledMonthlyCLass =true;
                _events.PublishOnUIThread(new SectionViewEvent(item));
            }
            else {
                _events.PublishOnUIThread(new SectionViewEvent(item));
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
            _events.PublishOnUIThread(new LogOnEvent());
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
        private void DownloadTutorial(Tutorial item)
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
                        myWebClient.DownloadFileAsync(new Uri(item.link), "C:\\AppliedMathsDocuments\\" + item.title  + ".pdf");
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

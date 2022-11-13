using appliedmaths.API;
using appliedmaths.EventModels;
using appliedmaths.Helpers;
using appliedmaths.Models;
using Caliburn.Micro;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace appliedmaths.ViewModels
{
    class VideoPlayerViewModel :Screen
    {
    
        private IEventAggregator _events;
      
        private bool _isMenuClicked;
        private string _profileImg;
        private string _profileName;

        private ILoggedInUser _loggedInUser;
        private string _url;
        public VideoPlayerViewModel( IEventAggregator events ,ILoggedInUser loggedInUser)
        {
            _events = events;
            _loggedInUser = loggedInUser;

            ShowMenuCommand = new DelegateCommand(ViewMenu);
            ProfileCommand = new DelegateCommand(ViewProfile);
            LogoutCommand = new DelegateCommand(Logout);
            BackCommand = new DelegateCommand(Back);

        }
        protected override  void OnViewLoaded(object view)
        {
         
            ProfileImage = _loggedInUser.image;
            ProfileName = _loggedInUser.userName;
            Url = Application.Current.Resources["internal_server_url"].ToString() +"video" ;
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
            Url = "http://localhost:8001/shutdown";
            _loggedInUser = null;
            Thread.Sleep(3000);
            _events.PublishOnUIThread(new LogoutEvent());
        }
        private void Back(object parameter)
        {
           
            Url = Application.Current.Resources["internal_server_url"].ToString() + "shutdown";
            Thread.Sleep(3000);
            Section _section = (Section)Application.Current.Resources["_section"];

            if ((string)Application.Current.Resources["currentClass_type"] != "SCHEDULEDCLASS")
            {
                _events.PublishOnUIThread(new SectionViewEvent(_section));
            }
            else {
                _events.PublishOnUIThread(new ScheduledClassViewEvent());
            }
            
        }


    }
}

using appliedmaths.EventModels;
using appliedmaths.Models;
using Caliburn.Micro;
using Microsoft.VisualStudio.PlatformUI;
using System.Windows;
using System.Windows.Input;

namespace appliedmaths.ViewModels
{
   public class EndClassViewModel: Screen
    {
        private IEventAggregator _events;

        private bool _isMenuClicked;
        private string _profileImg;
        private string _profileName;

        private ILoggedInUser _loggedInUser;
        private string _url;

        private string _html;
        public EndClassViewModel(IEventAggregator events, ILoggedInUser loggedInUser)
        {
            _events = events;
            _loggedInUser = loggedInUser;

            ShowMenuCommand = new DelegateCommand(ViewMenu);
            ProfileCommand = new DelegateCommand(ViewProfile);
            LogoutCommand = new DelegateCommand(Logout);
            BackCommand = new DelegateCommand(Back);

        }
        protected override void OnViewLoaded(object view)
        {

            ProfileImage = _loggedInUser.image;
            ProfileName = _loggedInUser.userName;
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
            _loggedInUser = null;
            _events.PublishOnUIThread(new LogoutEvent());
        }
        private void Back(object parameter)
        {

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
    }
}

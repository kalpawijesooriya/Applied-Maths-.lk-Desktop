using appliedmaths.Helpers;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using appliedmaths.API;
using appliedmaths.EventModels;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.PlatformUI;
using System.Configuration;

namespace appliedmaths.ViewModels
{
    public class LoginViewModel : Screen
     {
        private string _email;
        private string _password;
        private bool _isBussy;
        private IAPIHelper _apiHelper;
        private string _errorMessage;
        private IEventAggregator _events;
        private string _url;

        public LoginViewModel(IAPIHelper apiHelper,IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
            ForgotPasswordString = "";
            HyperlinkCommand = new DelegateCommand(OnHyperlink);
        }

        public ICommand HyperlinkCommand { get; private set; }
        public string Email
        {
           get
            {
                return _email;
            }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
                NotifyOfPropertyChange(() => CanLogIn);

            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
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
                NotifyOfPropertyChange(() => CanLogIn);
                
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
        public bool IsErrorVisible
        {
            get
            {
                bool output = false;
                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output;
            }

        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(()=> ErrorMessage);
                
            }
        }

        public string ForgotPasswordString
        {
            get
            {
                return _url;
            }
            set
            {
                _url = value;
                NotifyOfPropertyChange(() => ForgotPasswordString);

            }
        }

        public bool CanLogIn
        {
            get {
                bool output = false;
                if (Email?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }
                if (IsBussy)
                {
                    output = false;
                }

                return output;

            }
           

        }

        public async Task LogIn()
        {
            try
            {
                IsBussy = true;
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(Email, Password);
               //getUserInfo
                await _apiHelper.GetCurrentUser(result.access_token);
                IsBussy = false;
                _events.PublishOnUIThread(new LogOnEvent());
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                IsBussy = false;
            }
           
        }
       
        private void OnHyperlink(object parameter)
        {
            string baseUrl = ConfigurationManager.AppSettings.Get("BASE_URL");
            Process.Start(@"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe", baseUrl + @"login");

        }



    }
}

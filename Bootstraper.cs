using appliedmaths.Helpers;
using appliedmaths.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using appliedmaths.API;
using appliedmaths.Models;

namespace appliedmaths
{
    public class Bootstraper :BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();
        public Bootstraper() 
        {
            Initialize();
            ConventionManager.AddElementConvention<PasswordBox>(
           PasswordBoxHelper.BoundPasswordProperty,
           "Password",
           "PasswordChanged");
        }

        protected override void Configure()
        {
            _container.Instance(_container);

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IAPIHelper, APIHelper>()
                .Singleton<IAPIClient, APIClient>()
                .Singleton<ILoggedInUser, LoggedInUser>()
                .Singleton<IVdoCyperOTP, VdoCyperOTP>()
                .Singleton<IScheduledClassView, ScheduledClassView>();


            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType,viewModelType.ToString(),viewModelType));

        }

        private void Singleton<T1, T2>()
        {
            throw new NotImplementedException();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
          
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances( Type service) 
        { 
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

    }
}

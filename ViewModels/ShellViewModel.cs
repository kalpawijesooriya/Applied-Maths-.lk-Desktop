using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using appliedmaths.EventModels;
using System.Windows;

namespace appliedmaths.ViewModels
{
    class ShellViewModel :Conductor<object>,IHandle<LogOnEvent>,IHandle<CourseViewEvent>,IHandle<SectionViewEvent>,
        IHandle<PlayVideoEvent>,IHandle<LogoutEvent>, IHandle<EndCLassEvent>, IHandle<NotStartedEvent>,IHandle<ScheduledClassViewEvent>
    {
        LoginViewModel _loginVM;
        private IEventAggregator _events;
        private DashboardViewModel _dashboardVM;
        private LessonSectionsViewModel _sectionsVM;
        private SectionVideosViewModel _sectionVideosVM;
        private VideoPlayerViewModel _videoPlayerVM;
        private SimpleContainer _container;
        private EndClassViewModel _endClassViewModel;
        private NotStartedClassViewModel _notStartedClassViewModel;

        public ShellViewModel(LoginViewModel loginVM, LessonSectionsViewModel sectionsVM, IEventAggregator events,
            DashboardViewModel dashboardVM, SectionVideosViewModel sectionVideosVM, VideoPlayerViewModel videoPlayerVM, 
            EndClassViewModel endClassViewModel, NotStartedClassViewModel notStartedClassViewModel, 
            SimpleContainer container)
        {
            _events = events;
            _loginVM = loginVM;
            _sectionsVM = sectionsVM;
            _dashboardVM = dashboardVM;
            _container = container;
            _sectionVideosVM = sectionVideosVM;
            _videoPlayerVM = videoPlayerVM;
            _events.Subscribe(this);
            _endClassViewModel = endClassViewModel;
            _notStartedClassViewModel = notStartedClassViewModel;
            ActivateItem(_container.GetInstance<LoginViewModel>());
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_dashboardVM); 
        }

        public void Handle(CourseViewEvent message)
        {
            Application.Current.Resources["_course_id"] = message.Id;
            ActivateItem(_sectionsVM);
            
        }
        public void Handle(SectionViewEvent message)
        {
            Application.Current.Resources["currentClass_type"] = "COURSE";
            Application.Current.Resources["_section"] = message.Item;
            ActivateItem(_sectionVideosVM);

        }

        public void Handle(PlayVideoEvent message)
        {
            ActivateItem(_videoPlayerVM);
        }
        public void Handle(LogoutEvent message)
        {
            ActivateItem(_loginVM);
        }

        public void Handle(EndCLassEvent message)
        {
            ActivateItem(_endClassViewModel);
        }

        public void Handle(NotStartedEvent message)
        {
            Application.Current.Resources["currentClass_type"] = "COURSE";
            Application.Current.Resources["_section_video"] = message.Item;
            ActivateItem(_notStartedClassViewModel);
        }
        public void Handle(ScheduledClassViewEvent message)
        {
            Application.Current.Resources["currentClass_type"] ="SCHEDULEDCLASS";
            ActivateItem(_sectionVideosVM);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace appliedmaths.Views
{
    /// <summary>
    /// Interaction logic for SectionVideosView.xaml
    /// </summary>
    public partial class SectionVideosView : UserControl
    {
        public SectionVideosView()
        {
            InitializeComponent();
            this.Height = SystemParameters.PrimaryScreenHeight * 0.95;
         
        }
    }
}

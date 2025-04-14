using System.Windows;
using PrivateDoctorsApp.ViewModel.Admin;

namespace PrivateDoctorsApp.View.Admin
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}

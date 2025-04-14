using System.Windows;
using PrivateDoctorsApp.ViewModel.Doctor;

namespace PrivateDoctorsApp.View.Doctor
{
    /// <summary>
    /// Interaction logic for NotificationsWindow.xaml
    /// </summary>
    public partial class NotificationsWindow : Window
    {
        public NotificationsWindow()
        {
            InitializeComponent();
            DataContext = new NotificationsViewModel();
        }
    }
}

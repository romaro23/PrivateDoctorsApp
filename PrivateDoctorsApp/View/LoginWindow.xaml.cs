using System.Windows;
using PrivateDoctorsApp.ViewModel;

namespace PrivateDoctorsApp.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }
    }
}

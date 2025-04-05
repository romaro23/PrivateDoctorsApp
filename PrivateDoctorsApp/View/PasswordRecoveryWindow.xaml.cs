using System.Windows;
using PrivateDoctorsApp.ViewModel;

namespace PrivateDoctorsApp.View
{
    /// <summary>
    /// Interaction logic for PasswordRecoveryWindow.xaml
    /// </summary>
    public partial class PasswordRecoveryWindow : Window
    {
        public PasswordRecoveryWindow()
        {
            InitializeComponent();
            DataContext = new PasswordRecoveryViewModel(this);
        }
    }
}

using System.Windows;
using PrivateDoctorsApp.ViewModel;

namespace PrivateDoctorsApp.View.Admin
{
    /// <summary>
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
            DataContext = new AdminMainViewModel(MainFrame);
        }
    }
}

using System.Windows.Controls;
using PrivateDoctorsApp.ViewModel.Admin;

namespace PrivateDoctorsApp.View.Admin
{
    /// <summary>
    /// Interaction logic for AdminServicesPage.xaml
    /// </summary>
    public partial class AdminServicesPage : Page
    {
        public AdminServicesPage()
        {
            InitializeComponent();
            DataContext = new AdminServicesViewModel();
        }
    }
}

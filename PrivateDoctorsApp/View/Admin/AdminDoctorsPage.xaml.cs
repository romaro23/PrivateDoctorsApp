using System.Windows.Controls;
using PrivateDoctorsApp.ViewModel.Admin;

namespace PrivateDoctorsApp.View.Admin
{
    /// <summary>
    /// Interaction logic for AdminDoctorsPage.xaml
    /// </summary>
    public partial class AdminDoctorsPage : Page
    {
        public AdminDoctorsPage()
        {
            InitializeComponent();
            DataContext = new AdminDoctorsViewModel();
        }
    }
}

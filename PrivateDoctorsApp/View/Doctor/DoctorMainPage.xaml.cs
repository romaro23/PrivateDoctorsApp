using System.Windows.Controls;
using PrivateDoctorsApp.ViewModel.Doctor;

namespace PrivateDoctorsApp.View.Doctor
{
    /// <summary>
    /// Interaction logic for DoctorMainPage.xaml
    /// </summary>
    public partial class DoctorMainPage : Page
    {
        public DoctorMainPage()
        {
            InitializeComponent();
            DataContext = new DoctorMainPageViewModel();
        }
    }
}

using System.Windows.Controls;
using PrivateDoctorsApp.ViewModel.Doctor;

namespace PrivateDoctorsApp.View.Doctor
{
    /// <summary>
    /// Interaction logic for DoctorPersonalPage.xaml
    /// </summary>
    public partial class DoctorAppointmentsPage : Page
    {
        public DoctorAppointmentsPage()
        {
            InitializeComponent();
            DataContext = new DoctorAppointmentsViewModel();
        }
    }
}

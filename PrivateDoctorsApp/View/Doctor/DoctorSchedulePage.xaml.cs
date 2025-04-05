using System.Windows.Controls;
using PrivateDoctorsApp.ViewModel;

namespace PrivateDoctorsApp.View.Doctor
{
    public partial class DoctorSchedulePage : Page
    {
        public DoctorSchedulePage()
        {
            InitializeComponent();
            DataContext = new DoctorScheduleViewModel();
        }
    }
}

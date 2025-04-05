using System.Windows;
using PrivateDoctorsApp.ViewModel;

namespace PrivateDoctorsApp.View.Doctor
{
    public partial class DoctorMainWindow : Window
    {
        public DoctorMainWindow()
        {
            InitializeComponent();
            DataContext = new DoctorMainViewModel(MainFrame);
        }
    }
}

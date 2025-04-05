using System.Windows;
using PrivateDoctorsApp.ViewModel;

namespace PrivateDoctorsApp.View.Patient
{
    /// <summary>
    /// Interaction logic for PatientMainWindow.xaml
    /// </summary>
    public partial class PatientMainWindow : Window
    {
        public PatientMainWindow()
        {
            InitializeComponent();
            DataContext = new PatientMainViewModel(MainFrame);
        }
    }
}

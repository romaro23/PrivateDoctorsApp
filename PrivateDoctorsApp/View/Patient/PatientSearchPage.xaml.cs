using System.Windows.Controls;
using PrivateDoctorsApp.ViewModel.Patient;

namespace PrivateDoctorsApp.View.Patient    
{
    /// <summary>
    /// Interaction logic for PatientSearchPage.xaml
    /// </summary>
    public partial class PatientSearchPage : Page
    {
        public PatientSearchPage()
        {
            InitializeComponent();
            DataContext = new PatientSearchViewModel();
        }
    }
}

using System.Windows.Controls;
using PrivateDoctorsApp.ViewModel.Admin;

namespace PrivateDoctorsApp.View.Admin
{
    /// <summary>
    /// Interaction logic for AdminAccountancyPage.xaml
    /// </summary>
    public partial class AdminAccountancyPage : Page
    {
        public AdminAccountancyPage()
        {
            InitializeComponent();
            DataContext = new AdminAccountancyViewModel(TabControl);
        }
    }
}

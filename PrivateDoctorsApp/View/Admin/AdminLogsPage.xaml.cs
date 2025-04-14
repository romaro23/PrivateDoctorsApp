using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrivateDoctorsApp.View.Admin
{
    /// <summary>
    /// Interaction logic for AdminLogsPage.xaml
    /// </summary>
    public partial class AdminLogsPage : Page
    {
        public AdminLogsPage()
        {
            InitializeComponent();
            DataContext = new ViewModel.Admin.AdminLogsViewModel();
        }
    }
}

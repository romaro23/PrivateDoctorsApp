using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrivateDoctorsApp.View;
using PrivateDoctorsApp.View.Admin;

namespace PrivateDoctorsApp.ViewModel
{
    internal class AdminMainViewModel
    {
        private Frame _frame;
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public AdminMainViewModel(Frame frame)
        {
            LogoutCommand = new RelayCommand(ExecuteLogout);
            NavigateCommand = new RelayCommand(ExecuteNavigate);
            _frame = frame;
            _frame.Navigate(new AdminMainPage());
        }

        private void ExecuteNavigate(object parameter)
        {
            string page = parameter as string;
            if (page != null)
            {
                if (page == "Main")
                {
                    _frame.Navigate(new AdminMainPage());
                }

                if (page == "Appointments")
                {
                    _frame.Navigate(new AdminAppointmentsPage());
                }

                if (page == "Schedules")
                {
                    _frame.Navigate(new AdminSchedulesPage());
                }

                if (page == "Accountancy")
                {
                    _frame.Navigate(new AdminAccountancyPage());
                }

                if (page == "Doctors")
                {
                    _frame.Navigate(new AdminDoctorsPage());
                }

                if (page == "Services")
                {
                    _frame.Navigate(new AdminServicesPage());
                }
            }
        }
        private void ExecuteLogout(object parameter)
        {
            LoginWindow login = new LoginWindow();
            Application.Current.MainWindow.Close();
            Application.Current.MainWindow = login;
            login.Show();
        }   
    }
}

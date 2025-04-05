using System.Windows;
using PrivateDoctorsApp.View;
using System.Windows.Controls;
using System.Windows.Input;
using PrivateDoctorsApp.View.Doctor;

namespace PrivateDoctorsApp.ViewModel
{
    internal class DoctorMainViewModel
    {
        private Frame _frame;
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public DoctorMainViewModel(Frame frame)
        {
            LogoutCommand = new RelayCommand(ExecuteLogout);
            NavigateCommand = new RelayCommand(ExecuteNavigate);
            _frame = frame;
            _frame.Navigate(new DoctorMainPage());
        }

        private void ExecuteNavigate(object parameter)
        {
            string page = parameter as string;
            if (page != null)
            {
                if (page == "Main")
                {
                    _frame.Navigate(new DoctorMainPage());
                }

                if (page == "Appointments")
                {
                    _frame.Navigate(new DoctorAppointmentsPage());
                }

                if (page == "Schedule")
                {
                    _frame.Navigate(new DoctorSchedulePage());
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

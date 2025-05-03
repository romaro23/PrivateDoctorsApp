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
        public ICommand SettingsCommand { get; }
        public ICommand BackupCommand { get; }

        public AdminMainViewModel(Frame frame)
        {
            LogoutCommand = new RelayCommand(ExecuteLogout);
            NavigateCommand = new RelayCommand(ExecuteNavigate);
            SettingsCommand = new RelayCommand(ExecuteOpenSettings);
            BackupCommand = new RelayCommand(ExecuteOpenBackup);
            _frame = frame;
            _frame.Navigate(new AdminAccountancyPage());
        }

        private void ExecuteOpenSettings(object parameter)
        {
            SettingsWindow window = new SettingsWindow();
            window.Show();
        }

        private void ExecuteOpenBackup(object parameter)
        {
            BackupWindow window = new BackupWindow();
            window.Show();
        }
        private void ExecuteNavigate(object parameter)
        {
            string page = parameter as string;
            if (page != null)
            {
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

                if (page == "Logs")
                {
                    _frame.Navigate(new AdminLogsPage());
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

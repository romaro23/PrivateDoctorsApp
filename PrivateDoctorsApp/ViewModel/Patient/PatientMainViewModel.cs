using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrivateDoctorsApp.View;
using PrivateDoctorsApp.View.Patient;

namespace PrivateDoctorsApp.ViewModel
{
    internal class PatientMainViewModel
    {
        private Frame _frame;
        public ICommand NavigateCommand { get;  }
        public ICommand LogoutCommand { get; }

        public PatientMainViewModel(Frame frame)
        {
            LogoutCommand = new RelayCommand(ExecuteLogout);
            NavigateCommand = new RelayCommand(ExecuteNavigate);
            _frame = frame;
            _frame.Navigate(new PatientMainPage());
        }

        private void ExecuteNavigate(object parameter)
        {
            string page = parameter as string;
            if (page != null)
            {
                if (page == "Main")
                {
                    _frame.Navigate(new PatientMainPage());
                }

                if (page == "Personal")
                {
                    _frame.Navigate(new PatientPersonalPage());
                }

                if (page == "Search")
                {
                    _frame.Navigate(new PatientSearchPage());
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

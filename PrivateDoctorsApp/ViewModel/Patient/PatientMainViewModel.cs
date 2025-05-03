using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View;
using PrivateDoctorsApp.View.Patient;
using System.Linq;
using PrivateDoctorsApp.ViewModel.Patient;

namespace PrivateDoctorsApp.ViewModel
{
    internal class PatientMainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private Frame _frame;
        public ICommand NavigateCommand { get;  }
        public ICommand LogoutCommand { get; }

        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            set
            {
                if (value == _balance) return;
                _balance = value;
                OnPropertyChanged(nameof(Balance));
            }
        }
        public PatientMainViewModel(Frame frame)
        {
            LogoutCommand = new RelayCommand(ExecuteLogout);
            NavigateCommand = new RelayCommand(ExecuteNavigate);
            _frame = frame;
            var mainPage = new PatientMainPage();
            var patientMainPageViewModel = new PatientMainPageViewModel();
            mainPage.DataContext = patientMainPageViewModel;
            patientMainPageViewModel.DataUpdated += () => LoadBalance();
            _frame.Navigate(mainPage);
            LoadBalance();
        }

        public void LoadBalance()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var patient = context.Patients.FirstOrDefault(p => p.ID == CurrentUser.ID);
                        if (patient != null)
                        {
                            Balance = patient.AccountBalance ?? 0;
                            CurrentUser.Balance = Balance;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                Balance = 0;
            }
        }
        private void ExecuteNavigate(object parameter)
        {
            string page = parameter as string;
            if (page != null)
            {
                if (page == "Main")
                {
                    var mainPage = new PatientMainPage();
                    var patientMainPageViewModel = new PatientMainPageViewModel();
                    mainPage.DataContext = patientMainPageViewModel;
                    patientMainPageViewModel.DataUpdated += () => LoadBalance();
                    _frame.Navigate(mainPage);
                }

                if (page == "Search")
                {
                    var searchPage = new PatientSearchPage();
                    var patientSearchPageViewModel = new PatientSearchViewModel();
                    searchPage.DataContext = patientSearchPageViewModel;
                    patientSearchPageViewModel.DataUpdated += () => LoadBalance();
                    _frame.Navigate(searchPage);
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

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;


namespace PrivateDoctorsApp.ViewModel.Doctor
{
    internal class NotificationsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DoctorMainPageViewModel.Notification> Notifications { get; set; }
        public ObservableCollection<string> Patients { get; set; }
        public ICommand SelectedItemChangedCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        private string _patientName;
        public string PatientName
        {
            get => _patientName;
            set
            {
                _patientName = value;
                OnPropertyChanged(nameof(PatientName));
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand ClearCommand { get; }
        public NotificationsViewModel()
        {
            LoadNotifications();
            SelectedItemChangedCommand = new RelayCommand(ExecuteSelectedItemChanged);
            ClearCommand = new RelayCommand(ExecuteClear);
        }
        private void ExecuteClear(object parameter)
        {
            PatientName = null;

        }
        private void ExecuteSelectedItemChanged(object parameter)
        {
            LoadNotifications(PatientName);
        }

        private void LoadNotifications(string patientName = null)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Notifications = new ObservableCollection<DoctorMainPageViewModel.Notification>(
                            (from n in context.Notifications
                                join p in context.Patients on n.PatientID equals p.ID
                                where n.DoctorID == CurrentUser.ID && (n.Type == "appointment" || n.Type == "cancellation") && (string.IsNullOrEmpty(patientName) || p.LastName + " " + p.FirstName == patientName)
                                orderby n.DateOfCreation
                                select new DoctorMainPageViewModel.Notification
                                {
                                    Date = n.DateOfCreation,
                                    Description = n.Description,
                                    PatientName = p.LastName + " " + p.FirstName,
                                })
                            .ToList()
                        );
                        Patients = new ObservableCollection<string>(Notifications.Select(n => n.PatientName).Distinct().ToList());
                        OnPropertyChanged(nameof(Notifications));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

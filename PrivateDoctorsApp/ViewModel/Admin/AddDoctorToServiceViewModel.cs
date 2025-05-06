using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class AddDoctorToServiceViewModel : LogEventBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<int?> _ids;

        public ObservableCollection<int?> IDs
        {
            get => _ids;
            set
            {
                if (Equals(value, _ids)) return;
                _ids = value;
                OnPropertyChanged(nameof(IDs));
            }
        }

        private int? _id;

        public int? ID
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        private AdminServicesViewModel.ServiceItem _service;
        public ICommand AddDoctorToServiceCommand { get; }
        private bool CanAdd()
        {
            return ID.HasValue;
        }
        public AddDoctorToServiceViewModel(AdminServicesViewModel.ServiceItem service)
        {
            _service = service;
            AddDoctorToServiceCommand = new RelayCommand(ExecuteAddDoctorToService, CanAdd);
            LoadIds();
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
            
        }

        private void LoadIds()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        IDs = new ObservableCollection<int?>(context.Employees.Select(e => (int?)e.ID).ToList());

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ExecuteAddDoctorToService(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var doctorService = new Model.DoctorService
                        {
                            DoctorID = ID,
                            ServiceID = _service.ID
                        };
                        context.DoctorServices.Add(doctorService);
                        OnLogEvent("Додано послугу до лікаря", "DoctorServices");
                        context.SaveChanges();
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

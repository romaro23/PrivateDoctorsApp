using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;

namespace PrivateDoctorsApp.ViewModel.Doctor
{
    internal class DoctorAppointmentsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<AppointmentItem> _appointments;
        public ObservableCollection<AppointmentItem> Appointments
        {
            get { return _appointments; }
            set
            {
                _appointments = value;
                OnPropertyChanged(nameof(Appointments));
            }
        }
        public class AppointmentItem
        {
            public string PatientName { get; set; }
            public string ServiceName { get; set; }
            public string Status { get; set; }
            public DateTime? Date { get; set; }
            public string FormattedDate
            {
                get
                {
                    return Date.HasValue ? Date.Value.ToString("d MMMM", new CultureInfo("uk-UA")) : "";
                }
            }
        }
        private ObservableCollection<string> _patients;
        public ObservableCollection<string> Patients
        {
            get => _patients;
            set
            {
                _patients = value;
                OnPropertyChanged(nameof(Patients));
            }
        }

        private ObservableCollection<string> _services;
        public ObservableCollection<string> Services
        {
            get => _services;
            set
            {
                _services = value;
                OnPropertyChanged(nameof(Services));
            }
        }

        private ObservableCollection<string> _statuses;
        public ObservableCollection<string> Statuses
        {
            get => _statuses;
            set
            {
                _statuses = value;
                OnPropertyChanged(nameof(Statuses));
            }
        }

        private ObservableCollection<DateTime?> _dates;
        public ObservableCollection<DateTime?> Dates
        {
            get => _dates;
            set
            {
                _dates = value;
                OnPropertyChanged(nameof(Dates));
            }
        }
        public ICommand SelectedItemChangedCommand { get; }
        public ICommand ClearCommand { get; }
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
        private string _serviceName;
        public string ServiceName
        {
            get => _serviceName;
            set
            {
                _serviceName = value;
                OnPropertyChanged(nameof(ServiceName));
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(Status);
            }
        }

        private DateTime? _date;
        public DateTime? Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public DoctorAppointmentsViewModel()
        {
            LoadAppointments();
            SelectedItemChangedCommand = new RelayCommand(ExecuteSelectedItemChanged);
            ClearCommand = new RelayCommand(ExecuteClear);
        }

        private void ExecuteClear(object parameter)
        {
            PatientName = null;
            ServiceName = null;
            Status = null;
            Date = null;
        }

        private void ExecuteSelectedItemChanged(object parameter)
        {
            LoadAppointments(PatientName, ServiceName, Status, Date);
        }
        private void LoadAppointments(string patientName = null, string serviceName = null, string status = null, DateTime? date = null)
        {
            string statusKey = null;
            if (!string.IsNullOrEmpty(status) && CurrentUser.Status.ContainsValue(status))
            {
                statusKey = CurrentUser.Status.FirstOrDefault(x => x.Value == status).Key;
            }
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Appointments = new ObservableCollection<AppointmentItem>(
                            (from a in context.Appointments
                                join p in context.Patients on a.PatientID equals p.ID
                                join s in context.Services on a.ServiceID equals s.ID
                                where a.DoctorID == CurrentUser.ID && (string.IsNullOrEmpty(patientName) || p.LastName + " " + p.FirstName == patientName) && (string.IsNullOrEmpty(serviceName) || s.ServiceName == serviceName)
                                      && (string.IsNullOrEmpty(status) || a.Status == statusKey)
                                      && (!date.HasValue || a.AppointmentDate == date)
                                orderby a.AppointmentDate
                                select new AppointmentItem
                                {
                                    PatientName = p.LastName + " " + p.FirstName,
                                    ServiceName = s.ServiceName,
                                    Status = a.Status,
                                    Date = a.AppointmentDate
                                })
                            .ToList()
                        );
                    }
                }
                Appointments = new ObservableCollection<AppointmentItem>(
                    Appointments.Select(a =>
                    {
                        a.Status = CurrentUser.Status[a.Status];
                        return a;
                    })
                );
                Patients = new ObservableCollection<string>(Appointments.Select(n => n.PatientName).Distinct().ToList());
                Services = new ObservableCollection<string>(Appointments.Select(n => n.ServiceName).Distinct().ToList());
                Statuses = new ObservableCollection<string>(Appointments.Select(n => n.Status).Distinct().ToList());
                Dates = new ObservableCollection<DateTime?>(Appointments.Select(n => n.Date).Distinct().ToList());
                OnPropertyChanged(nameof(Appointments));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

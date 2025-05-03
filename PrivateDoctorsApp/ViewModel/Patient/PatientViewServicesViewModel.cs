using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Patient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PrivateDoctorsApp.ViewModel.Patient
{
    internal class PatientViewServicesViewModel : LogEventBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private PatientSearchViewModel.ServiceItem _service;
        private ObservableCollection<PatientSearchViewModel.DoctorItem> _doctors;
        private ObservableCollection<DateTime> _dates;
        private PatientSearchViewModel.DoctorItem _doctor;
        private DateTime _date;
        private PatientViewServicesWindow _window;

        public ObservableCollection<PatientSearchViewModel.DoctorItem> Doctors
        {
            get => _doctors;
            set
            {
                if (Equals(value, _doctors)) return;
                _doctors = value;
                OnPropertyChanged(nameof(Doctors));
            }
        }

        public ObservableCollection<DateTime> Dates
        {
            get => _dates;
            set
            {
                if (Equals(value, _dates)) return;
                _dates = value;
                OnPropertyChanged(nameof(Dates));
            }
        }

        public PatientSearchViewModel.DoctorItem Doctor
        {
            get => _doctor;
            set
            {
                if (Equals(value, _doctor)) return;
                _doctor = value;
                OnPropertyChanged(nameof(Doctor));
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (Nullable.Equals(value, _date)) return;
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public ICommand SelectedItemChangedCommand { get; }
        public ICommand CreateAppointmentCommand { get; }
        private bool CanCreate()
        {
            return Doctor != null && !string.IsNullOrWhiteSpace(Doctor.DoctorName) && Date > DateTime.MinValue;
        }
        public event Action DataUpdated;
        public PatientViewServicesViewModel(PatientSearchViewModel.ServiceItem service, PatientViewServicesWindow window)
        {
            _window = window;
            _service = service;
            LoadDoctors();
            SelectedItemChangedCommand = new RelayCommand(ExecuteSelectedItemChanged);
            CreateAppointmentCommand = new RelayCommand(ExecuteCreateAppointment, CanCreate);
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
        }

        private void ExecuteCreateAppointment(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var service = context.Services
                            .FirstOrDefault(s => s.ID == _service.ID);
                        if (CurrentUser.Balance < service.Price)
                        {
                            MessageBox.Show("Недостатньо коштів на рахунку", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        var scheduleToUpdate = context.Schedules
                            .FirstOrDefault(s => s.DoctorID == Doctor.ID && s.WorkDate == Date.Date &&
                                                 s.AppointmentStart.Value.Hour == Date.Hour &&
                                                 s.AppointmentStart.Value.Minute == Date.Minute && s.PatientID == null);
                        if (scheduleToUpdate != null)
                        {
                            scheduleToUpdate.PatientID = CurrentUser.ID;
                            context.SaveChanges();
                            var appointment = new Appointment
                            {
                                PatientID = CurrentUser.ID,
                                DoctorID = Doctor.ID,
                                ServiceID = _service.ID,
                                AppointmentDate = Date,
                                Status = "pending"
                            };
                            context.Appointments.Add(appointment);
                            context.SaveChanges();
                            var notification = new Notification
                            {
                                PatientID = CurrentUser.ID,
                                DoctorID = Doctor.ID,
                                Description = "Нове призначення на прийом",
                                DateOfCreation = DateTime.Now,
                                Type = "appointment"
                            };
                            context.Notifications.Add(notification);
                            var patient = context.Patients
                                .FirstOrDefault(p => p.ID == CurrentUser.ID);
                            patient.AccountBalance -= service.Price;
                            context.SaveChanges();
                            DataUpdated?.Invoke();
                            OnLogEvent("Додано запис на прийом", "Appointments");
                            OnLogEvent("Додано нове повідомлення", "Notifications");
                            _window.Close();
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecuteSelectedItemChanged(object parameter)
        {
            LoadDates();
        }

        private void LoadDates()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var dates = (from s in context.Schedules
                                where s.DoctorID == Doctor.ID
                                      && s.WorkDate != null
                                      && s.AppointmentStart != null && s.PatientID == null
                                     select new { s.WorkDate, s.AppointmentStart })
                            .ToList();
                        Dates = new ObservableCollection<DateTime>(
                            dates.Select(x => new DateTime(
                                x.WorkDate.Value.Year,
                                x.WorkDate.Value.Month,
                                x.WorkDate.Value.Day,
                                x.AppointmentStart.Value.Hour,
                                x.AppointmentStart.Value.Minute,
                                x.AppointmentStart.Value.Second))
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadDoctors()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Doctors = new ObservableCollection<PatientSearchViewModel.DoctorItem>((from ds in context.DoctorServices
                            join e in context.Employees on ds.DoctorID equals e.ID
                            where ds.ServiceID == _service.ID
                            select new PatientSearchViewModel.DoctorItem
                            {
                                ID = e.ID,
                                DoctorName = e.LastName + " " + e.FirstName + " " + e.MiddleName

                            }).Distinct().ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

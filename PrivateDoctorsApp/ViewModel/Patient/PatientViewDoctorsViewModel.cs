using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Patient;
using MessageBox = System.Windows.MessageBox;

namespace PrivateDoctorsApp.ViewModel.Patient
{
    internal class PatientViewDoctorsViewModel : LogEventBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private PatientSearchViewModel.DoctorItem _doctor;
        private ObservableCollection<PatientSearchViewModel.ServiceItem> _services;
        private ObservableCollection<DateTime> _dates;
        private PatientSearchViewModel.ServiceItem _service;
        private DateTime _date;
        private PatientViewDoctorsWindow _window;

        public ObservableCollection<PatientSearchViewModel.ServiceItem> Services
        {
            get => _services;
            set
            {
                if (Equals(value, _services)) return;
                _services = value;
                OnPropertyChanged(nameof(Services));
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

        public PatientSearchViewModel.ServiceItem Service
        {
            get => _service;
            set
            {
                if (Equals(value, _service)) return;
                _service = value;
                OnPropertyChanged(nameof(Service));
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (value.Equals(_date)) return;
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public ICommand SelectedItemChangedCommand { get; }
        public ICommand CreateAppointmentCommand { get; }
        private bool CanCreate()
        {
            return Service != null && !string.IsNullOrWhiteSpace(Service.ServiceName) && Date > DateTime.MinValue;
        }
        public event Action DataUpdated;
        public PatientViewDoctorsViewModel(PatientSearchViewModel.DoctorItem doctor, PatientViewDoctorsWindow window)
        {
            _window = window;
            _doctor = doctor;
            SelectedItemChangedCommand = new RelayCommand(ExecuteSelectedItemChanged);
            CreateAppointmentCommand = new RelayCommand(ExecuteCreateAppointment, CanCreate);
            LoadServices();
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
        }
        private void ExecuteSelectedItemChanged(object parameter)
        {
            LoadDates();
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
                            .FirstOrDefault(s => s.ID == Service.ID);
                        if (CurrentUser.Balance < service.Price)
                        {
                            MessageBox.Show("Недостатньо коштів на рахунку", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        var scheduleToUpdate = context.Schedules
                            .FirstOrDefault(s => s.DoctorID == _doctor.ID && s.WorkDate == Date.Date &&
                                                 s.AppointmentStart.Value.Hour == Date.Hour &&
                                                 s.AppointmentStart.Value.Minute == Date.Minute && s.PatientID == null);
                        if (scheduleToUpdate != null)
                        {
                            scheduleToUpdate.PatientID = CurrentUser.ID;
                            context.SaveChanges();
                            var appointment = new Appointment
                            {
                                PatientID = CurrentUser.ID,
                                DoctorID = _doctor.ID,
                                ServiceID = Service.ID,
                                AppointmentDate = Date,
                                Status = "pending"
                            };
                            context.Appointments.Add(appointment);
                            context.SaveChanges();
                            var notification = new Notification
                            {
                                PatientID = CurrentUser.ID,
                                DoctorID = _doctor.ID,
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
                                where s.DoctorID == _doctor.ID
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
        private void LoadServices()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Services = new ObservableCollection<PatientSearchViewModel.ServiceItem>((from ds in context.DoctorServices
                            join s in context.Services on ds.ServiceID equals s.ID
                            where ds.DoctorID == _doctor.ID
                            select new PatientSearchViewModel.ServiceItem
                            {
                                ID = s.ID,
                                ServiceName = s.ServiceName

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

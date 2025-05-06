using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Doctor;

namespace PrivateDoctorsApp.ViewModel
{
    internal class AddScheduleViewModel : LogEventBase, INotifyPropertyChanged
    {
        private DateTime? _workDate;

        public DateTime? WorkDate
        {
            get => _workDate;
            set
            {
                _workDate = value;
                OnPropertyChanged(nameof(WorkDate));
            }
        }


        private DateTime? _appointmentStart;

        public DateTime? AppointmentStart
        {
            get => _appointmentStart;
            set
            {
                if (value.HasValue)
                {
                    var time = value.Value.TimeOfDay;

                    if (time >= TimeSpan.FromHours(8) && time <= TimeSpan.FromHours(20))
                        _appointmentStart = value;
                    else
                        _appointmentStart = value.Value.Date.AddHours(8);
                }
                else
                {
                    _appointmentStart = null;
                }

                OnPropertyChanged(nameof(AppointmentStart));
            }
        }


        private DateTime? _appointmentEnd;

        public DateTime? AppointmentEnd
        {
            get => _appointmentEnd;
            set
            {
                if (value.HasValue)
                {
                    var time = value.Value.TimeOfDay;

                    if (time >= TimeSpan.FromHours(8) && time <= TimeSpan.FromHours(20))
                        _appointmentEnd = value;
                    else
                        _appointmentEnd = value.Value.Date.AddHours(20);
                }
                else
                {
                    _appointmentEnd = null;
                }

                OnPropertyChanged(nameof(AppointmentEnd));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public ICommand AddScheduleCommand { get; }
        private readonly AddScheduleWindow _window;
        public event Action DataUpdated;
        private bool CanAdd()
        {
            return WorkDate.HasValue && AppointmentStart.HasValue && AppointmentEnd.HasValue;
        }
        public AddScheduleViewModel(AddScheduleWindow window)
        {
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
            AddScheduleCommand = new RelayCommand(ExecuteAddSchedule, CanAdd);
            _window = window;
        }

        private void ExecuteAddSchedule(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var newSchedule = new Schedule()
                        {
                            WorkDate = WorkDate,
                            AppointmentStart = AppointmentStart,
                            AppointmentEnd = AppointmentEnd,
                            DoctorID = CurrentUser.ID,
                            PatientID = null
                        };
                        context.Schedules.Add(newSchedule);
                        context.SaveChanges();
                        OnLogEvent("Додано графік", "Schedules");
                        DataUpdated?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            _window.Close();
        }
    }
}

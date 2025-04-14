using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Doctor;
using System.Windows.Input;
using System.Windows;

namespace PrivateDoctorsApp.ViewModel.Doctor
{
    internal class ChangeScheduleViewModel: LogEventBase, INotifyPropertyChanged
    {
        private readonly ChangeScheduleWindow _window;
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action DataUpdated;
        public ICommand ChangeScheduleCommand { get; }

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
                if (_appointmentStart != value)
                {
                    _appointmentStart = value;
                    OnPropertyChanged(nameof(AppointmentStart));
                }
            }
        }
        private DateTime? _appointmentEnd;

        public DateTime? AppointmentEnd
        {
            get => _appointmentEnd;
            set
            {
                if (_appointmentEnd != value)
                {
                    _appointmentEnd = value;
                    OnPropertyChanged(nameof(AppointmentEnd));
                }
            }
        }
        private ObservableCollection<DateTime?> _scheduleDates;
        public ObservableCollection<DateTime?> ScheduleDates
        {
            get { return _scheduleDates; }
            set
            {
                _scheduleDates = value;
                OnPropertyChanged(nameof(ScheduleDates));
            }
        }
        private ObservableCollection<DateTime?> _scheduleStarts;
        public ObservableCollection<DateTime?> ScheduleStarts
        {
            get { return _scheduleStarts; }
            set
            {
                _scheduleStarts = value;
                OnPropertyChanged(nameof(ScheduleStarts));
            }
        }
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private bool CanChange()
        {
            return WorkDate.HasValue && AppointmentStart.HasValue && AppointmentEnd.HasValue;
        }

        public ChangeScheduleViewModel(ChangeScheduleWindow window)
        {
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
            ChangeScheduleCommand = new RelayCommand(ExecuteChangeSchedule, CanChange);
            _window = window;
            LoadSchedules();
        }


        private void ExecuteChangeSchedule(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var scheduleToUpdate = context.Schedules
                            .FirstOrDefault(s => s.WorkDate == WorkDate);

                        if (scheduleToUpdate != null)
                        {
                            scheduleToUpdate.AppointmentStart = AppointmentStart;
                            scheduleToUpdate.AppointmentEnd = AppointmentEnd;
                            context.SaveChanges();
                            OnLogEvent("Оновлено графік", "Schedules");
                            DataUpdated?.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            _window.Close();
        }
        private void LoadSchedules()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        ScheduleDates = new ObservableCollection<DateTime?>(
                            (from s in context.Schedules
                                where s.DoctorID == CurrentUser.ID
                                orderby s.WorkDate
                                select s.WorkDate)
                            .ToList());
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

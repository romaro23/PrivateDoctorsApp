using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Doctor;
using System.Windows;

namespace PrivateDoctorsApp.ViewModel
{
    internal class DeleteScheduleViewModel : LogEventBase, INotifyPropertyChanged

    {
        private readonly DeleteScheduleWindow _window;
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action DataUpdated;
        public ICommand DeleteScheduleCommand { get; }

        private DateTime? _workDate;
        public DateTime? WorkDate
        {
            get => _workDate;
            set
            {
                _workDate = value;
                OnPropertyChanged(nameof(WorkDate));
                LoadTimes();
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
        private bool CanDelete()
        {
            return WorkDate.HasValue && AppointmentStart.HasValue;
        }
        public DeleteScheduleViewModel(DeleteScheduleWindow window)
        {
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
            DeleteScheduleCommand = new RelayCommand(ExecuteDeleteSchedule, CanDelete);
            _window = window;
            LoadSchedules();
        }



        private void ExecuteDeleteSchedule(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var scheduleToDelete = context.Schedules
                            .FirstOrDefault(s => s.WorkDate == WorkDate && s.AppointmentStart == AppointmentStart);

                        if (scheduleToDelete != null)
                        {
                            context.Schedules.Remove(scheduleToDelete);
                            context.SaveChanges();
                            OnLogEvent("Видалено графік", "Schedules");
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

        private void LoadTimes()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        ScheduleStarts = new ObservableCollection<DateTime?>(
                            (from s in context.Schedules
                                where s.DoctorID == CurrentUser.ID && s.WorkDate == WorkDate
                                orderby s.WorkDate
                                select s.AppointmentStart)
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

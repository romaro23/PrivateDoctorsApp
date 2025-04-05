using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Doctor;

namespace PrivateDoctorsApp.ViewModel
{
    internal class AddScheduleViewModel : INotifyPropertyChanged
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

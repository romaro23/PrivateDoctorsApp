using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Doctor;
using PrivateDoctorsApp.ViewModel.Doctor;

namespace PrivateDoctorsApp.ViewModel
{
    internal class DoctorScheduleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public DateTime? WorkDate { get; set; }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<ScheduleItem> _schedule;
        public ObservableCollection<ScheduleItem> Schedule
        {
            get { return _schedule; }
            set
            {
                _schedule = value;
                OnPropertyChanged(nameof(Schedule));
            }
        }
        public class ScheduleItem
        {
            public DateTime? WorkDate { get; set; }
            public DateTime? AppointmentStart { get; set; }
            public DateTime? AppointmentEnd { get; set; }
            public string PatientName { get; set; } = "—";

            public string FormattedWorkDate
            {
                get
                {
                    return WorkDate.HasValue ? WorkDate.Value.ToString("d MMMM", new CultureInfo("uk-UA")) : "";
                }
            }

            public string FormattedAppointmentStart
            {
                get
                {
                    return AppointmentStart.HasValue ? AppointmentStart.Value.ToString("HH:mm", new CultureInfo("uk-UA")) : "";
                }
            }

            public string FormattedAppointmentEnd
            {
                get
                {
                    return AppointmentEnd.HasValue ? AppointmentEnd.Value.ToString("HH:mm", new CultureInfo("uk-UA")) : "";
                }
            }
        }
        public ICommand OpenAddScheduleWindowCommand { get; }
        public ICommand OpenDeleteScheduleWindowCommand { get; }
        public ICommand OpenChangeScheduleWindowCommand { get; }
        public ICommand SelectedDateChangedCommand { get; }
        public DoctorScheduleViewModel()
        {
            LoadSchedule();
            OpenAddScheduleWindowCommand = new RelayCommand(ExecuteOpenAddScheduleWindow);
            OpenDeleteScheduleWindowCommand = new RelayCommand(ExecuteOpenDeleteScheduleWindow);
            SelectedDateChangedCommand = new RelayCommand(ExecuteSelectDate);
            OpenChangeScheduleWindowCommand = new RelayCommand(ExecuteOpenChangeScheduleWindow);
        }

        private void ExecuteSelectDate(object parameter)
        {
            LoadSchedule(WorkDate);
        }
        private void ExecuteOpenChangeScheduleWindow(object parameter)
        {
            ChangeScheduleWindow window = new ChangeScheduleWindow();
            var changeScheduleViewModel = new ChangeScheduleViewModel(window);
            window.DataContext = changeScheduleViewModel;
            changeScheduleViewModel.DataUpdated += () => LoadSchedule();
            window.Show();
        }
        private void ExecuteOpenDeleteScheduleWindow(object parameter)
        {
            DeleteScheduleWindow window = new DeleteScheduleWindow();
            var deleteScheduleViewModel = new DeleteScheduleViewModel(window);
            window.DataContext = deleteScheduleViewModel;
            deleteScheduleViewModel.DataUpdated += () => LoadSchedule();
            window.Show();
        }
        private void ExecuteOpenAddScheduleWindow(object parameter)
        {
            AddScheduleWindow window = new AddScheduleWindow();
            var addScheduleViewModel = new AddScheduleViewModel(window);
            window.DataContext = addScheduleViewModel;
            addScheduleViewModel.DataUpdated += () => LoadSchedule();
            window.Show();
        }

        private void LoadSchedule(DateTime? date = null)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        if (date == null)
                        {
                            Schedule = new ObservableCollection<ScheduleItem>(
                                (from s in context.Schedules
                                    join p in context.Patients on s.PatientID equals p.ID into patientGroup
                                    from p in patientGroup.DefaultIfEmpty()
                                    where s.DoctorID == CurrentUser.ID
                                    orderby s.WorkDate
                                    select new ScheduleItem
                                    {
                                        WorkDate = s.WorkDate,
                                        AppointmentStart = s.AppointmentStart,
                                        AppointmentEnd = s.AppointmentEnd,
                                        PatientName = p != null ? p.FirstName + " " + p.LastName : "—"
                                    })
                                .ToList()
                            );
                        }
                        else
                        {
                            Schedule = new ObservableCollection<ScheduleItem>(
                                (from s in context.Schedules
                                    join p in context.Patients on s.PatientID equals p.ID into patientGroup
                                    from p in patientGroup.DefaultIfEmpty()
                                    where s.DoctorID == CurrentUser.ID && s.WorkDate == date
                                    orderby s.WorkDate
                                    select new ScheduleItem
                                    {
                                        WorkDate = s.WorkDate,
                                        AppointmentStart = s.AppointmentStart,
                                        AppointmentEnd = s.AppointmentEnd,
                                        PatientName = p != null ? p.FirstName + " " + p.LastName : "—"
                                    })
                                .ToList()
                            );
                            OnPropertyChanged(nameof(Schedule));
                        }
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

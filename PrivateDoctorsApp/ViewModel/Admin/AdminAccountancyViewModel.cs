using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrivateDoctorsApp.Model;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class AdminAccountancyViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<object> _appointments;
        public ObservableCollection<object> Appointments
        {
            get { return _appointments; }
            set
            {
                _appointments = value;
                OnPropertyChanged(nameof(Appointments));
            }
        }
        private ObservableCollection<object> _appointmentsBySpecialization;
        public ObservableCollection<object> AppointmentsBySpecialization
        {
            get { return _appointmentsBySpecialization; }
            set
            {
                _appointmentsBySpecialization = value;
                OnPropertyChanged(nameof(AppointmentsBySpecialization));
            }
        }
        private ObservableCollection<object> _incomeBySpecialization;
        public ObservableCollection<object> IncomeBySpecialization
        {
            get { return _incomeBySpecialization; }
            set
            {
                _incomeBySpecialization = value;
                OnPropertyChanged(nameof(IncomeBySpecialization));
            }
        }
        private ObservableCollection<object> _appointmentsByDoctor;
        public ObservableCollection<object> AppointmentsByDoctor
        {
            get { return _appointmentsByDoctor; }
            set
            {
                _appointmentsByDoctor = value;
                OnPropertyChanged(nameof(AppointmentsByDoctor));
            }
        }
        private ObservableCollection<object> _totalIncome;
        public ObservableCollection<object> TotalIncome
        {
            get { return _totalIncome; }
            set
            {
                _totalIncome = value;
                OnPropertyChanged(nameof(TotalIncome));
            }
        }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public ICommand DateCommand { get; set; }
        public ICommand GenerateCSVCommand { get; set; }
        private TabControl _tabControl;
        public AdminAccountancyViewModel(TabControl tabControl)
        {
            _tabControl = tabControl;
            DateCommand = new RelayCommand(ExecuteLoad);
            GenerateCSVCommand = new RelayCommand(GenerateReport);
            LoadSpecializationAppointments();
            LoadSpecializationIncome();
            LoadDoctorAppointments();
        }

        private void GenerateReport(object parameter)
        {
            var tab = _tabControl.SelectedItem as TabItem;
            var activeTab = tab.Header as string;
            var projectDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var rootDirectory = Directory.GetParent(projectDirectory).FullName;
            var assetsDirectory = Directory.GetParent(rootDirectory).FullName;
            string filePath = "";
            switch (activeTab)
            {
                case "Кількість бронювань за період":
                    filePath = Path.Combine(assetsDirectory, "Assets", "Appointments.csv");
                    GenerateCSV(Appointments, filePath);
                    break;
                case "Бронювання по спеціалізаціях":
                    filePath = Path.Combine(assetsDirectory, "Assets", "AppointmentsBySpecialization.csv");
                    GenerateCSV(AppointmentsBySpecialization, filePath);
                    break;
                case "Прибуток по спеціалізаціях":
                    filePath = Path.Combine(assetsDirectory, "Assets", "IncomeBySpecialization.csv");
                    GenerateCSV(IncomeBySpecialization, filePath);
                    break;
                case "Кількість бронювань по лікарях":
                    filePath = Path.Combine(assetsDirectory, "Assets", "AppointmentsByDoctor.csv");
                    GenerateCSV(AppointmentsByDoctor, filePath);
                    break;
                case "Прибуток за період":
                    filePath = Path.Combine(assetsDirectory, "Assets", "TotalIncome.csv");
                    GenerateCSV(TotalIncome, filePath);
                    break;
            }
        }

        private void GenerateCSV(ObservableCollection<object> data, string filePath)
        {
            var stringBuilder = new StringBuilder();
            
            var properties = data.FirstOrDefault()?.GetType().GetProperties();
            if (properties != null)
            {
                var headers = string.Join(",", properties.Select(p => p.Name));
                stringBuilder.AppendLine(headers);
            }

            foreach (var item in data)
            {
                var values = string.Join(",", properties.Select(p => p.GetValue(item)?.ToString().Replace(",", ";")));
                stringBuilder.AppendLine(values);
            }

            File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);
        }
        private void ExecuteLoad(object parameter)
        {
            LoadAppointments();
            LoadIncome();
        }

        private void LoadIncome()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var sum = context.Payments.Sum(p => p.Amount);
                        TotalIncome = new ObservableCollection<object>(
                            (from a in context.Appointments
                                join s in context.Services on a.ServiceID equals s.ID
                                where a.AppointmentDate >= DateStart && a.AppointmentDate <= DateEnd
                                group s by 1 into g
                                select new
                                {
                                    Income = g.Sum(service => service.Price) + sum
                                }
                            ).ToList()
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadDoctorAppointments()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        AppointmentsByDoctor = new ObservableCollection<object>(
                            (from e in context.Employees
                                join a in context.Appointments on e.ID equals a.DoctorID
                                group a by new { e.ID, e.LastName, e.FirstName, e.MiddleName } into g
                                select new
                                {
                                    Doctor = g.Key.LastName + " " + g.Key.FirstName + " " + g.Key.MiddleName,
                                    Appointment = g.Count()
                                }
                            ).ToList()
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LoadSpecializationIncome()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        IncomeBySpecialization = new ObservableCollection<object>(
                            (from p in context.Professionals
                                join a in context.Appointments on p.EmployeeID equals a.DoctorID
                                join s in context.Services on a.ServiceID equals s.ID
                                group s by p.Specialization into g
                                select new
                                {
                                    Specialization = g.Key,
                                    Income = g.Sum(service => service.Price)
                                }
                            ).ToList()
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LoadSpecializationAppointments()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        AppointmentsBySpecialization = new ObservableCollection<object>(
                            (from p in context.Professionals
                                join a in context.Appointments on p.EmployeeID equals a.DoctorID
                                group a by p.Specialization into g
                                select new
                                {
                                    Specialization = g.Key,
                                    Appointment = g.Count()
                                }
                            ).ToList()
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LoadAppointments()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var appointments = context.Appointments
                            .Where(a => a.AppointmentDate >= DateStart && a.AppointmentDate <= DateEnd)
                            .OrderBy(a => a.AppointmentDate)
                            .ToList();
                        var totalAppointments = appointments.Count;
                        var confirmedCount = appointments.Count(a => a.Status == "confirmed");
                        var pendingCount = appointments.Count(a => a.Status == "pending");
                        var cancelledCount = appointments.Count(a => a.Status == "cancelled");
                        Appointments = new ObservableCollection<object>
                        {
                            new
                            {
                                TotalAppointments = totalAppointments,
                                Confirmed = confirmedCount,
                                Pending = pendingCount,
                                Cancelled = cancelledCount
                            }
                        };
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using Bogus.DataSets;
using System.Windows.Media;
using PrivateDoctorsApp.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand SelectPathCommand { get; }
        public ICommand TestCommand { get; }
        private string _path = CurrentUser.Path;

        public string Path
        {
            get => _path;
            set
            {
                if (value == _path) return;
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }
        private DateTime? _startDate = CurrentUser.PeriodStart, _endDate = CurrentUser.PeriodEnd;

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (Nullable.Equals(value, _startDate)) return;
                _startDate = value;
                CurrentUser.PeriodStart = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (Nullable.Equals(value, _endDate)) return;
                _endDate = value;
                CurrentUser.PeriodEnd = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32", SetLastError = true)]
        public static extern void FreeConsole();
        public SettingsViewModel()
        {
            SelectPathCommand = new RelayCommand(ExecuteSelectPath);
            TestCommand = new RelayCommand(ExecuteTest);
        }

        private void ExecuteTest(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var doctors = GenerateFakeDoctors(1000);
                        var sw = Stopwatch.StartNew();
                        foreach (var doctor in doctors)
                        {
                            var newDoctor = new Model.Employee
                            {
                                FirstName = doctor.FirstName,
                                LastName = doctor.LastName,
                                MiddleName = doctor.MiddleName,
                                ContactNumber = doctor.ContactNumber,
                                Email = doctor.Email,
                                Address = doctor.Address
                            };
                            context.Employees.Add(newDoctor);
                            context.SaveChanges();
                        }

                        var result = (from e in context.Employees
                            join a in context.Appointments on e.ID equals a.DoctorID
                            select new
                            {
                                DoctorName = e.FirstName + " " + e.LastName,
                                Appointment = a.ID
                            }).ToList();
                        sw.Stop();
                        AllocConsole();
                        Console.WriteLine($"Час виконання запитів до багатьох даних: {sw.ElapsedMilliseconds} мс");
                    }
                }
                var newSw = Stopwatch.StartNew();
                Parallel.For(0, 50, i =>
                {
                    using (var context = new PrivateDoctorsDBEntities1())
                    {
                        if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                            context.Database.Connection.Open();
                        if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                        {
                            var newResult = (from e in context.Patients
                                join a in context.Appointments on e.ID equals a.PatientID
                                select new
                                {
                                    PName = e.FirstName + " " + e.LastName,
                                    Appointment = a.ID
                                }).ToList();
                        }
                    }
                });
                newSw.Stop();
                Console.WriteLine($"Час виконання запитів від багатьох користувачів: {newSw.ElapsedMilliseconds} мс");
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private List<Model.Employee> GenerateFakeDoctors(int count = 10)
        {
            var faker = new Bogus.Faker<Model.Employee>("uk")
                .RuleFor(e => e.FirstName, f => f.Name.FirstName())
                .RuleFor(e => e.LastName, f => f.Name.LastName())
                .RuleFor(e => e.MiddleName, f => f.Name.FirstName())
                .RuleFor(e => e.ContactNumber, f => f.Phone.PhoneNumber())
                .RuleFor(e => e.Email, f => f.Internet.Email())
                .RuleFor(e => e.Address, f => f.Address.FullAddress());
            return faker.Generate(count);
        }
        private void ExecuteSelectPath(object parameter)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Path = dialog.SelectedPath;
                    CurrentUser.Path = Path;
                }
            }
        }

    }
}

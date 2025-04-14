using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Doctor;

namespace PrivateDoctorsApp.ViewModel.Doctor
{
    internal class DoctorMainPageViewModel
    {
        public class PersonalData : INotifyPropertyChanged
        {
            private string _lastName;
            private string _firstName;
            private string _middleName;
            private string _contactNumber;
            private string _email;
            private string _address;
            private string _username;
            private string _password;
            private string _specialization;
            private int? _experienceYears;
            private string _education;
            private decimal? _rating;

            public string LastName
            {
                get => _lastName;
                set { _lastName = value; OnPropertyChanged(nameof(LastName)); }
            }

            public string FirstName
            {
                get => _firstName;
                set { _firstName = value; OnPropertyChanged(nameof(FirstName)); }
            }

            public string MiddleName
            {
                get => _middleName;
                set { _middleName = value; OnPropertyChanged(nameof(MiddleName)); }
            }

            public string ContactNumber
            {
                get => _contactNumber;
                set { _contactNumber = value; OnPropertyChanged(nameof(ContactNumber)); }
            }

            public string Email
            {
                get => _email;
                set { _email = value; OnPropertyChanged(nameof(Email)); }
            }

            public string Address
            {
                get => _address;
                set { _address = value; OnPropertyChanged(nameof(Address)); }
            }

            public string Username
            {
                get => _username;
                set { _username = value; OnPropertyChanged(nameof(Username)); }
            }

            public string Password
            {
                get => _password;
                set { _password = value; OnPropertyChanged(nameof(Password)); }
            }

            public string Specialization
            {
                get => _specialization;
                set { _specialization = value; OnPropertyChanged(nameof(Specialization)); }
            }

            public int? ExperienceYears
            {
                get => _experienceYears;
                set { _experienceYears = value; OnPropertyChanged(nameof(ExperienceYears)); }
            }

            public string Education
            {
                get => _education;
                set { _education = value; OnPropertyChanged(nameof(Education)); }
            }

            public decimal? Rating
            {
                get => _rating;
                set { _rating = value; OnPropertyChanged(nameof(Rating)); }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class Notification
        {
            public DateTime? Date { get; set; }
            public string Description { get; set; }
            public string PatientName { get; set; }
            public string FormattedDate
            {
                get
                {
                    return Date.HasValue ? Date.Value.ToString("d MMMM", new CultureInfo("uk-UA")) : "";
                }
            }
        }

        public class Patient
        {
            public DateTime? DateOfBirth { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string PatientName { get; set; }
            public string FormattedDateOfBirth
            {
                get
                {
                    return DateOfBirth.HasValue ? DateOfBirth.Value.ToString("dd/MM/yyyy") : "";
                }
            }
        }
        public PersonalData PersonalData_ { get; set; }
        public ObservableCollection<Notification> Notifications { get; set; }
        public ObservableCollection<Patient> Patients { get; set; }

        public ICommand ViewNotificationsCommand { get; }
        public DoctorMainPageViewModel()
        {
            LoadPersonalData();
            LoadNotifications();
            LoadPatients();
            ViewNotificationsCommand = new RelayCommand(ExecuteViewNotifications);
        }

        private void ExecuteViewNotifications(object parameter)
        {
            NotificationsWindow window = new NotificationsWindow();
            window.Show();
        }

        private void LoadPersonalData()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var result = (from e in context.Employees
                            join p in context.Professionals on e.ID equals p.EmployeeID
                            join u in context.Users on e.ID equals u.DoctorID
                            where e.ID == CurrentUser.ID
                            select new
                            {
                                e.LastName,
                                e.FirstName,
                                e.MiddleName,
                                e.ContactNumber,
                                e.Email,
                                e.Address,
                                u.Username,
                                u.Password,
                                p.Specialization,
                                p.ExperienceYears,
                                p.Education,
                                p.Rating
                            }).FirstOrDefault();
                        if (result != null)
                        {
                            PersonalData_ = new PersonalData
                            {
                                LastName = result.LastName,
                                FirstName = result.FirstName,
                                MiddleName = result.MiddleName,
                                ContactNumber = result.ContactNumber,
                                Email = result.Email,
                                Address = result.Address,
                                Username = result.Username,
                                Password = result.Password,
                                Specialization = result.Specialization,
                                ExperienceYears = result.ExperienceYears,
                                Education = result.Education,
                                Rating = result.Rating
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LoadNotifications()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Notifications = new ObservableCollection<Notification>(
                            (from n in context.Notifications
                                join p in context.Patients on n.PatientID equals p.ID
                                where n.DoctorID == CurrentUser.ID && (n.Type == "appointment" || n.Type == "cancellation")
                                orderby n.DateOfCreation
                                select new Notification
                                {
                                    Date = n.DateOfCreation,
                                    Description = n.Description,
                                    PatientName = "Сповіщення від " + p.LastName + " " + p.FirstName
                                })
                            .Take(3)
                            .ToList()
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadPatients()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Patients = new ObservableCollection<Patient>(
                            (from a in context.Appointments
                                join p in context.Patients on a.PatientID equals p.ID
                                where a.DoctorID == CurrentUser.ID
                                select new Patient
                                {
                                    DateOfBirth = p.BirthDate,
                                    Email = p.Email,
                                    PatientName = p.LastName + " " + p.FirstName,
                                    Phone = p.ContactNumber
                                })
                            .ToList()
                        );
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

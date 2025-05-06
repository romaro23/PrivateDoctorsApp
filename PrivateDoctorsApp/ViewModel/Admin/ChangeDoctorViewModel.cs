using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Admin;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class ChangeDoctorViewModel : LogEventBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _lastName, _firstName, _middleName, _phone, _email, _address, _specialization, _education, _username, _password, _experienceYears, _rating;
        private int? _id;
        public string Specialization
        {
            get => _specialization;
            set
            {
                if (value == _specialization) return;
                _specialization = value;
                OnPropertyChanged(nameof(Specialization));
            }
        }

        public string Education
        {
            get => _education;
            set
            {
                if (value == _education) return;
                _education = value;
                OnPropertyChanged(nameof(Education));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (value == _username) return;
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (value == _password) return;
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ExperienceYears
        {
            get => _experienceYears;
            set
            {
                if (value == _experienceYears) return;
                _experienceYears = value;
                OnPropertyChanged(nameof(ExperienceYears));
            }
        }

        public string Rating
        {
            get => _rating;
            set
            {
                if (value == _rating) return;
                _rating = value;
                OnPropertyChanged(nameof(Rating));
            }
        }

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

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(nameof(Phone)); }
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
        private bool IsValidLastName() =>
            !string.IsNullOrWhiteSpace(LastName);

        private bool IsValidFirstName() =>
            !string.IsNullOrWhiteSpace(FirstName);

        private bool IsValidMiddleName() =>
            !string.IsNullOrWhiteSpace(MiddleName);

        private bool IsValidEmail() =>
            !string.IsNullOrWhiteSpace(Email) &&
            Regex.IsMatch(Email, @"^[\w\.-]+@[\w\.-]+\.\w+$");

        private bool IsValidPhone() =>
            !string.IsNullOrWhiteSpace(Phone) &&
            Regex.IsMatch(Phone, @"^\+?\d{10,15}$");

        private bool IsValidAddress() =>
            !string.IsNullOrWhiteSpace(Address);

        private bool IsValidSpecialization() =>
            !string.IsNullOrWhiteSpace(Specialization);

        private bool IsValidExperienceYears() =>
            !string.IsNullOrWhiteSpace(ExperienceYears) &&
            Regex.IsMatch(ExperienceYears, @"^\d+$");

        private bool IsValidEducation() =>
            !string.IsNullOrWhiteSpace(Education);

        private bool IsValidRating() =>
            !string.IsNullOrWhiteSpace(Rating) &&
            decimal.TryParse(Rating, NumberStyles.Number, CultureInfo.InvariantCulture, out _);

        private bool IsValidUsername() =>
            !string.IsNullOrWhiteSpace(Username);

        private bool IsValidPassword() =>
            !string.IsNullOrWhiteSpace(Password);
        public ICommand ValidateLastNameCommand => new RelayCommand(_ =>
        {
            if (!IsValidLastName())
                ShowWarning("Прізвище не може бути порожнім.");
        });
        public ICommand ValidateFirstNameCommand => new RelayCommand(_ =>
        {
            if (!IsValidFirstName())
                ShowWarning("Ім'я не може бути порожнім.");
        });
        public ICommand ValidateMiddleNameCommand => new RelayCommand(_ =>
        {
            if (!IsValidMiddleName())
                ShowWarning("По-батькові не може бути порожнім.");
        });
        public ICommand ValidateEmailCommand => new RelayCommand(_ =>
        {
            if (!IsValidEmail())
                ShowWarning("Невірний формат Email.");
        });

        public ICommand ValidatePhoneCommand => new RelayCommand(_ =>
        {
            if (!IsValidPhone())
                ShowWarning("Невірний формат номера телефону.");
        });

        public ICommand ValidateAddressCommand => new RelayCommand(_ =>
        {
            if (!IsValidAddress())
                ShowWarning("Адреса не може бути порожньою.");
        });

        public ICommand ValidateSpecializationCommand => new RelayCommand(_ =>
        {
            if (!IsValidSpecialization())
                ShowWarning("Спеціалізація не може бути порожньою.");
        });
        public ICommand ValidateExperienceYearsCommand => new RelayCommand(_ =>
        {
            if (!IsValidExperienceYears())
            {
                ShowWarning("Досвід не може бути порожнім і повинен містити лише цифри.");
            }
        });
        public ICommand ValidateEducationCommand => new RelayCommand(_ =>
        {
            if (!IsValidEducation())
                ShowWarning("Освіта не може бути порожньою.");
        });
        public ICommand ValidateRatingCommand => new RelayCommand(_ =>
        {
            if (!IsValidRating())
            {
                ShowWarning("Рейтинг не може бути порожнім і повинен бути числом з десятковою крапкою (наприклад: 4.5).");
            }
        });
        public ICommand ValidateUsernameCommand => new RelayCommand(_ =>
        {
            if (!IsValidUsername())
                ShowWarning("Логін не може бути порожнім.");
        });
        public ICommand ValidatePasswordCommand => new RelayCommand(_ =>
        {
            if (!IsValidPassword())
                ShowWarning("Пароль не може бути порожнім.");
        });
        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private readonly ChangeDoctorWindow _window;
        public event Action DataUpdated;
        public ICommand ChangeDoctorCommand { get; }
        private bool CanChange()
        {
            return IsValidFirstName() &&
                   IsValidLastName() &&
                   IsValidEmail() &&
                   IsValidMiddleName() &&
                   IsValidPhone() &&
                   IsValidSpecialization() &&
                   IsValidEducation() &&
                   IsValidExperienceYears() &&
                   IsValidAddress() &&
                   IsValidRating() &&
                   IsValidUsername() &&
                   IsValidPassword();
        }

        private AdminDoctorsViewModel.PersonalData _doctor;
        public ChangeDoctorViewModel(ChangeDoctorWindow window, AdminDoctorsViewModel.PersonalData doctor = null)
        {
            LogEvent += (sender,action, tableName) => CurrentUser.AddLog(action, tableName);
            _doctor = doctor;
            _window = window;
            ChangeDoctorCommand = new RelayCommand(ExecuteChange, CanChange);
            if (_window.Title == "Оновити дані" || _window.Title == "Видалити лікаря")
            {
                FitData();
                _id = doctor.ID;
            }
        }

        private void FitData()
        {
            LastName = _doctor.LastName;
            Email = _doctor.Email;
            MiddleName = _doctor.MiddleName;
            FirstName = _doctor.FirstName;
            Address = _doctor.Address;
            Phone = _doctor.ContactNumber;
            Specialization = _doctor.Specialization;
            Password = _doctor.Password;
            Username = _doctor.Username;
            ExperienceYears = _doctor.ExperienceYears.ToString();
            Rating = _doctor.Rating.ToString();
            Education = _doctor.Education;
        }

        private void ExecuteChange(object obj)
        {
            var windowName = _window.Title;
            switch (windowName)
            {
                case "Додати лікаря":
                    try
                    {
                        using (var context = new PrivateDoctorsDBEntities1())
                        {
                            if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                                context.Database.Connection.Open();
                            if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                var newEmployee = new Model.Employee
                                {
                                    LastName = LastName,
                                    FirstName = FirstName,
                                    MiddleName = MiddleName,
                                    Address = Address,
                                    ContactNumber = Phone,
                                    Email = Email
                                };
                                context.Employees.Add(newEmployee);
                                context.SaveChanges();
                                var id = newEmployee.ID;
                                var experienceYears = int.Parse(ExperienceYears);
                                var rating = decimal.Parse(Rating);
                                var newProfessional = new Model.Professional
                                {
                                    Education = Education,
                                    EmployeeID = id,
                                    ExperienceYears = experienceYears,
                                    Specialization = Specialization,
                                    Rating = rating
                                };
                                context.Professionals.Add(newProfessional);
                                context.SaveChanges();
                                var newUser = new Model.User
                                {
                                    DoctorID = id,
                                    Username = Username,
                                    Password = Password,
                                    Role = "doctor"
                                };
                                context.Users.Add(newUser);
                                context.SaveChanges();
                                OnLogEvent("Додано особисту інформацію лікаря", "Employee");
                                OnLogEvent("Додано професійну інформацію лікаря", "Professional");
                                OnLogEvent("Додано дані акаунта лікаря", "Users");
                                DataUpdated?.Invoke();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;
                case "Оновити дані":
                    try
                    {
                        using (var context = new PrivateDoctorsDBEntities1())
                        {
                            if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                                context.Database.Connection.Open();
                            if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                var employee = context.Employees.FirstOrDefault(e => e.ID == _id);
                                if (employee != null)
                                {
                                    employee.LastName = LastName;
                                    employee.FirstName = FirstName;
                                    employee.MiddleName = MiddleName;
                                    employee.Address = Address;
                                    employee.ContactNumber = Phone;
                                    employee.Email = Email;

                                    var professional = context.Professionals.FirstOrDefault(p => p.EmployeeID == _id);
                                    if (professional != null)
                                    {
                                        professional.Education = Education;
                                        professional.ExperienceYears = int.Parse(ExperienceYears);
                                        professional.Specialization = Specialization;
                                        professional.Rating = decimal.Parse(Rating);
                                    }

                                    var user = context.Users.FirstOrDefault(u => u.DoctorID == _id);
                                    if (user != null)
                                    {
                                        user.Username = Username;
                                        user.Password = Password;
                                    }
                                    context.SaveChanges();
                                    OnLogEvent("Оновлено особисту інформацію лікаря", "Employee");
                                    OnLogEvent("Оновлено професійну інформацію лікаря", "Professional");
                                    OnLogEvent("Оновлено дані акаунта лікаря", "Users");
                                    DataUpdated?.Invoke();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;
                case "Видалити лікаря":
                    try
                    {
                        using (var context = new PrivateDoctorsDBEntities1())
                        {
                            if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                                context.Database.Connection.Open();
                            if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                var employee = context.Employees.FirstOrDefault(e => e.ID == _id);
                                if (employee != null)
                                {
                                    var doctorServices = context.DoctorServices.Where(ds => ds.DoctorID == _id).ToList();
                                    context.DoctorServices.RemoveRange(doctorServices);
                                    var schedules = context.Schedules.Where(s => s.DoctorID == _id).ToList();
                                    context.Schedules.RemoveRange(schedules);
                                    var notifications = context.Notifications.Where(n => n.DoctorID == _id).ToList();
                                    context.Notifications.RemoveRange(notifications);
                                    var appointments = context.Appointments.Where(a => a.DoctorID == _id).ToList();
                                    context.Appointments.RemoveRange(appointments);
                                    var professional = context.Professionals.FirstOrDefault(p => p.EmployeeID == _id);
                                    if (professional != null)
                                        context.Professionals.Remove(professional);

                                    var user = context.Users.FirstOrDefault(u => u.DoctorID == _id);
                                    var logs = context.Logs.Where(l => l.UserID == user.ID).ToList();
                                    context.Logs.RemoveRange(logs);
                                    if (user != null)
                                        context.Users.Remove(user);

                                    context.Employees.Remove(employee);
                                    context.SaveChanges();
                                    OnLogEvent("Видалено особисту інформацію лікаря", "Employee");
                                    OnLogEvent("Видалено професійну інформацію лікаря", "Professional");
                                    OnLogEvent("Видалено дані акаунта лікаря", "Users");
                                    OnLogEvent("Видалено послугу у лікаря", "DoctorServices");
                                    DataUpdated?.Invoke();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;
            }
            _window.Close();
        }
    }
}

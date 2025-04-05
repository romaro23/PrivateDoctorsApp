using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Admin;
using PrivateDoctorsApp.View.Doctor;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class ChangeDoctorViewModel : INotifyPropertyChanged
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

        public ICommand ValidateLastNameCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(LastName))
                ShowWarning("Прізвище не може бути порожнім.");
        });
        public ICommand ValidateFirstNameCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                ShowWarning("Ім'я не може бути порожнім.");
        });
        public ICommand ValidateMiddleNameCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(MiddleName))
                ShowWarning("По-батькові не може бути порожнім.");
        });
        public ICommand ValidateEmailCommand => new RelayCommand(_ =>
        {
            if (!Regex.IsMatch(Email ?? "", @"^[\w\.-]+@[\w\.-]+\.\w+$"))
                ShowWarning("Невірний формат Email.");
        });

        public ICommand ValidatePhoneCommand => new RelayCommand(_ =>
        {
            if (!Regex.IsMatch(Phone ?? "", @"^\+?\d{10,15}$"))
                ShowWarning("Невірний формат номера телефону.");
        });

        public ICommand ValidateAddressCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(Address))
                ShowWarning("Адреса не може бути порожньою.");
        });

        public ICommand ValidateSpecializationCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(Specialization))
                ShowWarning("Спеціалізація не може бути порожньою.");
        });
        public ICommand ValidateExperienceYearsCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(ExperienceYears))
            {
                ShowWarning("Досвід не може бути порожнім.");
            }
            else if (!Regex.IsMatch(ExperienceYears, @"^\d+$"))
            {
                ShowWarning("Досвід повинен містити лише цифри.");
            }
        });
        public ICommand ValidateEducationCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(Education))
                ShowWarning("Освіта не може бути порожньою.");
        });
        public ICommand ValidateRatingCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(Rating))
            {
                ShowWarning("Рейтинг не може бути порожнім.");
            }
            else if (!decimal.TryParse(Rating, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal e))
            {
                ShowWarning("Рейтинг повинен бути числом з десятковою крапкою (наприклад: 4.5).");
            }
        });
        public ICommand ValidateUsernameCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(Username))
                ShowWarning("Логін не може бути порожнім.");
        });
        public ICommand ValidatePasswordCommand => new RelayCommand(_ =>
        {
            if (string.IsNullOrWhiteSpace(Password))
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
            return !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(MiddleName) &&
                   !string.IsNullOrWhiteSpace(Phone) &&
                   !string.IsNullOrWhiteSpace(Specialization) &&
                   !string.IsNullOrWhiteSpace(Education) &&
                   !string.IsNullOrWhiteSpace(ExperienceYears) &&
                   !string.IsNullOrWhiteSpace(Address) &&
                   !string.IsNullOrWhiteSpace(Rating) &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        private AdminDoctorsViewModel.PersonalData _doctor;
        public ChangeDoctorViewModel(ChangeDoctorWindow window, AdminDoctorsViewModel.PersonalData doctor = null)
        {
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
                                var id = context.Employees.FirstOrDefault(e => e.Email == Email).ID;
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
                                    Password = Password
                                };
                                context.Users.Add(newUser);
                                context.SaveChanges();
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
                                    var professional = context.Professionals.FirstOrDefault(p => p.EmployeeID == _id);
                                    if (professional != null)
                                        context.Professionals.Remove(professional);

                                    var user = context.Users.FirstOrDefault(u => u.DoctorID == _id);
                                    if (user != null)
                                        context.Users.Remove(user);

                                    context.Employees.Remove(employee);
                                    context.SaveChanges();
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

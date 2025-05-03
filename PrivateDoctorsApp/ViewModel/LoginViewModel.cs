using System;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using PrivateDoctorsApp.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PrivateDoctorsApp.View;
using PrivateDoctorsApp.View.Doctor;
using PrivateDoctorsApp.View.Patient;
using AdminMainWindow = PrivateDoctorsApp.View.Admin.AdminMainWindow;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace PrivateDoctorsApp.ViewModel
{
    internal class LoginViewModel : LogEventBase, INotifyPropertyChanged
    {
        private string _username = "koval_olena";
        private string _password = "password123";
        private string
            _newUsername,
            _newPassword,
            _firstName,
            _lastName,
            _middleName,
            _email,
            _phone;

        private DateTime? _dateOfBirth;
        [StringLength(3, ErrorMessage = "Username cannot be longer than 3 characters.")]
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string NewUsername
        {
            get => _newUsername;
            set
            {
                _newUsername = value;
                OnPropertyChanged(nameof(NewUsername));
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string MiddleName
        {
            get => _middleName;
            set
            {
                _middleName = value;
                OnPropertyChanged(nameof(MiddleName));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegCommand { get; }
        public ICommand RecoveryCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public LoginViewModel()
        {
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
            LoginCommand = new RelayCommand(ExecuteLogin, CanLogin);
            RegCommand = new RelayCommand(ExecuteReg, CanRegister);
            RecoveryCommand = new RelayCommand(ExecuteRecovery);
            CheckAppointments();
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

        private bool IsValidUsername() =>
            !string.IsNullOrWhiteSpace(Username);

        private bool IsValidPassword() =>
            !string.IsNullOrWhiteSpace(Password);
        private bool IsValidNewUsername() =>
            !string.IsNullOrWhiteSpace(NewUsername);

        private bool IsValidNewPassword() =>
            !string.IsNullOrWhiteSpace(NewPassword);
        private bool IsValidDateOfBirth() =>
            DateOfBirth.HasValue;
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
        public ICommand ValidateNewUsernameCommand => new RelayCommand(_ =>
        {
            if (!IsValidNewUsername())
                ShowWarning("Логін не може бути порожнім.");
        });
        public ICommand ValidateNewPasswordCommand => new RelayCommand(_ =>
        {
            if (!IsValidNewPassword())
                ShowWarning("Пароль не може бути порожнім.");
        });
        public ICommand ValidateDateOfBirthCommand => new RelayCommand(_ =>
        {
            if (!IsValidDateOfBirth())
                ShowWarning("Дата народження не може бути порожньою.");
        });
        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private bool CanRegister()
        {
            return IsValidFirstName() &&
                   IsValidLastName() &&
                   IsValidMiddleName() &&
                   IsValidEmail() &&
                   IsValidPhone() &&
                   IsValidNewUsername() &&
                   IsValidNewPassword() &&
                   IsValidDateOfBirth();
        }

        private bool CanLogin()
        {
            return IsValidUsername() && IsValidPassword();
        }

        private void CheckAppointments()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var today = DateTime.Today;
                        foreach (var appointment in context.Appointments)
                        {
                            if (appointment.AppointmentDate < today && appointment.Status == "pending")
                            {
                                appointment.Status = "confirmed";
                            }
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ExecuteLogin(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var user = context.Users.FirstOrDefault(u => u.Username == Username);
                        if (user != null)
                        {
                            if (user.Password == Password)
                            {
                                MessageBox.Show("Вхід успішний!");
                                Window main = null;
                                switch (user.Role)
                                {
                                    case "patient":
                                        CurrentUser.ID = user.PatientID;
                                        main = new PatientMainWindow();
                                        break;
                                    case "doctor":
                                        CurrentUser.ID = user.DoctorID;
                                        main = new DoctorMainWindow();
                                        break;
                                    case "admin":
                                        CurrentUser.ID = user.ID;
                                        main = new AdminMainWindow();
                                        break;
                                }
                                Application.Current.MainWindow.Close();
                                Application.Current.MainWindow = main;
                                main.Show();

                            }
                            else
                            {
                                MessageBox.Show("Невірний пароль!");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Користувач не знайдений.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void ExecuteReg(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var existingPatient = context.Patients
                            .FirstOrDefault(p => p.ContactNumber == Phone || p.Email == Email);

                        if (existingPatient != null)
                        {
                            MessageBox.Show("Пацієнт з таким телефоном або електронною поштою вже існує.");
                            return;
                        }
                        var existingUser = context.Users.FirstOrDefault(u => u.Username == NewUsername);
                        if (existingUser != null)
                        {
                            MessageBox.Show("Пацієнт з логіном вже існує.");
                            return;
                        }
                        var newPatient = new Model.Patient
                        {
                            LastName = LastName,
                            FirstName = FirstName,
                            MiddleName = MiddleName,
                            BirthDate = DateOfBirth,
                            ContactNumber = Phone,
                            Email = Email
                        };

                        context.Patients.Add(newPatient);
                        context.SaveChanges();
                        var newUser = new User
                        {
                            PatientID = newPatient.ID,
                            Password = NewPassword,
                            Username = NewUsername,
                            Role = "patient"
                        };
                        context.Users.Add(newUser);
                        context.SaveChanges();
                        OnLogEvent("Додано пацієнта", "Patients");
                        OnLogEvent("Додано акаунт пацієнта", "Users");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecuteRecovery(object parameter)
        {
            PasswordRecoveryWindow passwordRecoveryWindow = new PasswordRecoveryWindow();
            passwordRecoveryWindow.Show();
        }
}
}

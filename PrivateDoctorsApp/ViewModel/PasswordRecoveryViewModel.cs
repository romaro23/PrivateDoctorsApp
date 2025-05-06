using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View;

namespace PrivateDoctorsApp.ViewModel
{

    internal class PasswordRecoveryViewModel : LogEventBase, INotifyPropertyChanged
    {
        private string _email;
        private readonly PasswordRecoveryWindow _window;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); ((RelayCommand)RecoveryCommand).RaiseCanExecuteChanged(); }
        }

        public ICommand RecoveryCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public PasswordRecoveryViewModel(PasswordRecoveryWindow window)
        {
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
            RecoveryCommand = new RelayCommand(ExecuteRecovery, CanRecovery);
            _window = window;
        }
        private bool IsValidEmail() =>
            !string.IsNullOrWhiteSpace(Email) &&
            Regex.IsMatch(Email, @"^[\w\.-]+@[\w\.-]+\.\w+$");
        private bool CanRecovery()
        {
            return IsValidEmail();
        }
        private void ExecuteRecovery(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var patient = context.Patients.FirstOrDefault(p => p.Email == Email);

                        if (patient == null)
                        {
                            MessageBox.Show("Користувача з такою електронною поштою не знайдено.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        string newPassword = GenerateRandomPassword();
                        if (SendEmail(Email, newPassword))
                        {
                            UpdateUserPasswordInDatabase(Email, newPassword);
                            MessageBox.Show("Новий пароль надіслано на вашу електронну пошту.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                            _window.Close();
                        }
                        else
                        {
                            MessageBox.Show("Помилка при відправленні листа. Спробуйте ще раз пізніше.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private string GenerateRandomPassword()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[8];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData).Substring(0, 8);
            }
        }

        private bool SendEmail(string toEmail, string newPassword)
        {
            try
            {
                var fromAddress = new MailAddress("romaro2k18@gmail.com", "Support");
                var toAddress = new MailAddress(toEmail);
                const string fromPassword = "tjtt oltk xyof kqyg";
                const string subject = "Password Reset";
                string body = $"Ваш новий пароль: {newPassword}";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                       {
                           Subject = subject,
                           Body = body
                       })
                {
                    smtp.Send(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка відправлення листа: {ex.Message}");
                return false;
            }
        }

        private void UpdateUserPasswordInDatabase(string email, string newPassword)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var patient = context.Patients.FirstOrDefault(p => p.Email == email);
                        var user = context.Users.FirstOrDefault(u => u.PatientID == patient.ID);
                        if (user != null)
                        {
                            user.Password = newPassword;
                            context.SaveChanges();
                            OnLogEvent("Оновлено пароль пацієнта", "Users");
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
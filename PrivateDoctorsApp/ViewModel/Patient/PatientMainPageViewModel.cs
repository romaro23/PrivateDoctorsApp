using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;

namespace PrivateDoctorsApp.ViewModel
{
    internal class PatientMainPageViewModel : LogEventBase, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _cardNumber, _CVV, _date;
        private decimal _amount;

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (value == _amount) return;
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }

        public string CardNumber
        {
            get => _cardNumber;
            set
            {
                if (value == _cardNumber) return;
                _cardNumber = value;
                OnPropertyChanged(nameof(CardNumber));
            }
        }

        public string CVV
        {
            get => _CVV;
            set
            {
                if (value == _CVV) return;
                _CVV = value;
                OnPropertyChanged(nameof(CVV));
            }
        }

        public string Date
        {
            get => _date;
            set
            {
                if (value == _date) return;
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        private bool IsValidCardNumber() =>
            !string.IsNullOrWhiteSpace(CardNumber) && CardNumber.All(char.IsDigit) && CardNumber.Length is 16;
        private bool IsValidCVV() =>
            !string.IsNullOrWhiteSpace(CVV) && CVV.All(char.IsDigit) && (CVV.Length == 3 || CVV.Length == 4);
        private bool IsValidDate() =>
            DateTime.TryParseExact(Date, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
            && parsedDate > DateTime.Now;
        private bool IsValidAmount() =>
            !string.IsNullOrWhiteSpace(Amount.ToString()) && Amount > 0 &&
            Regex.IsMatch(Amount.ToString(), @"^\d+$");
        public ICommand ValidateCardNumberCommand => new RelayCommand(_ =>
        {
            if (!IsValidCardNumber())
                ShowWarning("Невірний формат номера карти.");
        });
        public ICommand ValidateCVVCommand => new RelayCommand(_ =>
        {
            if (!IsValidCVV())
                ShowWarning("Невірний формат CVV.");
        });
        public ICommand ValidateDateCommand => new RelayCommand(_ =>
        {
            if (!IsValidDate())
                ShowWarning("Некоректна дата.");
        });
        public ICommand ValidateAmountCommand => new RelayCommand(_ =>
        {
            if (!IsValidAmount())
                ShowWarning("Сума повинна бути числом.");
        });
        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public class Appointment
        {
            public string DoctorName { get; set; }
            public string Service { get; set; }
            public DateTime? Date { get; set; }
            public string Status { get; set; }
            public string FormattedDate
            {
                get
                {
                    return Date.HasValue ? Date.Value.ToString("d MMMM, HH:mm", new CultureInfo("uk-UA")) : "";
                }
            }

        }

        public class Payment
        {
            public DateTime? Date { get; set; }
            public decimal? Amount { get; set; }
            public string FormattedDate
            {
                get
                {
                    return Date.HasValue ? Date.Value.ToString("d MMMM", new CultureInfo("uk-UA")) : "";
                }
            }

            public string FormattedAmount
            {
                get
                {
                    return Amount.ToString() + " грн";
                }
            }
        }
        
        public ObservableCollection<Appointment> Appointments { get; set; }

        public ObservableCollection<Payment> Payments { get; set; }
        public ICommand MakeDepositCommand { get; }
        public event Action DataUpdated;
        private bool CanDeposit()
        {
            return IsValidCardNumber() && IsValidCVV() && IsValidDate() && IsValidAmount();
        }
        public PatientMainPageViewModel()
        {
            LoadAppointments();
            LoadPayments();
            MakeDepositCommand = new RelayCommand(ExecuteMakeDeposit, CanDeposit);
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
        }

        private void ExecuteMakeDeposit(object parameter)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var patient = context.Patients.FirstOrDefault(p => p.ID == CurrentUser.ID);

                        if (patient != null)
                        {
                            patient.AccountBalance += Amount;
                            var payment = new Model.Payment
                            {
                                Amount = Amount,
                                DateOfPayment = DateTime.Now,
                                PatientID = CurrentUser.ID
                            };
                            context.Payments.Add(payment);
                            context.SaveChanges();
                            OnLogEvent("Додано оплату", "Payments");
                            DataUpdated?.Invoke();
                            LoadPayments();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadPayments()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Payments = new ObservableCollection<Payment>(
                            (from p in context.Payments
                                where p.PatientID == CurrentUser.ID
                                orderby p.DateOfPayment descending 
                                select new Payment
                                {
                                    Date = p.DateOfPayment,
                                    Amount = p.Amount
                                })
                            .Take(3)
                            .ToList()
                        );
                        OnPropertyChanged(nameof(Payments));
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
                        Appointments = new ObservableCollection<Appointment>(
                            (from a in context.Appointments
                                join d in context.Employees on a.DoctorID equals d.ID
                                join s in context.Services on a.ServiceID equals s.ID
                                where a.PatientID == CurrentUser.ID
                                orderby a.AppointmentDate descending 
                                select new Appointment
                                {
                                    DoctorName = d.FirstName + " " + d.LastName,
                                    Service = s.ServiceName,
                                    Date = a.AppointmentDate,
                                    Status = a.Status
                                })
                            .Take(3)
                            .ToList()
                        );
                    }
                }
                Appointments = new ObservableCollection<Appointment>(
                    Appointments.Select(a =>
                    {
                        a.Status = CurrentUser.Status[a.Status];
                        return a;
                    })
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

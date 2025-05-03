using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Admin;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class ChangeServiceViewModel : LogEventBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ChangeServiceWindow _window;
        private AdminServicesViewModel.ServiceItem _service;
        public event Action DataUpdated;
        private int? _id;
        private string _serviceName, price, duration;

        public string ServiceName
        {
            get => _serviceName;
            set
            {
                if (value == _serviceName) return;
                _serviceName = value;
                OnPropertyChanged(nameof(ServiceName));
            }
        }

        public string Price
        {
            get => price;
            set
            {
                if (value == price) return;
                price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public string Duration
        {
            get => duration;
            set
            {
                if (value == duration) return;
                duration = value;
                OnPropertyChanged(nameof(Duration));
            }
        }
        private bool IsValidServiceName() =>
            !string.IsNullOrWhiteSpace(ServiceName);
        private bool IsValidPrice() =>
            !string.IsNullOrWhiteSpace(Price) &&
            decimal.TryParse(Price, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsedPrice) && parsedPrice > 0;
        private bool IsValidDuration() =>
            !string.IsNullOrWhiteSpace(Duration) &&
            int.TryParse(Duration, out var parsedDuration) && parsedDuration > 0;
        public ICommand ValidateServiceNameCommand => new RelayCommand(_ =>
        {
            if (!IsValidServiceName())
                ShowWarning("Послуга не може бути порожньою.");
        });
        public ICommand ValidatePriceCommand => new RelayCommand(_ =>
        {
            if (!IsValidPrice())
                ShowWarning("Ціна не може бути порожньою і повинна бути числом більше нуля.");
        });
        public ICommand ValidateDurationCommand => new RelayCommand(_ =>
        {
            if (!IsValidDuration())
                ShowWarning("Тривалість не може бути порожньою і повинна бути числом більше нуля.");
        });
        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public ICommand ChangeServiceCommand { get; }
        private bool CanChange()
        {
            return IsValidServiceName() &&
                   IsValidPrice() &&
                   IsValidDuration();
        }
        public ChangeServiceViewModel(ChangeServiceWindow window, AdminServicesViewModel.ServiceItem service = null)
        {
            LogEvent += (sender, action, tableName) => CurrentUser.AddLog(action, tableName);
            _window = window;
            _service = service;
            ChangeServiceCommand = new RelayCommand(ExecuteChange, CanChange);
            if (_window.Title == "Оновити послугу" || _window.Title == "Видалити послугу")
            {
                FitData();
                _id = _service.ID;
            }
        }
        private void FitData()
        {
            ServiceName = _service.ServiceName;
            Duration = _service.Duration.ToString();
            Price = _service.Price.ToString();
        }
        private void ExecuteChange(object obj)
        {
            var windowName = _window.Title;
            switch (windowName)
            {
                case "Додати послугу":
                    try
                    {
                        using (var context = new PrivateDoctorsDBEntities1())
                        {
                            if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                                context.Database.Connection.Open();
                            if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                var price = decimal.Parse(Price);
                                var duration = int.Parse(Duration);
                                var newService = new Model.Service
                                {
                                    ServiceName = ServiceName,
                                    Price = price,
                                    DurationMinutes = duration
                                };
                                context.Services.Add(newService);
                                context.SaveChanges();
                                OnLogEvent("Додано послугу", "Services");
                                DataUpdated?.Invoke();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    break;
                case "Оновити послугу":
                    try
                    {
                        using (var context = new PrivateDoctorsDBEntities1())
                        {
                            if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                                context.Database.Connection.Open();
                            if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                var service = context.Services.FirstOrDefault(s => s.ID == _id);
                                if (service != null)
                                {
                                    service.ServiceName = ServiceName;
                                    service.DurationMinutes = int.Parse(Duration);
                                    service.Price = decimal.Parse(Price);
                                    context.SaveChanges();
                                    OnLogEvent("Оновлено послугу", "Services");
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
                case "Видалити послугу":
                    try
                    {
                        using (var context = new PrivateDoctorsDBEntities1())
                        {
                            if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                                context.Database.Connection.Open();
                            if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                            {
                                var service = context.Services.FirstOrDefault(s => s.ID == _id);
                                if (service != null)
                                {
                                    context.Services.Remove(service);
                                    context.SaveChanges();
                                    OnLogEvent("Видалено послугу", "Services");
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

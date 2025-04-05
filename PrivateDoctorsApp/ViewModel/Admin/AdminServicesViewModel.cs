using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Admin;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class AdminServicesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class ServiceItem
        {
            public int? ID { get; set; }
            public string ServiceName { get; set; }
            public decimal? Price { get; set; }
            public int? Duration { get; set; }
        }

        private ObservableCollection<ServiceItem> _services;

        public ObservableCollection<ServiceItem> Services
        {
            get { return _services; }
            set
            {
                _services = value;
                OnPropertyChanged(nameof(Services));
            }
        }
        private ObservableCollection<int?> _ids;

        public ObservableCollection<int?> IDs
        {
            get => _ids;
            set
            {
                if (Equals(value, _ids)) return;
                _ids = value;
                OnPropertyChanged(nameof(IDs));
            }
        }

        private int? _id;

        public int? ID
        {
            get => _id;
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        public ICommand OpenAddServiceWindowCommand { get; }
        public ICommand OpenChangeServiceWindowCommand { get; }
        public ICommand OpenDeleteServiceWindowCommand { get; }
        private bool CanChangeOrDelete()
        {
            return ID.HasValue;
        }
        public AdminServicesViewModel()
        {
            OpenAddServiceWindowCommand = new RelayCommand(OpenAddServiceWindow);
            OpenChangeServiceWindowCommand = new RelayCommand(OpenChangeServiceWindow, CanChangeOrDelete);
            OpenDeleteServiceWindowCommand = new RelayCommand(OpenDeleteServiceWindow, CanChangeOrDelete);
            LoadServices();
        }
        private void OpenAddServiceWindow(object parameter)
        {
            ChangeServiceWindow window = new ChangeServiceWindow
            {
                Title = "Додати послугу"
            };
            var changeServiceViewModel = new ChangeServiceViewModel(window);
            window.DataContext = changeServiceViewModel;
            changeServiceViewModel.DataUpdated += () => LoadServices();
            window.Show();
        }

        private void OpenChangeServiceWindow(object parameter)
        {
            ChangeServiceWindow window = new ChangeServiceWindow
            {
                Title = "Оновити послугу"
            };
            ServiceItem service = Services.FirstOrDefault(s => s.ID == ID);
            var changeServiceViewModel = new ChangeServiceViewModel(window, service);
            window.DataContext = changeServiceViewModel;
            changeServiceViewModel.DataUpdated += () => LoadServices();
            window.Show();
        }
        private void OpenDeleteServiceWindow(object parameter)
        {
            ChangeServiceWindow window = new ChangeServiceWindow
            {
                Title = "Видалити послугу"
            };
            ServiceItem doctor = Services.FirstOrDefault(s => s.ID == ID);
            var changeServiceViewModel = new ChangeServiceViewModel(window, doctor);
            window.DataContext = changeServiceViewModel;
            changeServiceViewModel.DataUpdated += () => LoadServices();
            window.Show();
        }
        private void LoadServices()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Services = new ObservableCollection<ServiceItem>(
                            (from s in context.Services
                                select new ServiceItem
                                {
                                    ID = s.ID,
                                    ServiceName = s.ServiceName,
                                    Price = s.Price,
                                    Duration = s.DurationMinutes
                                }
                            ).ToList()
                        );
                        IDs = new ObservableCollection<int?>(Services.Select(s => s.ID).ToList());
                        OnPropertyChanged(nameof(Services));
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

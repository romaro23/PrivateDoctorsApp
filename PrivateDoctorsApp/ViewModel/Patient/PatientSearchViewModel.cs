using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using static PrivateDoctorsApp.ViewModel.DoctorScheduleViewModel;
using static PrivateDoctorsApp.ViewModel.PatientMainPageViewModel;

namespace PrivateDoctorsApp.ViewModel.Patient
{
    internal class PatientSearchViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class DoctorItem
        {
            public string DoctorName { get; set; }
            public string Specialization { get; set; }
            public string Address { get; set; }
            public decimal? Rating { get; set; }
        }
        private ObservableCollection<DoctorItem> _doctors;
        public ObservableCollection<DoctorItem> Doctors
        {
            get { return _doctors; }
            set
            {
                _doctors = value;
                OnPropertyChanged(nameof(Doctors));
            }
        }

        public class ServiceItem
        {
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
        private ObservableCollection<string> _specializations;
        public ObservableCollection<string> Specializations
        {
            get => _specializations;
            set
            {
                _specializations = value;
                OnPropertyChanged(nameof(Specializations));
            }
        }
        private ObservableCollection<string> _addresses;
        public ObservableCollection<string> Addresses
        {
            get => _addresses;
            set
            {
                _addresses = value;
                OnPropertyChanged(nameof(Addresses));
            }
        }
        private string _specialization;
        public string Specialization
        {
            get => _specialization;
            set
            {
                _specialization = value;
                OnPropertyChanged(Specialization);
            }
        }
        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(Address);
            }
        }

        private bool _sortByRating;

        public bool SortByRating
        {
            get => _sortByRating;
            set
            {
                _sortByRating = value;
                OnPropertyChanged(nameof(SortByRating));
            }
        }

        private int? _maxPrice;

        public int? MaxPrice
        {
            get => _maxPrice;
            set
            {
                _maxPrice = value;
                OnPropertyChanged(nameof(MaxPrice));
            }
        }
        private int? _minPrice;

        public int? MinPrice
        {
            get => _minPrice;
            set
            {
                _minPrice = value;
                OnPropertyChanged(nameof(MinPrice));
            }
        }
        private int? _price;

        public int? Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
        public ICommand ClearCommand { get; }
        public ICommand SelectedItemChangedCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand PriceCommand { get; }
        public PatientSearchViewModel()
        {
            LoadDoctors();
            LoadServices();
            ClearCommand = new RelayCommand(ExecuteClear);
            SelectedItemChangedCommand = new RelayCommand(ExecuteSelectedItemChanged);
            SortCommand = new RelayCommand(_ =>
            {
                SortByRating = true;
                LoadDoctors(Specialization, Address, SortByRating);
            });
            PriceCommand = new RelayCommand(ExecuteFilterPrice);
        }

        private void ExecuteFilterPrice(object parameter)
        {
            decimal? price = (Price < MinPrice) ? (decimal?)null : Price;
            LoadServices(price);
        }
        private void ExecuteSelectedItemChanged(object parameter)
        {
            LoadDoctors(Specialization, Address, SortByRating);
        }
        private void ExecuteClear(object parameter)
        {
            Specialization = null;
            Address = null;
            SortByRating = false;
            LoadDoctors(Specialization, Address, SortByRating);
        }
        private void LoadServices(decimal? price = null)
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
                                where !price.HasValue || s.Price <= price
                                select new ServiceItem
                                {
                                    ServiceName = s.ServiceName,
                                    Price = s.Price,
                                    Duration = s.DurationMinutes
                                }
                            ).ToList()
                        );
                        MaxPrice = (int)(Services.Select(s => s.Price).DefaultIfEmpty(0).Max());
                        MinPrice = (int)(Services.Select(s => s.Price).DefaultIfEmpty(0).Min());
                        OnPropertyChanged(nameof(Services));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            
        }

        private void LoadDoctors(string specialization = null, string address = null, bool sortByRating = false)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var result = new ObservableCollection<DoctorItem>(
                            (from e in context.Employees
                                join p in context.Professionals on e.ID equals p.EmployeeID
                                where (string.IsNullOrEmpty(specialization) || p.Specialization == specialization) && (string.IsNullOrEmpty(address) || e.Address.Contains(address))
                                select new DoctorItem
                                {
                                    DoctorName = e.LastName + " " + e.FirstName + " " + e.MiddleName,
                                    Address = e.Address,
                                    Rating = p.Rating,
                                    Specialization = p.Specialization
                                }
                            ).ToList()
                        );
                        if (sortByRating)
                        {
                            result = new ObservableCollection<DoctorItem>(result.OrderByDescending(d => d.Rating));
                        }

                        Doctors = new ObservableCollection<DoctorItem>(result);
                        Specializations = new ObservableCollection<string>(Doctors.Select(d => d.Specialization).Distinct().ToList());
                        Addresses = new ObservableCollection<string>(
                            Doctors.Select(d => d.Address.Split(',')[0].Trim()).Distinct().ToList()
                        );
                        OnPropertyChanged(nameof(Doctors));
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

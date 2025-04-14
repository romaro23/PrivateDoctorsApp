using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using PrivateDoctorsApp.View.Admin;
using PrivateDoctorsApp.ViewModel.Doctor;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class AdminDoctorsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class PersonalData : DoctorMainPageViewModel.PersonalData
        {
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
        }
        private ObservableCollection<PersonalData> _doctors;

        public ObservableCollection<PersonalData> Doctors
        {
            get => _doctors;
            set
            {
                if (Equals(value, _doctors)) return;
                _doctors = value;
                OnPropertyChanged(nameof(Doctors));
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

        public ICommand OpenAddDoctorWindowCommand { get; }
        public ICommand OpenChangeDoctorWindowCommand { get; }
        public ICommand OpenDeleteDoctorWindowCommand { get; }
        private bool CanChangeOrDelete()
        {
            return ID.HasValue;
        }
        public AdminDoctorsViewModel()
        {
            OpenAddDoctorWindowCommand = new RelayCommand(OpenAddDoctorWindow);
            OpenChangeDoctorWindowCommand = new RelayCommand(OpenChangeDoctorWindow, CanChangeOrDelete);
            OpenDeleteDoctorWindowCommand = new RelayCommand(OpenDeleteDoctorWindow, CanChangeOrDelete);
            LoadDoctors();
        }
        private void OpenAddDoctorWindow(object parameter)
        {
            ChangeDoctorWindow window = new ChangeDoctorWindow
            {
                Title = "Додати лікаря"
            };
            var changeDoctorViewModel = new ChangeDoctorViewModel(window);
            window.DataContext = changeDoctorViewModel;
            changeDoctorViewModel.DataUpdated += () => LoadDoctors();
            window.Show();
        }

        private void OpenChangeDoctorWindow(object parameter)
        {
            ChangeDoctorWindow window = new ChangeDoctorWindow
            {
                Title = "Оновити дані"
            };
            PersonalData doctor = Doctors.FirstOrDefault(d => d.ID == ID);
            var changeDoctorViewModel = new ChangeDoctorViewModel(window, doctor);
            window.DataContext = changeDoctorViewModel;
            changeDoctorViewModel.DataUpdated += () => LoadDoctors();
            window.Show();
        }
        private void OpenDeleteDoctorWindow(object parameter)
        {
            ChangeDoctorWindow window = new ChangeDoctorWindow
            {
                Title = "Видалити лікаря"
            };
            PersonalData doctor = Doctors.FirstOrDefault(d => d.ID == ID);
            var changeDoctorViewModel = new ChangeDoctorViewModel(window, doctor);
            window.DataContext = changeDoctorViewModel;
            changeDoctorViewModel.DataUpdated += () => LoadDoctors();
            window.Show();
        }
        private void LoadDoctors()
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        Doctors = new ObservableCollection<PersonalData>((from e in context.Employees
                                join p in context.Professionals on e.ID equals p.EmployeeID
                                join u in context.Users on e.ID equals u.DoctorID into userGroup
                                from u in userGroup.DefaultIfEmpty()
                                select new PersonalData
                                {
                                    ID = e.ID,
                                    LastName = e.LastName,
                                    FirstName = e.FirstName,
                                    MiddleName = e.MiddleName,
                                    ContactNumber = e.ContactNumber,
                                    Email = e.Email,
                                    Address = e.Address,
                                    Username = u != null ? u.Username : null,
                                    Password = u != null ? u.Password : null,
                                    Specialization = p.Specialization,
                                    ExperienceYears = p.ExperienceYears,
                                    Education = p.Education,
                                    Rating = p.Rating
                                }).ToList()
                        );
                        IDs = new ObservableCollection<int?>(Doctors.Select(d => d.ID).ToList());
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

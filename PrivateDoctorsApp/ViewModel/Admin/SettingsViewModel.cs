using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using PrivateDoctorsApp.Model;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand SelectPathCommand { get; }
        private string _path = CurrentUser.Path;

        public string Path
        {
            get => _path;
            set
            {
                if (value == _path) return;
                _path = value;
                OnPropertyChanged(nameof(Path));
            }
        }
        private DateTime? _startDate = CurrentUser.PeriodStart, _endDate = CurrentUser.PeriodEnd;

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (Nullable.Equals(value, _startDate)) return;
                _startDate = value;
                CurrentUser.PeriodStart = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (Nullable.Equals(value, _endDate)) return;
                _endDate = value;
                CurrentUser.PeriodEnd = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public SettingsViewModel()
        {
            SelectPathCommand = new RelayCommand(ExecuteSelectPath);
        }

        private void ExecuteSelectPath(object parameter)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Path = dialog.SelectedPath;
                    CurrentUser.Path = Path;
                }
            }
        }
    }
}

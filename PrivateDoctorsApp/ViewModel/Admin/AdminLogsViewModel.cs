using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PrivateDoctorsApp.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class AdminLogsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Model.Log> _logs;

        public ObservableCollection<Log> Logs
        {
            get => _logs;
            set
            {
                if (Equals(value, _logs)) return;
                _logs = value;
                OnPropertyChanged(nameof(Logs));
            }
        }

        private string _action, _tableName;
        private int? _userID;
        private DateTime? _date;
        private ObservableCollection<string> _actions, _tableNames;
        private ObservableCollection<int?> _userIDs;
        private ObservableCollection<DateTime?> _dates;

        public string Action
        {
            get => _action;
            set
            {
                if (value == _action) return;
                _action = value;
                OnPropertyChanged(nameof(Action));
            }
        }

        public string TableName
        {
            get => _tableName;
            set
            {
                if (value == _tableName) return;
                _tableName = value;
                OnPropertyChanged(nameof(TableName));
            }
        }

        public int? UserID
        {
            get => _userID;
            set
            {
                if (value == _userID) return;
                _userID = value;
                OnPropertyChanged(nameof(UserID));
            }
        }

        public DateTime? Date
        {
            get => _date;
            set
            {
                if (Nullable.Equals(value, _date)) return;
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public ObservableCollection<string> Actions
        {
            get => _actions;
            set
            {
                if (Equals(value, _actions)) return;
                _actions = value;
                OnPropertyChanged(nameof(Actions));
            }
        }

        public ObservableCollection<string> TableNames
        {
            get => _tableNames;
            set
            {
                if (Equals(value, _tableNames)) return;
                _tableNames = value;
                OnPropertyChanged(nameof(TableNames));
            }
        }

        public ObservableCollection<int?> UserIDs
        {
            get => _userIDs;
            set
            {
                if (Equals(value, _userIDs)) return;
                _userIDs = value;
                OnPropertyChanged(nameof(UserIDs));
            }
        }

        public ObservableCollection<DateTime?> Dates
        {
            get => _dates;
            set
            {
                if (Equals(value, _dates)) return;
                _dates = value;
                OnPropertyChanged(nameof(Dates));
            }
        }
        public ICommand SelectedItemChangedCommand { get; }
        public ICommand ClearCommand { get; }
        public AdminLogsViewModel()
        {
            LoadLogs();
            SelectedItemChangedCommand = new RelayCommand(ExecuteSelectedItemChanged);
            ClearCommand = new RelayCommand(ExecuteClear);
        }
        private void ExecuteClear(object parameter)
        {
            UserID = null;
            Action = null;
            TableName = null;
            Date = null;
        }

        private void ExecuteSelectedItemChanged(object parameter)
        {
            LoadLogs(UserID, Action, TableName, Date);
        }

        private void LoadLogs(int? userID = null, string action = null, string tableName = null, DateTime? date = null)
        {
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        var logs = context.Logs
                            .Where(l =>
                                (!userID.HasValue || l.UserID == userID) &&
                                (string.IsNullOrEmpty(action) || l.Action == action) &&
                                (string.IsNullOrEmpty(tableName) || l.TableName == tableName) &&
                                (!date.HasValue || DbFunctions.TruncateTime(l.Date) == date))
                            .ToList();

                        Logs = new ObservableCollection<Log>(logs);
                        Actions = new ObservableCollection<string>(logs.Select(l => l.Action).Distinct());
                        TableNames = new ObservableCollection<string>(logs.Select(l => l.TableName).Distinct());
                        UserIDs = new ObservableCollection<int?>(logs.Select(l => l.UserID).Distinct());
                        Dates = new ObservableCollection<DateTime?>(logs.Select(l => l.Date).Distinct());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при з'єднанні з БД: " + ex.Message, "Попередження",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

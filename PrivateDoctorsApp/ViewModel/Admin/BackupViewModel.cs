using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PrivateDoctorsApp.Model;

namespace PrivateDoctorsApp.ViewModel.Admin
{
    internal class BackupViewModel
    {
        public ICommand SaveDatabaseCommand { get; }
        public ICommand RestoreDatabaseCommand { get; }
        public ICommand ChangePeriodDatabaseCommand { get; }

        public BackupViewModel()
        {
            SaveDatabaseCommand = new RelayCommand(ExecuteSaveDatabase);
            RestoreDatabaseCommand = new RelayCommand(ExecuteRestoreDatabase);
            ChangePeriodDatabaseCommand = new RelayCommand(ExecuteChangePeriodDatabase);
        }

        private void ExecuteSaveDatabase(object parameter)
        {
            if (CurrentUser.Path == "")
            {
                System.Windows.MessageBox.Show("Будь-ласка, встановіть шлях для збереження бази даних у вікні Налаштування.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string path = CurrentUser.Path + "\\PrivateDoctors.bak";
            string query = $"BACKUP DATABASE [PrivateDoctorsDB] TO DISK='{path}'";
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, query);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Помилка під час збереження бази даних:\n" + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteRestoreDatabase(object parameter)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Backup files (*.bak)|*.bak";
                dialog.Title = "Оберіть .bak файл для відновлення бази даних";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFilePath = dialog.FileName;

                    if (!selectedFilePath.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
                    {
                        System.Windows.MessageBox.Show("Оберіть файл з розширенням .bak", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        string restoreQuery = $@"
                USE master;
                ALTER DATABASE [PrivateDoctorsDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE [PrivateDoctorsDB] 
                FROM DISK = N'{selectedFilePath}' 
                WITH REPLACE;
                ALTER DATABASE [PrivateDoctorsDB] SET MULTI_USER;";

                        using (var context = new PrivateDoctorsDBEntities1())
                        {
                            context.Database.ExecuteSqlCommand(
                                System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction,
                                restoreQuery);
                        }

                        System.Windows.MessageBox.Show("Базу даних успішно відновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Помилка під час відновлення бази даних:\n" + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ExecuteChangePeriodDatabase(object parameter)
        {
            var result = System.Windows.MessageBox.Show("Увага! Всі дані за вказаний вами період будуть видалені, Ви впевнені?", "Попередження", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result != MessageBoxResult.OK)
                return;

            if (CurrentUser.PeriodStart == null || CurrentUser.PeriodEnd == null)
            {
                System.Windows.MessageBox.Show("Будь ласка, вкажіть дату початку і кінця періоду у вікні Налаштування.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    DateTime start = CurrentUser.PeriodStart.Value.Date;
                    DateTime end = CurrentUser.PeriodEnd.Value.Date.AddDays(1).AddTicks(-1);

                    var notificationsToDelete = context.Notifications
                        .Where(n => n.DateOfCreation >= start && n.DateOfCreation <= end);
                    context.Notifications.RemoveRange(notificationsToDelete);

                    var paymentsToDelete = context.Payments
                        .Where(p => p.DateOfPayment >= start && p.DateOfPayment <= end);
                    context.Payments.RemoveRange(paymentsToDelete);

                    var appointmentsToDelete = context.Appointments
                        .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end);
                    context.Appointments.RemoveRange(appointmentsToDelete);

                    var schedulesToDelete = context.Schedules
                        .Where(s => s.WorkDate >= start && s.WorkDate <= end);
                    context.Schedules.RemoveRange(schedulesToDelete);

                    context.SaveChanges();

                    System.Windows.MessageBox.Show("Дані успішно очищено за вказаний період.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Помилка при видаленні даних:\n" + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

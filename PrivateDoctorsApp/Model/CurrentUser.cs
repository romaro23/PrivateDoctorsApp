using System;
using System.Collections.Generic;
using System.Windows;

namespace PrivateDoctorsApp.Model
{
    public static class CurrentUser
    {
        public static int? ID {  get; set; }
        public static Dictionary<string, string> Status = new Dictionary<string, string>
        {
            { "pending", "Заплановано"},
            { "cancelled", "Скасовано"},
            { "confirmed", "Завершено"}

        };

        public static string Path { get; set; } = "";
        public static DateTime? PeriodStart { get; set; }
        public static DateTime? PeriodEnd{ get; set; }

        public static void AddLog(string action, string tableName)
        {

            try
            {
                using (var context = new PrivateDoctorsDBEntities1())
                {
                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        context.Database.Connection.Open();
                    if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        if (ID != null)
                        {
                            var newLog = new Model.Log
                            {
                                UserID = (int)ID,
                                Action = action,
                                TableName = tableName,
                                Date = DateTime.Now
                            };
                            context.Logs.Add(newLog);
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
    }
}

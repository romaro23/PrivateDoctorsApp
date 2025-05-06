using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
namespace PrivateDoctorsApp.Model
{
    public static class CurrentUser
    {
        public static int? ID {  get; set; }
        public static string Role { get; set; }
        public static Dictionary<string, string> Status = new Dictionary<string, string>
        {
            { "pending", "Заплановано"},
            { "confirmed", "Завершено"}

        };
        public static decimal Balance { get; set; }
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
                            User user;
                            if (Role == "patient")
                            {
                                user = context.Users.FirstOrDefault(u => u.DoctorID == null && u.PatientID == ID);
                            }

                            else if (Role == "doctor")
                            {
                                user = context.Users.FirstOrDefault(u => u.DoctorID == ID && u.PatientID == null);
                            }

                            else
                            {
                                user = context.Users.FirstOrDefault(u => u.DoctorID == null && u.PatientID == null);
                            }
                            var newLog = new Model.Log
                            {
                                UserID = user.ID,
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

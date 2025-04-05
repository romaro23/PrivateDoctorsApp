using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using PrivateDoctorsApp.Model;

namespace PrivateDoctorsApp.ViewModel
{
    internal class PatientMainPageViewModel
    {
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

        public PatientMainPageViewModel()
        {
            LoadAppointments();
            LoadPayments();
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
                                orderby p.DateOfPayment
                                select new Payment
                                {
                                    Date = p.DateOfPayment,
                                    Amount = p.Amount
                                })
                            .Take(3)
                            .ToList()
                        );
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
                                orderby a.AppointmentDate
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

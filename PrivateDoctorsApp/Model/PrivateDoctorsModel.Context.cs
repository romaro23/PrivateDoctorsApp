﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrivateDoctorsApp.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PrivateDoctorsDBEntities1 : DbContext
    {
        public PrivateDoctorsDBEntities1()
            : base("name=PrivateDoctorsDBEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<DoctorService> DoctorServices { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Professional> Professionals { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
    }
}

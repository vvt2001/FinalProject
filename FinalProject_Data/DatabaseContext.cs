using FinalProject_Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace FinalProject_Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Meeting
            modelBuilder.ApplyConfiguration(new Meeting_configuration());
            modelBuilder.ApplyConfiguration(new MeetingForm_configuration());
            modelBuilder.ApplyConfiguration(new MeetingTime_configuration());

            //Attendee
            modelBuilder.ApplyConfiguration(new Attendee_configuration());
            modelBuilder.ApplyConfiguration(new Attendee_Meeting_configuration());
            modelBuilder.ApplyConfiguration(new Attendee_MeetingForm_configuration());

            //User
            modelBuilder.ApplyConfiguration(new User_configuration());
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);   
        }
        public DbSet<User> users { get; set; }
        public DbSet<Meeting> meetings { get; set; }
        public DbSet<MeetingForm> meetingforms { get; set; }
        public DbSet<MeetingTime> meetingtimes { get; set; }
        public DbSet<Attendee> attendees { get; set; }
        public DbSet<Attendee_Meeting> attendee_meetings { get; set; }
        public DbSet<Attendee_MeetingForm> attendee_meetingforms { get; set; }

    }
}

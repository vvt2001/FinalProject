using FinalProject_Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject_Data.Model
{
    public class Attendee : Attendee_properties
    {
        public MeetingForm? meetingform { get; set; }
        public Meeting? meeting { get; set; }
    }
    public class Attendee_properties : Entity
    {
        public string name { get; set; }
        public string email { get; set; }
        public string? meetingform_id { get; set; }
        public string? meeting_id { get; set; }
    }
    public class Attendee_configuration : IEntityTypeConfiguration<Attendee>
    {
        public void Configure(EntityTypeBuilder<Attendee> builder)
        {
            builder.HasOne(a => a.meetingform).WithMany(mf => mf.attendee).HasForeignKey(mt => mt.meetingform_id);
            builder.HasOne(a => a.meeting).WithMany(m => m.attendees).HasForeignKey(mt => mt.meeting_id);
        }
    }
}
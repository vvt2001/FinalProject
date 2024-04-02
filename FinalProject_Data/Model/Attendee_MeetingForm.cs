using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject_Data.Model
{
    public class Attendee_MeetingForm : Attendee_MeetingForm_properties
    {
        public Attendee attendee { get; set; }
        public MeetingForm meetingform { get; set; }
    }
    public class Attendee_MeetingForm_properties
    {
        public string attendee_id { get; set; }
        public string meetingform_id { get; set; }
    }
    public class Attendee_MeetingForm_configuration : IEntityTypeConfiguration<Attendee_MeetingForm>
    {
        public void Configure(EntityTypeBuilder<Attendee_MeetingForm> builder)
        {
            builder.HasKey(o => new { o.attendee_id, o.meetingform_id });
            builder.HasOne(a_m => a_m.attendee).WithMany().HasForeignKey(a_m => a_m.attendee_id);
            builder.HasOne(a_m => a_m.meetingform).WithMany(mf => mf.attendee_meetingforms).HasForeignKey(a_m => a_m.meetingform_id);
        }
    }
}

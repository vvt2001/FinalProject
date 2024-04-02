using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject_Data.Model
{
    public class Attendee_Meeting : Attendee_Meeting_properties
    {
        public Attendee attendee { get; set; }
        public Meeting meeting { get; set; }
    }
    public class Attendee_Meeting_properties
    {
        public string attendee_id { get; set; }
        public string meeting_id { get; set; }
    }
    public class Attendee_Meeting_configuration : IEntityTypeConfiguration<Attendee_Meeting>
    {
        public void Configure(EntityTypeBuilder<Attendee_Meeting> builder)
        {
            builder.HasKey(o => new { o.attendee_id, o.meeting_id });
            builder.HasOne(a_m => a_m.attendee).WithMany().HasForeignKey(a_m => a_m.attendee_id);
            builder.HasOne(a_m => a_m.meeting).WithMany(m => m.attendee_meetings).HasForeignKey(a_m => a_m.meeting_id);
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalProject_Data.Model
{
    public class Meeting : Meeting_properties
    {
        public ICollection<Attendee>? attendees { get; set; }
        public User owner { get; set; }
    }
    public class Meeting_properties : Entity
    {
        public DateTime starttime { get; set; }
        public int duration { get; set; }
        public string meeting_title { get; set; }
        public string? meeting_description { get; set; }
        public string? location { get; set; }
        public int platform { get; set; }
        public int trangthai { get; set; }
        public string owner_id { get; set; }
        public string? meeting_link { get;set; }
        public string? event_id { get;set; }
        public string? meetingform_id { get; set; }
        public bool? is_active { get; set; } = true;
        public string? note { get; set; }
    }
    public class Meeting_configuration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.HasOne(m => m.owner).WithMany().HasForeignKey(m => m.owner_id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

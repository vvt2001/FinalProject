﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject_Data.Model
{
    public class MeetingForm : MeetingForm_properties
    {
        public ICollection<MeetingTime> times { get; set; }
        public ICollection<Attendee_MeetingForm> attendee_meetingforms { get; set; }
        public User owner { get; set; }
    }
    public class MeetingForm_properties : Entity
    {
        public DateTime? starttime { get; set; }
        public int? duration { get; set; }
        public string URL { get; set; }
        public string meeting_title { get; set; }
        public string meeting_description { get; set; }
        public string location { get; set; }
        public int platform { get; set; }
        public int trangthai { get; set; }
        public string owner_id { get; set; }
    }
    public class MeetingForm_configuration : IEntityTypeConfiguration<MeetingForm>
    {
        public void Configure(EntityTypeBuilder<MeetingForm> builder)
        {
            builder.HasOne(mf => mf.owner).WithMany().HasForeignKey(mf => mf.owner_id);
        }
    }
}
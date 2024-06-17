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
    public class VotingHistory : VotingHistory_properties
    {
        public Attendee attendee { get; set; }
        public MeetingForm meetingform { get; set; }
        public MeetingTime meetingtime { get; set; }
    }

    public class VotingHistory_properties : Entity
    {
        public string attendee_id { get; set; }
        public string meetingform_id { get; set; }
        public string meetingtime_id { get; set; }
    }

    public class VotingHistory_configuration : IEntityTypeConfiguration<VotingHistory>
    {
        public void Configure(EntityTypeBuilder<VotingHistory> builder)
        {
            builder.HasOne(vh => vh.attendee).WithMany(a => a.voting_histories).HasForeignKey(vh => vh.attendee_id).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(vh => vh.meetingform).WithMany(mf => mf.voting_histories).HasForeignKey(vh => vh.meetingform_id).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(vh => vh.meetingtime).WithMany(mt => mt.voting_histories).HasForeignKey(vh => vh.meetingtime_id).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject_Data.Model
{
    public class MeetingTime : MeetingTime_properties
    {
        public MeetingForm meetingform { get; set; }
        public ICollection<VotingHistory> voting_histories { get; set; }
    }
    public class MeetingTime_properties : Entity
    {
        public int vote_count { get; set; } = 0;
        public DateTime time { get; set; }
        public int duration { get; set; }
        public string meetingform_id { get; set; }
        public int trangthai { get; set; }
    }
    public class MeetingTime_configuration : IEntityTypeConfiguration<MeetingTime>
    {
        public void Configure(EntityTypeBuilder<MeetingTime> builder)
        {
            builder.HasOne(mt => mt.meetingform).WithMany(mf => mf.times).HasForeignKey(mt => mt.meetingform_id).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

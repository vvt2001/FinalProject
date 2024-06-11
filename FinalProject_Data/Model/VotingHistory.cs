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
    }

    public class VotingHistory_properties : Entity
    {
        public string name { get; set; }
        public string email { get; set; }
        public string meeting_form_id { get; set; }
        public string meeting_time_id { get; set; }
    }

    public class VotingHistory_configuration : IEntityTypeConfiguration<VotingHistory>
    {
        public void Configure(EntityTypeBuilder<VotingHistory> builder)
        {
        }
    }
}

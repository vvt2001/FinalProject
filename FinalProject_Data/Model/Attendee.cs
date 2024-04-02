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
    }
    public class Attendee_properties : Entity
    {
        public string username { get; set; }
        public string email { get; set; }
    }
    public class Attendee_configuration : IEntityTypeConfiguration<Attendee>
    {
        public void Configure(EntityTypeBuilder<Attendee> builder)
        {
        }
    }
}
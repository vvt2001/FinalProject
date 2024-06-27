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
    public class User : User_properties
    {
    }

    public class User_properties : Entity
    {
        [Required(ErrorMessage = "Họ và tên không được trống")]
        public string name { get; set; }
        [Required(ErrorMessage = "Tên đăng nhập không được trống")]
        public string username { get; set; }
        public string email { get; set; }
        public int trangthai { get; set; }
        public string hash { get; set; }
        public string salt { get; set; }
        public bool has_googlecredentials { get; set; }
    }

    public class User_configuration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(o => o.username).HasMaxLength(200);
            builder.HasIndex(o => o.email).IsUnique();
            builder.HasIndex(o => o.username).IsUnique();
            builder.HasIndex(o => o.name).IsUnique();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinalProject_Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {

            //var connectionString = "Server=VVT;Database=FinalProject_MeetingScheduler;User Id=vvt1508;Password=123456a@;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True";
            var connectionString = "Server=104.197.148.232;Database=FinalProject_MeetingScheduler;User Id=vvt1508;Password=123456a@;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}

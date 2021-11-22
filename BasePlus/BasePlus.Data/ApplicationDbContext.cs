using BasePlus.Common;
using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasePlus.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(DbHelper.SqlConnectionString);
        }
        public DbSet<mockuser> Users { get; set; }
        public DbSet<userinfo> UserInfos { get; set; }
        public DbSet<TestLog> TestLog { get; set; }
        public DbSet<Score> Score { get; set; }
    }
}

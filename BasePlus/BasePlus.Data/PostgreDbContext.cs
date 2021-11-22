using BasePlus.Common;
using BasePlus.Common.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasePlus.Data
{
    public class PostgreDbContext : DbContext
    {
        public PostgreDbContext()
        {

        }
        public PostgreDbContext(DbContextOptions<PostgreDbContext> options)
    : base(options)
        { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(DbHelper.PostgreConnectionString);
        }
        public DbSet<mockuser> Users { get; set; }
        public DbSet<userinfo> UserInfos { get; set; }
    }
}

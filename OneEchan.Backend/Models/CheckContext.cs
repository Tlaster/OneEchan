using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OneEchan.Backend.Models
{
    public class CheckContext : DbContext
    {
        public DbSet<CheckModel> WeiboList { get; set; }
        public DbSet<CheckModel> CheckList { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./check.db");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneEchan.Server.Models;

namespace OneEchan.Server.Data
{
    public class TranscodeDbContext : DbContext
    {
        public DbSet<TranscodeModel> Transcode { get; set; }
        public TranscodeDbContext(DbContextOptions<TranscodeDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using OneEchan.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Category { get; set; }   
        public DbSet<Video> Video { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<CategoryName> CategoryName { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<VideoUrl> VideoUrl { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<SiteUserInfo> SiteUserInfo { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}

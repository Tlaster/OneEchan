using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneEchan.Server.Models
{
    public partial class AnimateDatabaseContext : DbContext
    {
        public AnimateDatabaseContext(DbContextOptions<AnimateDatabaseContext> options)
            : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimateList>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.EnUs)
                    .IsRequired()
                    .HasColumnName("en_US");

                entity.Property(e => e.JaJp).HasColumnName("ja_JP");

                entity.Property(e => e.RuRu).HasColumnName("ru_RU");

                entity.Property(e => e.UpdatedAt).HasColumnName("Updated_At");

                entity.Property(e => e.ZhTw).HasColumnName("zh_TW");
            });

            modelBuilder.Entity<SetDetail>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.SetName })
                    .HasName("PK_SetDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Created_At).HasColumnName("Created_At");

                entity.Property(e => e.FilePath).IsRequired();

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.SetDetail)
                    .HasForeignKey(d => d.Id)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK__SetDetail__ID__117F9D94");
            });
        }

        public virtual DbSet<AnimateList> AnimateList { get; set; }
        public virtual DbSet<SetDetail> SetDetail { get; set; }
    }
}
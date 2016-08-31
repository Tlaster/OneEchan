using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OneEchan.Backend.Models;

namespace OneEchan.Backend.Migrations
{
    [DbContext(typeof(CheckContext))]
    [Migration("20160831041713_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("OneEchan.Backend.Models.CheckModel", b =>
                {
                    b.Property<int>("ID");

                    b.Property<int>("ItemID");

                    b.Property<string>("SetName");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID", "ItemID", "SetName");

                    b.ToTable("CheckList");
                });

            modelBuilder.Entity("OneEchan.Backend.Models.WeiboModel", b =>
                {
                    b.Property<int>("ID");

                    b.Property<int>("ItemID");

                    b.Property<string>("SetName");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ZhTW")
                        .IsRequired();

                    b.HasKey("ID", "ItemID", "SetName");

                    b.ToTable("WeiboList");
                });
        }
    }
}

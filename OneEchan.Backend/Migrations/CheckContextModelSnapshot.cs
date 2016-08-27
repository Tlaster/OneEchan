using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OneEchan.Backend.Models;

namespace OneEchan.Backend.Migrations
{
    [DbContext(typeof(CheckContext))]
    partial class CheckContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("OneEchan.Backend.Models.CheckModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ItemID");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("SetName")
                        .IsRequired();

                    b.Property<string>("ZhTW")
                        .IsRequired();

                    b.HasKey("ID");

                    b.ToTable("CheckModel");
                });
        }
    }
}

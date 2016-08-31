using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OneEchan.Backend.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckList",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    SetName = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckList", x => new { x.ID, x.ItemID, x.SetName });
                });

            migrationBuilder.CreateTable(
                name: "WeiboList",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    SetName = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ZhTW = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeiboList", x => new { x.ID, x.ItemID, x.SetName });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckList");

            migrationBuilder.DropTable(
                name: "WeiboList");
        }
    }
}

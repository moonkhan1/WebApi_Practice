using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi_Practice.Migrations
{
    public partial class Third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "tblStudent",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "tblCourse",
                type: "datetime",
                nullable: true,
                defaultValueSql: "Getdate()",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValue: new DateTime(2023, 7, 24, 18, 31, 54, 467, DateTimeKind.Local).AddTicks(8110));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "tblStudent");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "tblCourse",
                type: "datetime",
                nullable: true,
                defaultValue: new DateTime(2023, 7, 24, 18, 31, 54, 467, DateTimeKind.Local).AddTicks(8110),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "Getdate()");
        }
    }
}

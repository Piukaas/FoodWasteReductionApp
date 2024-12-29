using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodWasteReduction.Infrastructure.Migrations.Application
{
    /// <inheritdoc />
    public partial class RemoveIdentityFromUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AccessFailedCount", table: "Students");

            migrationBuilder.DropColumn(name: "ConcurrencyStamp", table: "Students");

            migrationBuilder.DropColumn(name: "EmailConfirmed", table: "Students");

            migrationBuilder.DropColumn(name: "LockoutEnabled", table: "Students");

            migrationBuilder.DropColumn(name: "LockoutEnd", table: "Students");

            migrationBuilder.DropColumn(name: "NormalizedEmail", table: "Students");

            migrationBuilder.DropColumn(name: "NormalizedUserName", table: "Students");

            migrationBuilder.DropColumn(name: "PasswordHash", table: "Students");

            migrationBuilder.DropColumn(name: "PhoneNumberConfirmed", table: "Students");

            migrationBuilder.DropColumn(name: "SecurityStamp", table: "Students");

            migrationBuilder.DropColumn(name: "TwoFactorEnabled", table: "Students");

            migrationBuilder.DropColumn(name: "UserName", table: "Students");

            migrationBuilder.DropColumn(name: "AccessFailedCount", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "ConcurrencyStamp", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "Email", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "EmailConfirmed", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "LockoutEnabled", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "LockoutEnd", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "Name", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "NormalizedEmail", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "NormalizedUserName", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "PasswordHash", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "PhoneNumber", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "PhoneNumberConfirmed", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "SecurityStamp", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "TwoFactorEnabled", table: "CanteenStaff");

            migrationBuilder.DropColumn(name: "UserName", table: "CanteenStaff");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"
            );

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Students",
                type: "datetimeoffset",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "CanteenStaff",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "CanteenStaff",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "CanteenStaff",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "CanteenStaff",
                type: "datetimeoffset",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "CanteenStaff",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "CanteenStaff",
                type: "bit",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "CanteenStaff",
                type: "nvarchar(max)",
                nullable: true
            );
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatternPal.LoggingServer.Migrations
{
    /// <inheritdoc />
    public partial class ProgsnapUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubjectID",
                table: "Events",
                newName: "SubjectId");

            migrationBuilder.RenameColumn(
                name: "CodeStateID",
                table: "Events",
                newName: "CodeStateId");

            migrationBuilder.RenameColumn(
                name: "EventID",
                table: "Events",
                newName: "EventId");

            // run raw sql

            migrationBuilder.Sql("ALTER TABLE \"Events\" ALTER COLUMN \"SubjectId\" TYPE uuid USING \"SubjectId\"::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"Events\" ALTER COLUMN \"EventId\" TYPE uuid USING \"EventId\"::uuid;");

            migrationBuilder.Sql("ALTER TABLE \"Events\" ALTER COLUMN \"CodeStateId\" TYPE uuid USING \"CodeStateId\"::uuid;");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ClientDatetime",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ServerDatetime",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientDatetime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ServerDatetime",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "Events",
                newName: "SubjectID");

            migrationBuilder.RenameColumn(
                name: "CodeStateId",
                table: "Events",
                newName: "CodeStateID");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "Events",
                newName: "EventID");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectID",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "CodeStateID",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "EventID",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}

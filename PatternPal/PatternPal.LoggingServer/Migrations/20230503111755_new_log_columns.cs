using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatternPal.LoggingServer.Migrations
{
    /// <inheritdoc />
    public partial class new_log_columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeStateSection",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompileMessage",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompileMessageType",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceLocation",
                table: "Events",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeStateSection",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CompileMessage",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CompileMessageType",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SourceLocation",
                table: "Events");
        }
    }
}

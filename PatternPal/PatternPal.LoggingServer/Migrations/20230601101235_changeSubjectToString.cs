using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatternPal.LoggingServer.Migrations
{
    /// <inheritdoc />
    public partial class changeSubjectToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "Events",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                table: "Events",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}

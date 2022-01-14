using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDesign.LoggingAPI.Migrations
{
    public partial class currentDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommitID = table.Column<string>(type: "TEXT", nullable: true),
                    ExerciseID = table.Column<int>(type: "INTEGER", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SessionID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ActionTypeID = table.Column<string>(type: "TEXT", nullable: false),
                    ModeID = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ActionTypes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ExtensionErrors",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    ActionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtensionErrors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Modes",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExtensionVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    StartSession = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndSession = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TimeZone = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "ActionTypes",
                column: "ID",
                value: "Build");

            migrationBuilder.InsertData(
                table: "ActionTypes",
                column: "ID",
                value: "RebuildAll");

            migrationBuilder.InsertData(
                table: "ActionTypes",
                column: "ID",
                value: "Clean");

            migrationBuilder.InsertData(
                table: "ActionTypes",
                column: "ID",
                value: "Deploy");

            migrationBuilder.InsertData(
                table: "Modes",
                column: "ID",
                value: "Step by Step");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");

            migrationBuilder.DropTable(
                name: "ActionTypes");

            migrationBuilder.DropTable(
                name: "ExtensionErrors");

            migrationBuilder.DropTable(
                name: "Modes");

            migrationBuilder.DropTable(
                name: "Sessions");
        }
    }
}

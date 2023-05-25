using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatternPal.LoggingServer.Migrations
{
    /// <inheritdoc />
    public partial class RecognizerColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecognizerConfig",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecognizerResult",
                table: "Events",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecognizerConfig",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RecognizerResult",
                table: "Events");
        }
    }
}

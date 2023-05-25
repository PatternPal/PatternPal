using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatternPal.LoggingServer.Migrations
{
    /// <inheritdoc />
    public partial class AddExecutionResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExecutionResult",
                table: "Events",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionResult",
                table: "Events");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartAiChat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ChatSessions",
                type: "nvarchar(450)",
                nullable: true,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "AiConfigurations",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "OpenAI",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "OpenAI");

            migrationBuilder.AlterColumn<decimal>(
                name: "OutputCostPer1000Tokens",
                table: "AiConfigurations",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "InputCostPer1000Tokens",
                table: "AiConfigurations",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ChatSessions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true,
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<string>(
                name: "Provider",
                table: "AiConfigurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "OpenAI",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "OpenAI");

            migrationBuilder.AlterColumn<decimal>(
                name: "OutputCostPer1000Tokens",
                table: "AiConfigurations",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "InputCostPer1000Tokens",
                table: "AiConfigurations",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldPrecision: 18,
                oldScale: 6);
        }
    }
}

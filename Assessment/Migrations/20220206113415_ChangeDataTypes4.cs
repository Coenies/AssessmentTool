using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Migrations
{
    public partial class ChangeDataTypes4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Max_Supply",
                table: "DateItems",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<float>(
                name: "self_reported_circulating_supply",
                table: "DateItems",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "self_reported_market_cap",
                table: "DateItems",
                type: "real",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "self_reported_circulating_supply",
                table: "DateItems");

            migrationBuilder.DropColumn(
                name: "self_reported_market_cap",
                table: "DateItems");

            migrationBuilder.AlterColumn<double>(
                name: "Max_Supply",
                table: "DateItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}

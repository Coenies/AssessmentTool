using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DateItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cmc_Rank = table.Column<int>(type: "int", nullable: false),
                    Num_Market_Pairs = table.Column<int>(type: "int", nullable: false),
                    Circulating_Supply = table.Column<int>(type: "int", nullable: false),
                    Total_Supply = table.Column<int>(type: "int", nullable: false),
                    Max_Supply = table.Column<int>(type: "int", nullable: false),
                    Last_Updated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Added = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetrievedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fiat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataItemId = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Volume_24h = table.Column<int>(type: "int", nullable: false),
                    Volume_Change_24h = table.Column<float>(type: "real", nullable: false),
                    Percent_Change_1h = table.Column<float>(type: "real", nullable: false),
                    Percent_Change_24h = table.Column<float>(type: "real", nullable: false),
                    Percent_Change_7d = table.Column<float>(type: "real", nullable: false),
                    Market_Cap = table.Column<float>(type: "real", nullable: false),
                    Market_Cap_Dominance = table.Column<int>(type: "int", nullable: false),
                    Fully_Diluted_Market_Cap = table.Column<float>(type: "real", nullable: false),
                    Last_updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fiat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fiat_DateItems_DataItemId",
                        column: x => x.DataItemId,
                        principalTable: "DateItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DateItems_RetrievedOn",
                table: "DateItems",
                column: "RetrievedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Fiat_DataItemId",
                table: "Fiat",
                column: "DataItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fiat");

            migrationBuilder.DropTable(
                name: "DateItems");
        }
    }
}

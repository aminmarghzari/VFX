using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VFX.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForeignExchangeRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromCurrencyId = table.Column<int>(type: "int", nullable: false),
                    ToCurrencyId = table.Column<int>(type: "int", nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AskPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastRefreshed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignExchangeRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForeignExchangeRate_Currency_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForeignExchangeRate_Currency_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "USD", "United States Dollar" },
                    { 2, "EUR", "Euro" },
                    { 3, "CAD", "Canadian Dollar" },
                    { 4, "CHF", "Swiss Franc" },
                    { 5, "JPY", "Japanese Yen" },
                    { 6, "GBP", "British Pound Sterling" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currency_Code",
                table: "Currency",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForeignExchangeRate_FromCurrencyId",
                table: "ForeignExchangeRate",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignExchangeRate_ToCurrencyId",
                table: "ForeignExchangeRate",
                column: "ToCurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForeignExchangeRate");

            migrationBuilder.DropTable(
                name: "Currency");
        }
    }
}

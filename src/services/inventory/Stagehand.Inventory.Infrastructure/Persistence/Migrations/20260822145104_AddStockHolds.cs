using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stagehand.Inventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockHolds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stock_holds",
                schema: "inventory",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    StockItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_holds", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_stock_holds_stock_items_StockItemId",
                        column: x => x.StockItemId,
                        principalSchema: "inventory",
                        principalTable: "stock_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_items_ListingId",
                schema: "inventory",
                table: "stock_items",
                column: "ListingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_holds_StockItemId",
                schema: "inventory",
                table: "stock_holds",
                column: "StockItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_holds",
                schema: "inventory");

            migrationBuilder.DropIndex(
                name: "IX_stock_items_ListingId",
                schema: "inventory",
                table: "stock_items");
        }
    }
}

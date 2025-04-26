using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MertcanDoner.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedOptionsProperly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedOption_OrderItems_OrderItemId",
                table: "SelectedOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedOption",
                table: "SelectedOption");

            migrationBuilder.RenameTable(
                name: "SelectedOption",
                newName: "SelectedOptions");

            migrationBuilder.RenameIndex(
                name: "IX_SelectedOption_OrderItemId",
                table: "SelectedOptions",
                newName: "IX_SelectedOptions_OrderItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedOptions",
                table: "SelectedOptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedOptions_OrderItems_OrderItemId",
                table: "SelectedOptions",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedOptions_OrderItems_OrderItemId",
                table: "SelectedOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SelectedOptions",
                table: "SelectedOptions");

            migrationBuilder.RenameTable(
                name: "SelectedOptions",
                newName: "SelectedOption");

            migrationBuilder.RenameIndex(
                name: "IX_SelectedOptions_OrderItemId",
                table: "SelectedOption",
                newName: "IX_SelectedOption_OrderItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SelectedOption",
                table: "SelectedOption",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedOption_OrderItems_OrderItemId",
                table: "SelectedOption",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

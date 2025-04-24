using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MertcanDoner.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedOptionsToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SelectedOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderItemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedOption_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedOption_OrderItemId",
                table: "SelectedOption",
                column: "OrderItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelectedOption");
        }
    }
}

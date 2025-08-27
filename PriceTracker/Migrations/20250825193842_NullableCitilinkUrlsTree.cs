using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTracker.Migrations
{
    /// <inheritdoc />
    public partial class NullableCitilinkUrlsTree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CitilinkCatalogUrlsTree~",
                table: "CitilinkParsingExecutionStateEntity");

            migrationBuilder.AlterColumn<int>(
                name: "CatalogUrlsId",
                table: "CitilinkParsingExecutionStateEntity",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CitilinkCatalogUrlsTree~",
                table: "CitilinkParsingExecutionStateEntity",
                column: "CatalogUrlsId",
                principalTable: "CitilinkCatalogUrlsTreeEntity",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CitilinkCatalogUrlsTree~",
                table: "CitilinkParsingExecutionStateEntity");

            migrationBuilder.AlterColumn<int>(
                name: "CatalogUrlsId",
                table: "CitilinkParsingExecutionStateEntity",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CitilinkCatalogUrlsTree~",
                table: "CitilinkParsingExecutionStateEntity",
                column: "CatalogUrlsId",
                principalTable: "CitilinkCatalogUrlsTreeEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

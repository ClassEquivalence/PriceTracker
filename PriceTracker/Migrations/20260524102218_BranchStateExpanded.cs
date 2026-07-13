using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTracker.Migrations
{
    /// <inheritdoc />
    public partial class BranchStateExpanded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsBranchProcessed",
                table: "CitilinkCatalogBranchEntity",
                newName: "DescendantsProcessed");

            migrationBuilder.RenameColumn(
                name: "CurrentCatalogUrl",
                table: "CitilinkCatalogBranchEntity",
                newName: "Url");

            migrationBuilder.AddColumn<int>(
                name: "AugmentationState",
                table: "CitilinkCatalogBranchEntity",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NodeType",
                table: "CitilinkCatalogBranchEntity",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AugmentationState",
                table: "CitilinkCatalogBranchEntity");

            migrationBuilder.DropColumn(
                name: "NodeType",
                table: "CitilinkCatalogBranchEntity");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "CitilinkCatalogBranchEntity",
                newName: "CurrentCatalogUrl");

            migrationBuilder.RenameColumn(
                name: "DescendantsProcessed",
                table: "CitilinkCatalogBranchEntity",
                newName: "IsBranchProcessed");
        }
    }
}

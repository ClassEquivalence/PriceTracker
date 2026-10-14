using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PriceTracker.Migrations
{
    /// <inheritdoc />
    public partial class BranchStateSpecified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitilinkCatalogBranchEntity_CitilinkCatalogBranchEntity_Cit~",
                table: "CitilinkCatalogBranchEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_CitilinkCatalogUrlsTreeEntity_CitilinkCatalogBranchEntity_R~",
                table: "CitilinkCatalogUrlsTreeEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CitilinkCatalogUrlsTree~",
                table: "CitilinkParsingExecutionStateEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitilinkCatalogUrlsTreeEntity",
                table: "CitilinkCatalogUrlsTreeEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitilinkCatalogBranchEntity",
                table: "CitilinkCatalogBranchEntity");

            migrationBuilder.DropColumn(
                name: "AugmentationState",
                table: "CitilinkCatalogBranchEntity");

            migrationBuilder.RenameTable(
                name: "CitilinkCatalogUrlsTreeEntity",
                newName: "CatalogUrlsTreeEntity");

            migrationBuilder.RenameTable(
                name: "CitilinkCatalogBranchEntity",
                newName: "CitilinkBranches");

            migrationBuilder.RenameIndex(
                name: "IX_CitilinkCatalogUrlsTreeEntity_RootId",
                table: "CatalogUrlsTreeEntity",
                newName: "IX_CatalogUrlsTreeEntity_RootId");

            migrationBuilder.RenameColumn(
                name: "DescendantsProcessed",
                table: "CitilinkBranches",
                newName: "ScrapeTargetsProcessed");

            migrationBuilder.RenameIndex(
                name: "IX_CitilinkCatalogBranchEntity_CitilinkCatalogBranchEntityId",
                table: "CitilinkBranches",
                newName: "IX_CitilinkBranches_CitilinkCatalogBranchEntityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogUrlsTreeEntity",
                table: "CatalogUrlsTreeEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitilinkBranches",
                table: "CitilinkBranches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogUrlsTreeEntity_CitilinkBranches_RootId",
                table: "CatalogUrlsTreeEntity",
                column: "RootId",
                principalTable: "CitilinkBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CitilinkBranches_CitilinkBranches_CitilinkCatalogBranchEnti~",
                table: "CitilinkBranches",
                column: "CitilinkCatalogBranchEntityId",
                principalTable: "CitilinkBranches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CatalogUrlsTreeEntity_C~",
                table: "CitilinkParsingExecutionStateEntity",
                column: "CatalogUrlsId",
                principalTable: "CatalogUrlsTreeEntity",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogUrlsTreeEntity_CitilinkBranches_RootId",
                table: "CatalogUrlsTreeEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_CitilinkBranches_CitilinkBranches_CitilinkCatalogBranchEnti~",
                table: "CitilinkBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CatalogUrlsTreeEntity_C~",
                table: "CitilinkParsingExecutionStateEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CitilinkBranches",
                table: "CitilinkBranches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogUrlsTreeEntity",
                table: "CatalogUrlsTreeEntity");

            migrationBuilder.RenameTable(
                name: "CitilinkBranches",
                newName: "CitilinkCatalogBranchEntity");

            migrationBuilder.RenameTable(
                name: "CatalogUrlsTreeEntity",
                newName: "CitilinkCatalogUrlsTreeEntity");

            migrationBuilder.RenameColumn(
                name: "ScrapeTargetsProcessed",
                table: "CitilinkCatalogBranchEntity",
                newName: "DescendantsProcessed");

            migrationBuilder.RenameIndex(
                name: "IX_CitilinkBranches_CitilinkCatalogBranchEntityId",
                table: "CitilinkCatalogBranchEntity",
                newName: "IX_CitilinkCatalogBranchEntity_CitilinkCatalogBranchEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_CatalogUrlsTreeEntity_RootId",
                table: "CitilinkCatalogUrlsTreeEntity",
                newName: "IX_CitilinkCatalogUrlsTreeEntity_RootId");

            migrationBuilder.AddColumn<int>(
                name: "AugmentationState",
                table: "CitilinkCatalogBranchEntity",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitilinkCatalogBranchEntity",
                table: "CitilinkCatalogBranchEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CitilinkCatalogUrlsTreeEntity",
                table: "CitilinkCatalogUrlsTreeEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CitilinkCatalogBranchEntity_CitilinkCatalogBranchEntity_Cit~",
                table: "CitilinkCatalogBranchEntity",
                column: "CitilinkCatalogBranchEntityId",
                principalTable: "CitilinkCatalogBranchEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CitilinkCatalogUrlsTreeEntity_CitilinkCatalogBranchEntity_R~",
                table: "CitilinkCatalogUrlsTreeEntity",
                column: "RootId",
                principalTable: "CitilinkCatalogBranchEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CitilinkCatalogUrlsTree~",
                table: "CitilinkParsingExecutionStateEntity",
                column: "CatalogUrlsId",
                principalTable: "CitilinkCatalogUrlsTreeEntity",
                principalColumn: "Id");
        }
    }
}

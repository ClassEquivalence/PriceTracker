using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PriceTracker.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCitilinkExtractionExecutionState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCatalogUrl",
                table: "CitilinkParsingExecutionStateEntity");

            migrationBuilder.RenameColumn(
                name: "CatalogPageNumber",
                table: "CitilinkParsingExecutionStateEntity",
                newName: "CatalogUrlsId");

            migrationBuilder.CreateTable(
                name: "CitilinkCatalogBranchEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrentCatalogUrl = table.Column<string>(type: "text", nullable: false),
                    IsBranchProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    CitilinkCatalogBranchEntityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitilinkCatalogBranchEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitilinkCatalogBranchEntity_CitilinkCatalogBranchEntity_Cit~",
                        column: x => x.CitilinkCatalogBranchEntityId,
                        principalTable: "CitilinkCatalogBranchEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CitilinkCatalogUrlsTreeEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RootId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitilinkCatalogUrlsTreeEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitilinkCatalogUrlsTreeEntity_CitilinkCatalogBranchEntity_R~",
                        column: x => x.RootId,
                        principalTable: "CitilinkCatalogBranchEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitilinkParsingExecutionStateEntity_CatalogUrlsId",
                table: "CitilinkParsingExecutionStateEntity",
                column: "CatalogUrlsId");

            migrationBuilder.CreateIndex(
                name: "IX_CitilinkCatalogBranchEntity_CitilinkCatalogBranchEntityId",
                table: "CitilinkCatalogBranchEntity",
                column: "CitilinkCatalogBranchEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CitilinkCatalogUrlsTreeEntity_RootId",
                table: "CitilinkCatalogUrlsTreeEntity",
                column: "RootId");

            migrationBuilder.AddForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CitilinkCatalogUrlsTree~",
                table: "CitilinkParsingExecutionStateEntity",
                column: "CatalogUrlsId",
                principalTable: "CitilinkCatalogUrlsTreeEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CitilinkParsingExecutionStateEntity_CitilinkCatalogUrlsTree~",
                table: "CitilinkParsingExecutionStateEntity");

            migrationBuilder.DropTable(
                name: "CitilinkCatalogUrlsTreeEntity");

            migrationBuilder.DropTable(
                name: "CitilinkCatalogBranchEntity");

            migrationBuilder.DropIndex(
                name: "IX_CitilinkParsingExecutionStateEntity_CatalogUrlsId",
                table: "CitilinkParsingExecutionStateEntity");

            migrationBuilder.RenameColumn(
                name: "CatalogUrlsId",
                table: "CitilinkParsingExecutionStateEntity",
                newName: "CatalogPageNumber");

            migrationBuilder.AddColumn<string>(
                name: "CurrentCatalogUrl",
                table: "CitilinkParsingExecutionStateEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

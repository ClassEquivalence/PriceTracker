using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PriceTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CitilinkExtractionStorageStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StorageState = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitilinkExtractionStorageStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CitilinkParsingExecutionStateEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrentCatalogUrl = table.Column<string>(type: "text", nullable: false),
                    CatalogPageNumber = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitilinkParsingExecutionStateEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeExtractionProcessHappened",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastTimeStarted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastTimeFinished = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeExtractionProcessHappened", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Merches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShopId = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    CitilinkId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Merches_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchPriceHistoryEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MerchId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchPriceHistoryEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchPriceHistoryEntities_Merches_MerchId",
                        column: x => x.MerchId,
                        principalTable: "Merches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimestampedPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MerchPriceHistoryId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimestampedPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimestampedPrices_MerchPriceHistoryEntities_MerchPriceHisto~",
                        column: x => x.MerchPriceHistoryId,
                        principalTable: "MerchPriceHistoryEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CurrentPricePointerEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MerchPriceHistoryId = table.Column<int>(type: "integer", nullable: false),
                    CurrentPriceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentPricePointerEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrentPricePointerEntity_MerchPriceHistoryEntities_MerchPr~",
                        column: x => x.MerchPriceHistoryId,
                        principalTable: "MerchPriceHistoryEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrentPricePointerEntity_TimestampedPrices_CurrentPriceId",
                        column: x => x.CurrentPriceId,
                        principalTable: "TimestampedPrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrentPricePointerEntity_CurrentPriceId",
                table: "CurrentPricePointerEntity",
                column: "CurrentPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentPricePointerEntity_MerchPriceHistoryId",
                table: "CurrentPricePointerEntity",
                column: "MerchPriceHistoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Merches_ShopId",
                table: "Merches",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchPriceHistoryEntities_MerchId",
                table: "MerchPriceHistoryEntities",
                column: "MerchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimestampedPrices_MerchPriceHistoryId",
                table: "TimestampedPrices",
                column: "MerchPriceHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitilinkExtractionStorageStates");

            migrationBuilder.DropTable(
                name: "CitilinkParsingExecutionStateEntity");

            migrationBuilder.DropTable(
                name: "CurrentPricePointerEntity");

            migrationBuilder.DropTable(
                name: "TimeExtractionProcessHappened");

            migrationBuilder.DropTable(
                name: "TimestampedPrices");

            migrationBuilder.DropTable(
                name: "MerchPriceHistoryEntities");

            migrationBuilder.DropTable(
                name: "Merches");

            migrationBuilder.DropTable(
                name: "Shops");
        }
    }
}

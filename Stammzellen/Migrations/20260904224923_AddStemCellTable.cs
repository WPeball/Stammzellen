using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stammzellen.Migrations
{
    /// <inheritdoc />
    public partial class AddStemCellTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StemCellSamples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CellType = table.Column<string>(type: "TEXT", nullable: false),
                    BatchNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ViabilityPercentage = table.Column<double>(type: "REAL", nullable: false),
                    CellCountMillions = table.Column<double>(type: "REAL", nullable: false),
                    StorageDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StorageLocation = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StemCellSamples", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StemCellSamples");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stammzellen.Migrations
{
    /// <inheritdoc />
    public partial class AddAutorToBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Autor",
                table: "BlogPosts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Autor",
                table: "BlogPosts");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class CountryDeleteAdditionalName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Countries_LongName",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "LongName",
                table: "Countries");

            migrationBuilder.RenameColumn(
                name: "ShortName",
                table: "Countries",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_ShortName",
                table: "Countries",
                newName: "IX_Countries_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Countries",
                newName: "ShortName");

            migrationBuilder.RenameIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                newName: "IX_Countries_ShortName");

            migrationBuilder.AddColumn<string>(
                name: "LongName",
                table: "Countries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_LongName",
                table: "Countries",
                column: "LongName",
                unique: true);
        }
    }
}

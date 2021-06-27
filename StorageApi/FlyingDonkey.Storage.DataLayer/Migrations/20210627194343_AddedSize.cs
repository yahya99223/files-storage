using Microsoft.EntityFrameworkCore.Migrations;

namespace FlyingDonkey.Storage.DataLayer.Migrations
{
    public partial class AddedSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "Files",
                newName: "UploadDate");

            migrationBuilder.AddColumn<double>(
                name: "Size",
                table: "Files",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "UploadDate",
                table: "Files",
                newName: "LastUpdated");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHasProfileImageToPersons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.AddColumn<bool>(
                name: "HasProfileImage",
                table: "Persons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Update existing persons with profile images
            _ = migrationBuilder.Sql(
                @"UPDATE Persons 
                  SET HasProfileImage = 1 
                  WHERE ProfileImageUrl IS NOT NULL AND ProfileImageUrl != ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            _ = migrationBuilder.DropColumn(
                name: "HasProfileImage",
                table: "Persons");
        }
    }
}

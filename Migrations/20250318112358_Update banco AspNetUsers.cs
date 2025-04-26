using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patrimonios.Migrations
{
    /// <inheritdoc />
    public partial class UpdatebancoAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
           name: "Grupo",
           table: "AspNetUsers",
           type: "int",
           nullable: false,
           defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "Grupo",
            table: "AspNetUsers");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patrimonios.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Equipamento",
                table: "Patrimonios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoConservacao",
                table: "Patrimonios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsavel",
                table: "Patrimonios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Equipamento",
                table: "Patrimonios");

            migrationBuilder.DropColumn(
                name: "EstadoConservacao",
                table: "Patrimonios");

            migrationBuilder.DropColumn(
                name: "Responsavel",
                table: "Patrimonios");
        }
    }
}

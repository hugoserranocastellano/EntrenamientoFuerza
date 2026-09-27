using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntrenamientoFuerza.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregaImagenAEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imagen_url",
                table: "ejercicios",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imagen_url",
                table: "ejercicios");
        }
    }
}

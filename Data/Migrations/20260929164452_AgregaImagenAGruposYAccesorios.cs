using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntrenamientoFuerza.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregaImagenAGruposYAccesorios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imagen_url",
                table: "grupos_musculares",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "imagen_url",
                table: "accesorios",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imagen_url",
                table: "grupos_musculares");

            migrationBuilder.DropColumn(
                name: "imagen_url",
                table: "accesorios");
        }
    }
}

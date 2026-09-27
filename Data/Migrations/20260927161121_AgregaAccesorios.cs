using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EntrenamientoFuerza.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregaAccesorios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accesorios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accesorios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ejercicio_accesorios",
                columns: table => new
                {
                    ejercicio_id = table.Column<int>(type: "integer", nullable: false),
                    accesorio_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ejercicio_accesorios", x => new { x.ejercicio_id, x.accesorio_id });
                    table.ForeignKey(
                        name: "FK_ejercicio_accesorios_accesorios_accesorio_id",
                        column: x => x.accesorio_id,
                        principalTable: "accesorios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ejercicio_accesorios_ejercicios_ejercicio_id",
                        column: x => x.ejercicio_id,
                        principalTable: "ejercicios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_accesorios_nombre",
                table: "accesorios",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ejercicio_accesorios_accesorio_id",
                table: "ejercicio_accesorios",
                column: "accesorio_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ejercicio_accesorios");

            migrationBuilder.DropTable(
                name: "accesorios");
        }
    }
}

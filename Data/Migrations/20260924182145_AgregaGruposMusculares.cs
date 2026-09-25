using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EntrenamientoFuerza.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregaGruposMusculares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "grupo_muscular_id",
                table: "ejercicios",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "grupos_musculares",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grupos_musculares", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ejercicios_grupo_muscular_id",
                table: "ejercicios",
                column: "grupo_muscular_id");

            migrationBuilder.CreateIndex(
                name: "IX_grupos_musculares_nombre",
                table: "grupos_musculares",
                column: "nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ejercicios_grupos_musculares_grupo_muscular_id",
                table: "ejercicios",
                column: "grupo_muscular_id",
                principalTable: "grupos_musculares",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ejercicios_grupos_musculares_grupo_muscular_id",
                table: "ejercicios");

            migrationBuilder.DropTable(
                name: "grupos_musculares");

            migrationBuilder.DropIndex(
                name: "IX_ejercicios_grupo_muscular_id",
                table: "ejercicios");

            migrationBuilder.DropColumn(
                name: "grupo_muscular_id",
                table: "ejercicios");
        }
    }
}

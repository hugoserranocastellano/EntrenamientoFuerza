using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EntrenamientoFuerza.Data.Migrations
{
    /// <inheritdoc />
    public partial class InicialEsquema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ejercicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    video_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ejercicios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    nombre_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    rol = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_alta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rutinas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    creado_por_id = table.Column<int>(type: "integer", nullable: false),
                    activa = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutinas", x => x.id);
                    table.ForeignKey(
                        name: "FK_rutinas_usuarios_creado_por_id",
                        column: x => x.creado_por_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rutina_ejercicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rutina_id = table.Column<int>(type: "integer", nullable: false),
                    ejercicio_id = table.Column<int>(type: "integer", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    series = table.Column<int>(type: "integer", nullable: false),
                    repeticiones = table.Column<int>(type: "integer", nullable: true),
                    duracion_segundos = table.Column<int>(type: "integer", nullable: true),
                    descanso_segundos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rutina_ejercicios", x => x.id);
                    table.ForeignKey(
                        name: "FK_rutina_ejercicios_ejercicios_ejercicio_id",
                        column: x => x.ejercicio_id,
                        principalTable: "ejercicios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_rutina_ejercicios_rutinas_rutina_id",
                        column: x => x.rutina_id,
                        principalTable: "rutinas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sesiones_diario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    rutina_id = table.Column<int>(type: "integer", nullable: true),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    notas = table.Column<string>(type: "text", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sesiones_diario", x => x.id);
                    table.ForeignKey(
                        name: "FK_sesiones_diario_rutinas_rutina_id",
                        column: x => x.rutina_id,
                        principalTable: "rutinas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_sesiones_diario_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sesion_ejercicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sesion_diario_id = table.Column<int>(type: "integer", nullable: false),
                    ejercicio_id = table.Column<int>(type: "integer", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    notas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sesion_ejercicios", x => x.id);
                    table.ForeignKey(
                        name: "FK_sesion_ejercicios_ejercicios_ejercicio_id",
                        column: x => x.ejercicio_id,
                        principalTable: "ejercicios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_sesion_ejercicios_sesiones_diario_sesion_diario_id",
                        column: x => x.sesion_diario_id,
                        principalTable: "sesiones_diario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sesion_series",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sesion_ejercicio_id = table.Column<int>(type: "integer", nullable: false),
                    numero_serie = table.Column<int>(type: "integer", nullable: false),
                    repeticiones_realizadas = table.Column<int>(type: "integer", nullable: true),
                    duracion_segundos_real = table.Column<int>(type: "integer", nullable: true),
                    peso_kg = table.Column<decimal>(type: "numeric(6,2)", nullable: true),
                    completada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sesion_series", x => x.id);
                    table.ForeignKey(
                        name: "FK_sesion_series_sesion_ejercicios_sesion_ejercicio_id",
                        column: x => x.sesion_ejercicio_id,
                        principalTable: "sesion_ejercicios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rutina_ejercicios_ejercicio_id",
                table: "rutina_ejercicios",
                column: "ejercicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_rutina_ejercicios_rutina_id",
                table: "rutina_ejercicios",
                column: "rutina_id");

            migrationBuilder.CreateIndex(
                name: "IX_rutinas_creado_por_id",
                table: "rutinas",
                column: "creado_por_id");

            migrationBuilder.CreateIndex(
                name: "IX_sesion_ejercicios_ejercicio_id",
                table: "sesion_ejercicios",
                column: "ejercicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_sesion_ejercicios_sesion_diario_id",
                table: "sesion_ejercicios",
                column: "sesion_diario_id");

            migrationBuilder.CreateIndex(
                name: "IX_sesion_series_sesion_ejercicio_id",
                table: "sesion_series",
                column: "sesion_ejercicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_sesiones_diario_rutina_id",
                table: "sesiones_diario",
                column: "rutina_id");

            migrationBuilder.CreateIndex(
                name: "IX_sesiones_diario_usuario_id",
                table: "sesiones_diario",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_nombre_usuario",
                table: "usuarios",
                column: "nombre_usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rutina_ejercicios");

            migrationBuilder.DropTable(
                name: "sesion_series");

            migrationBuilder.DropTable(
                name: "sesion_ejercicios");

            migrationBuilder.DropTable(
                name: "ejercicios");

            migrationBuilder.DropTable(
                name: "sesiones_diario");

            migrationBuilder.DropTable(
                name: "rutinas");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}

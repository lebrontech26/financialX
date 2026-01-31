using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cuil = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Domicilio_Calle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Domicilio_Localidad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Domicilio_Provincia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EstaActivo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualizadoEn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesScoring",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    CalculadoEn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PuntajeBase = table.Column<int>(type: "INTEGER", nullable: false),
                    PuntajeFinal = table.Column<int>(type: "INTEGER", nullable: false),
                    Categoria = table.Column<int>(type: "INTEGER", nullable: false),
                    SinEvidenciaCrediticia = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesScoring", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesScoring_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AjustesScoring",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HistorialScoringId = table.Column<int>(type: "INTEGER", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Valor = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AjustesScoring", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AjustesScoring_HistorialesScoring_HistorialScoringId",
                        column: x => x.HistorialScoringId,
                        principalTable: "HistorialesScoring",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertasScoring",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HistorialScoringId = table.Column<int>(type: "INTEGER", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasScoring", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertasScoring_HistorialesScoring_HistorialScoringId",
                        column: x => x.HistorialScoringId,
                        principalTable: "HistorialesScoring",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AjustesScoring_Codigo",
                table: "AjustesScoring",
                column: "Codigo");

            migrationBuilder.CreateIndex(
                name: "IX_AjustesScoring_HistorialScoringId",
                table: "AjustesScoring",
                column: "HistorialScoringId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertasScoring_HistorialScoringId",
                table: "AlertasScoring",
                column: "HistorialScoringId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cuil_Unique",
                table: "Clientes",
                column: "Cuil",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EstaActivo",
                table: "Clientes",
                column: "EstaActivo");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EstaActivo_CreadoEn",
                table: "Clientes",
                columns: new[] { "EstaActivo", "CreadoEn" });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_NombreApellido",
                table: "Clientes",
                columns: new[] { "Nombre", "Apellido" });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesScoring_ClienteId",
                table: "HistorialesScoring",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesScoring_ClienteId_CalculadoEn_Desc",
                table: "HistorialesScoring",
                columns: new[] { "ClienteId", "CalculadoEn" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AjustesScoring");

            migrationBuilder.DropTable(
                name: "AlertasScoring");

            migrationBuilder.DropTable(
                name: "HistorialesScoring");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}

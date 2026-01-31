using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
namespace Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReglasScoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReglasAjustes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TipoAjuste = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorMinimo = table.Column<int>(type: "INTEGER", nullable: false),
                    Ajuste = table.Column<int>(type: "INTEGER", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EstaActivo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    ValorMaximo = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglasAjustes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReglasScoreBase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Situacion = table.Column<int>(type: "INTEGER", nullable: false),
                    ScoreBase = table.Column<int>(type: "INTEGER", nullable: false),
                    CreadoEn = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EstaActivo = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglasScoreBase", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ReglasAjustes",
                columns: new[] { "Id", "Ajuste", "Codigo", "Descripcion", "TipoAjuste", "ValorMaximo", "ValorMinimo" },
                values: new object[,]
                {
                    { 1, 0, "PEOR_SIT_24M_NORMAL", "Sin deterioro histórico", 1, 1, 1 },
                    { 2, -5, "PEOR_SIT_24M_NIVEL_2", "Riesgo bajo histórico detectado", 1, 2, 2 },
                    { 3, -12, "PEOR_SIT_24M_NIVEL_3", "Dificultades históricas detectadas", 1, 3, 3 },
                    { 4, -20, "PEOR_SIT_24M_NIVEL_4", "Situación severa histórica", 1, 4, 4 },
                    { 5, -35, "PEOR_SIT_24M_CRITICA", "Situación crítica en historial", 1, null, 5 },
                    { 6, 0, "MESES_MORA_NINGUNO", "Sin períodos de mora", 2, 0, 0 },
                    { 7, -5, "MESES_MORA_OCASIONAL", "1-2 meses en mora", 2, 2, 1 },
                    { 8, -12, "MESES_MORA_FRECUENTE", "3-5 meses en mora", 2, 5, 3 },
                    { 9, -20, "MESES_MORA_PERSISTENTE", "6-10 meses en mora", 2, 10, 6 },
                    { 10, -30, "MESES_MORA_CRONICA", "Más de 10 meses en mora", 2, null, 11 },
                    { 11, -20, "MORA_MUY_RECIENTE", "Mora en los últimos 2 meses", 3, 2, 0 },
                    { 12, -12, "MORA_RECIENTE", "Mora hace 3-5 meses", 3, 5, 3 },
                    { 13, -6, "MORA_MODERADA", "Mora hace 6-11 meses", 3, 11, 6 },
                    { 14, 0, "MORA_ANTIGUA", "Mora hace más de 12 meses", 3, null, 12 },
                    { 15, 0, "ENTIDADES_BAJA", "0-1 entidades", 4, 1, 0 },
                    { 16, -4, "ENTIDADES_MEDIA", "2-3 entidades", 4, 3, 2 },
                    { 17, -8, "ENTIDADES_ALTA", "4-6 entidades", 4, 6, 4 },
                    { 18, -12, "ENTIDADES_MUY_ALTA", "Más de 6 entidades", 4, null, 7 },
                    { 19, 0, "CHEQUES_NINGUNO", "Sin cheques rechazados", 5, 0, 0 },
                    { 20, -8, "CHEQUES_UNO", "1 cheque rechazado", 5, 1, 1 },
                    { 21, -15, "CHEQUES_DOS", "2 cheques rechazados", 5, 2, 2 },
                    { 22, -30, "CHEQUES_MULTIPLES", "3 o más cheques rechazados", 5, null, 3 }
                });

            migrationBuilder.InsertData(
                table: "ReglasScoreBase",
                columns: new[] { "Id", "EstaActivo", "ScoreBase", "Situacion" },
                values: new object[,]
                {
                    { 1, true, 80, 1 },
                    { 2, true, 55, 2 },
                    { 3, true, 35, 3 },
                    { 4, true, 15, 4 },
                    { 5, true, 0, 5 },
                    { 6, true, 0, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReglasAjustes_Tipo_Activa",
                table: "ReglasAjustes",
                columns: new[] { "TipoAjuste", "EstaActivo" });

            migrationBuilder.CreateIndex(
                name: "IX_ReglasAjustes_Tipo_Rango",
                table: "ReglasAjustes",
                columns: new[] { "TipoAjuste", "ValorMinimo", "ValorMaximo" });

            migrationBuilder.CreateIndex(
                name: "IX_ReglasScoreBase_Situacion_Activa",
                table: "ReglasScoreBase",
                columns: new[] { "Situacion", "EstaActivo" },
                unique: true,
                filter: "EstaActivo = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReglasAjustes");

            migrationBuilder.DropTable(
                name: "ReglasScoreBase");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturesBcraToHistorialScoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HistorialesScoring_ClienteId_CalculadoEn_Desc",
                table: "HistorialesScoring");

            migrationBuilder.DropIndex(
                name: "IX_AjustesScoring_Codigo",
                table: "AjustesScoring");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CalculadoEn",
                table: "HistorialesScoring",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "CantidadEntidadesActual",
                table: "HistorialesScoring",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChequesEventos12m",
                table: "HistorialesScoring",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxSituacionActual",
                table: "HistorialesScoring",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MesesMora24m",
                table: "HistorialesScoring",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeorSituacion24m",
                table: "HistorialesScoring",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecenciaMora",
                table: "HistorialesScoring",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesScoring_Cliente_Fecha",
                table: "HistorialesScoring",
                columns: new[] { "ClienteId", "CalculadoEn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HistorialesScoring_Cliente_Fecha",
                table: "HistorialesScoring");

            migrationBuilder.DropColumn(
                name: "CantidadEntidadesActual",
                table: "HistorialesScoring");

            migrationBuilder.DropColumn(
                name: "ChequesEventos12m",
                table: "HistorialesScoring");

            migrationBuilder.DropColumn(
                name: "MaxSituacionActual",
                table: "HistorialesScoring");

            migrationBuilder.DropColumn(
                name: "MesesMora24m",
                table: "HistorialesScoring");

            migrationBuilder.DropColumn(
                name: "PeorSituacion24m",
                table: "HistorialesScoring");

            migrationBuilder.DropColumn(
                name: "RecenciaMora",
                table: "HistorialesScoring");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CalculadoEn",
                table: "HistorialesScoring",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesScoring_ClienteId_CalculadoEn_Desc",
                table: "HistorialesScoring",
                columns: new[] { "ClienteId", "CalculadoEn" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_AjustesScoring_Codigo",
                table: "AjustesScoring",
                column: "Codigo");
        }
    }
}

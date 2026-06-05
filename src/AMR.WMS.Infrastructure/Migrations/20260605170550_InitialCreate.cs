using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMR.WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Localizacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Zona = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Corredor = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Prateleira = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Posicao = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Capacidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Ocupacao = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoLocalizacao = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localizacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movimentacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalizacaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimentacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimentacoes_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalTable: "Localizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Localizacoes_Codigo",
                table: "Localizacoes",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_LocalizacaoId",
                table: "Movimentacoes",
                column: "LocalizacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimentacoes");

            migrationBuilder.DropTable(
                name: "Localizacoes");
        }
    }
}

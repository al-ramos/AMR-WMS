using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMR.WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeparacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrdensSeparacao",
                columns: table => new
                {
                    Id            = table.Column<Guid>(type: "TEXT", nullable: false),
                    PedidoVendaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status        = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao   = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdensSeparacao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensSeparacao",
                columns: table => new
                {
                    Id               = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrdemSeparacaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProdutoId        = table.Column<int>(type: "INTEGER", nullable: false),
                    LocalizacaoId    = table.Column<Guid>(type: "TEXT", nullable: true),
                    QntSolicitada    = table.Column<int>(type: "INTEGER", nullable: false),
                    QntSeparada      = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensSeparacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensSeparacao_OrdensSeparacao_OrdemSeparacaoId",
                        column: x => x.OrdemSeparacaoId,
                        principalTable: "OrdensSeparacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensSeparacao_Localizacoes_LocalizacaoId",
                        column: x => x.LocalizacaoId,
                        principalTable: "Localizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensSeparacao_OrdemSeparacaoId",
                table: "ItensSeparacao",
                column: "OrdemSeparacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensSeparacao_LocalizacaoId",
                table: "ItensSeparacao",
                column: "LocalizacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ItensSeparacao");
            migrationBuilder.DropTable(name: "OrdensSeparacao");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Angajati",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cnp = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Marca = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Angajati", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Masini",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumarDeKilometri = table.Column<int>(type: "int", nullable: false),
                    AnFabricatie = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerieDeSasiu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NumarDeInmatriculare = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Masini", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deplasari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motiv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataPlecare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataSosire = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SrcX = table.Column<double>(type: "float", nullable: false),
                    SrcY = table.Column<double>(type: "float", nullable: false),
                    DstX = table.Column<double>(type: "float", nullable: false),
                    DstY = table.Column<double>(type: "float", nullable: false),
                    MasinaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AngajatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Observatii = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deplasari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deplasari_Angajati_AngajatId",
                        column: x => x.AngajatId,
                        principalTable: "Angajati",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deplasari_Masini_MasinaId",
                        column: x => x.MasinaId,
                        principalTable: "Masini",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Angajati_Cnp",
                table: "Angajati",
                column: "Cnp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Angajati_Email",
                table: "Angajati",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Angajati_Marca",
                table: "Angajati",
                column: "Marca",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deplasari_AngajatId",
                table: "Deplasari",
                column: "AngajatId");

            migrationBuilder.CreateIndex(
                name: "IX_Deplasari_MasinaId",
                table: "Deplasari",
                column: "MasinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Masini_NumarDeInmatriculare",
                table: "Masini",
                column: "NumarDeInmatriculare",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Masini_SerieDeSasiu",
                table: "Masini",
                column: "SerieDeSasiu",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deplasari");

            migrationBuilder.DropTable(
                name: "Angajati");

            migrationBuilder.DropTable(
                name: "Masini");
        }
    }
}

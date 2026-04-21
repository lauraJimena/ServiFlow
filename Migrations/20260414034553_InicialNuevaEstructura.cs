using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiFlow.Migrations
{
    /// <inheritdoc />
    public partial class InicialNuevaEstructura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreCliente",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "Servicio",
                table: "Citas");

            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "Disponibilidades",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Citas",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ServicioId",
                table: "Citas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Citas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EmprendimientoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicios_Emprendimientos_EmprendimientoId",
                        column: x => x.EmprendimientoId,
                        principalTable: "Emprendimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disponibilidades_ServicioId",
                table: "Disponibilidades",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_ServicioId",
                table: "Citas",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_UsuarioId",
                table: "Citas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_EmprendimientoId",
                table: "Servicios",
                column: "EmprendimientoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Servicios_ServicioId",
                table: "Citas",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Usuarios_UsuarioId",
                table: "Citas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Disponibilidades_Servicios_ServicioId",
                table: "Disponibilidades",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Servicios_ServicioId",
                table: "Citas");

            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Usuarios_UsuarioId",
                table: "Citas");

            migrationBuilder.DropForeignKey(
                name: "FK_Disponibilidades_Servicios_ServicioId",
                table: "Disponibilidades");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Disponibilidades_ServicioId",
                table: "Disponibilidades");

            migrationBuilder.DropIndex(
                name: "IX_Citas_ServicioId",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_UsuarioId",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "ServicioId",
                table: "Disponibilidades");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "ServicioId",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Citas");

            migrationBuilder.AddColumn<string>(
                name: "NombreCliente",
                table: "Citas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Servicio",
                table: "Citas",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
